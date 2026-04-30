import { useEffect, useState, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'react-toastify';

const useNotifications = () => {
  const [connection, setConnection] = useState(null);
  const [notifications, setNotifications] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_NOTIFICATION_SERVICE_URL}/hubs/notification`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log('Connected to NotificationService');

          connection.on('ReceiveNotification', (notification) => {
            setNotifications((prev) => [notification, ...prev]);
            toast.info(notification.message);
          });
        })
        .catch((e) => console.log('Connection failed: ', e));
    }

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection]);

  const markAsRead = useCallback((id) => {
    // Optionally call an API to mark as read in the database
    setNotifications((prev) =>
      prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
    );
  }, []);

  return { notifications, markAsRead, connected: !!connection };
};

export default useNotifications;
