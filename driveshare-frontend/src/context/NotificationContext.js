import React, { createContext, useState, useEffect, useContext, useRef, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { AuthContext } from './AuthContext';
import { toast } from 'react-toastify';
import notificationService from '../services/notificationService';

export const NotificationContext = createContext();

// ─── Notification type → Toast style mapping ───
const TOAST_CONFIG = {
  // License Workflow
  LicenseApproved:  { toastFn: toast.success, icon: '✅' },
  LicenseRejected:  { toastFn: toast.error,   icon: '❌' },
  LicenseUploaded:  { toastFn: toast.info,     icon: '📄' },

  // Car Listing Workflow
  NewCarListing:    { toastFn: toast.info,     icon: '🚗' },
  CarApproved:      { toastFn: toast.success,  icon: '✅' },
  CarRejected:      { toastFn: toast.error,    icon: '❌' },

  // Booking Workflow
  NewBookingRequest:{ toastFn: toast.info,     icon: '📋' },
  BookingAccepted:  { toastFn: toast.success,  icon: '🎉' },
  BookingRejected:  { toastFn: toast.error,    icon: '😔' },

  // Account
  AccountApproved:  { toastFn: toast.success,  icon: '🏢' },
  AccountRejected:  { toastFn: toast.error,    icon: '🚫' },
  AccountPending:   { toastFn: toast.info,     icon: '⏳' },
  NewRegistration:  { toastFn: toast.info,     icon: '👤' },

  // Fallback
  General:          { toastFn: toast.info,      icon: '🔔' },
};

export const NotificationProvider = ({ children }) => {
  const { user } = useContext(AuthContext);
  const [notifications, setNotifications] = useState([]);
  const connectionRef = useRef(null);

  // ─── Extract userId robustly from JWT claims ───
  const getUserId = useCallback(() => {
    if (!user) return null;
    return user.id
      || user.nameid
      || user.sub
      || user['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
      || null;
  }, [user]);

  // ─── 1. Fetch notification history on login/mount ───
  useEffect(() => {
    const userId = getUserId();
    if (userId) {
      fetchHistory();
    } else {
      setNotifications([]);
    }
  }, [getUserId]);

  const fetchHistory = async () => {
    try {
      const res = await notificationService.getMyNotifications();
      if (res.success) {
        setNotifications(res.data || []);
      }
    } catch (err) {
      console.error('NotificationContext: History fetch failed', err);
    }
  };

  // ─── 2. SignalR connection lifecycle ───
  useEffect(() => {
    const userId = getUserId();
    const currentToken = user?.token || localStorage.getItem('token');

    // Guard: Need both userId and token
    if (!userId || !currentToken) {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }
      return;
    }

    // Guard: Don't create duplicate connections
    if (
      connectionRef.current &&
      connectionRef.current.state !== signalR.HubConnectionState.Disconnected
    ) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5003/notificationHub", {
        accessTokenFactory: () => user?.token || localStorage.getItem('token'),
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]) // Escalating retry intervals
      .configureLogging(signalR.LogLevel.Warning) // Production-level logging
      .build();

    // ─── Register the ReceiveNotification handler ───
    connection.on('ReceiveNotification', (payload) => {
      console.log("New SignalR Message:", payload);
      
      const message = payload.message || payload.Message || 'New notification';
      const type = payload.type || payload.Type || 'General';

      // Determine toast style from notification type
      const config = TOAST_CONFIG[type] || TOAST_CONFIG.General;
      config.toastFn(`${config.icon}  ${message}`);

      // Dispatch UI update events for license/account status changes
      if (type === 'LicenseApproved') {
        window.dispatchEvent(
          new CustomEvent('USER_STATUS_UPDATED', {
            detail: { licenseStatus: 'Verified' },
          })
        );
      } else if (type === 'LicenseRejected') {
        window.dispatchEvent(
          new CustomEvent('USER_STATUS_UPDATED', {
            detail: { licenseStatus: 'Rejected' },
          })
        );
      } else if (type === 'LicenseUploaded') {
        window.dispatchEvent(new CustomEvent('LICENSE_UPLOADED'));
      }

      // Play sound for important notifications
      if (type === 'LicenseUploaded' || type === 'NewCarListing') {
        try {
          // A short ping sound or fallback to a relative path
          const audio = new Audio('/ping.mp3'); 
          audio.play().catch(e => console.log('Audio playback prevented by browser:', e));
        } catch (err) { }
      }

      // Prepend to local notification list and keep only the last 10
      setNotifications((prev) => {
        const updated = [
          {
            id: payload.id || payload.Id || Date.now(),
            message,
            type,
            isRead: false,
            createdAt: payload.createdAt || payload.CreatedAt || new Date().toISOString(),
          },
          ...prev,
        ];
        return updated.slice(0, 10); // Notification History limited to 10
      });
    });

    // ─── Start the connection ───
    const startConnection = async () => {
      try {
        await connection.start();
        connectionRef.current = connection;
      } catch (err) {
        console.error('NotificationContext: SignalR connection failed', err);
        // Retry once after 5 seconds (the built-in reconnect handles subsequent retries)
        setTimeout(() => {
          if (getUserId() && localStorage.getItem('token')) {
            startConnection();
          }
        }, 5000);
      }
    };

    startConnection();

    // ─── Cleanup on unmount or user change ───
    return () => {
      if (connection) {
        connection.off('ReceiveNotification');
        if (connection.state !== signalR.HubConnectionState.Disconnected) {
          connection.stop();
        }
        connectionRef.current = null;
      }
    };
  }, [getUserId, user?.token]);

  // ─── 3. API actions ───
  const markAsRead = async (id) => {
    try {
      const res = await notificationService.markAsRead(id);
      if (res.success) {
        setNotifications((prev) =>
          prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
        );
      }
    } catch (err) {
      console.error('NotificationContext: Failed to mark as read', err);
    }
  };

  const markAllAsRead = async () => {
    try {
      const res = await notificationService.markAllAsRead();
      if (res.success) {
        setNotifications((prev) =>
          prev.map((n) => ({ ...n, isRead: true }))
        );
      }
    } catch (err) {
      console.error('NotificationContext: Failed to mark all as read', err);
    }
  };

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  return (
    <NotificationContext.Provider
      value={{ notifications, unreadCount, markAsRead, markAllAsRead, fetchHistory }}
    >
      {children}
    </NotificationContext.Provider>
  );
};
