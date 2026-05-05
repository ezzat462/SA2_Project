import React, { useState, useContext, useRef, useEffect } from 'react';
import { NotificationContext } from '../context/NotificationContext';

// ─── Type → Icon/Color mapping for visual categorization ───
const TYPE_STYLES = {
  LicenseApproved:   { icon: '✅', color: '#198754' },
  LicenseRejected:   { icon: '❌', color: '#dc3545' },
  LicenseUploaded:   { icon: '📄', color: '#0d6efd' },
  NewCarListing:     { icon: '🚗', color: '#6610f2' },
  CarApproved:       { icon: '✅', color: '#198754' },
  CarRejected:       { icon: '❌', color: '#dc3545' },
  NewBookingRequest: { icon: '📋', color: '#fd7e14' },
  BookingAccepted:   { icon: '🎉', color: '#198754' },
  BookingRejected:   { icon: '😔', color: '#dc3545' },
  AccountApproved:   { icon: '🏢', color: '#198754' },
  AccountRejected:   { icon: '🚫', color: '#dc3545' },
  AccountPending:    { icon: '⏳', color: '#fd7e14' },
  NewRegistration:   { icon: '👤', color: '#0d6efd' },
  General:           { icon: '🔔', color: '#6c757d' },
};

export default function NotificationBell() {
  const { notifications, unreadCount, markAsRead, markAllAsRead } = useContext(NotificationContext);
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const getStyle = (type) => TYPE_STYLES[type] || TYPE_STYLES.General;

  const formatTime = (dateStr) => {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now - date;
    const diffMin = Math.floor(diffMs / 60000);

    if (diffMin < 1) return 'Just now';
    if (diffMin < 60) return `${diffMin}m ago`;
    if (diffMin < 1440) return `${Math.floor(diffMin / 60)}h ago`;
    return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
  };

  return (
    <div className="position-relative me-3" ref={dropdownRef}>
      <button
        id="notification-bell-btn"
        className="btn position-relative"
        onClick={() => setIsOpen(!isOpen)}
        style={{
          border: 'none',
          background: 'transparent',
          fontSize: '1.3rem',
          padding: '6px 10px',
          transition: 'transform 0.2s ease',
          transform: isOpen ? 'scale(1.1)' : 'scale(1)',
        }}
        aria-label="Notifications"
      >
        🔔
        {unreadCount > 0 && (
          <span
            className="position-absolute translate-middle badge rounded-pill bg-danger"
            style={{
              fontSize: '0.6rem',
              top: '4px',
              right: '-2px',
              minWidth: '18px',
              animation: 'pulse 2s infinite',
            }}
          >
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
      </button>

      {isOpen && (
        <div
          id="notification-dropdown"
          className="position-absolute shadow-lg rounded-4 p-0 overflow-hidden"
          style={{
            right: 0,
            top: '48px',
            width: '360px',
            zIndex: 1050,
            border: '1px solid rgba(0,0,0,0.08)',
            background: '#ffffff',
            maxHeight: '480px',
            display: 'flex',
            flexDirection: 'column',
          }}
        >
          {/* Header */}
          <div
            className="d-flex justify-content-between align-items-center px-3 py-3"
            style={{
              borderBottom: '1px solid #f0f0f0',
              background: 'linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%)',
            }}
          >
            <h6 className="m-0 fw-bold" style={{ fontSize: '0.95rem' }}>
              Notifications
              {unreadCount > 0 && (
                <span
                  className="ms-2 badge rounded-pill"
                  style={{
                    fontSize: '0.65rem',
                    background: 'linear-gradient(135deg, #6a0dad, #9b30ff)',
                    color: '#fff',
                  }}
                >
                  {unreadCount} new
                </span>
              )}
            </h6>
            {unreadCount > 0 && (
              <button
                id="mark-all-read-btn"
                className="btn btn-link btn-sm p-0 text-decoration-none"
                style={{ fontSize: '0.78rem', color: '#6a0dad', fontWeight: 500 }}
                onClick={markAllAsRead}
              >
                Mark all read
              </button>
            )}
          </div>

          {/* Body */}
          {notifications.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <div style={{ fontSize: '2rem', marginBottom: '8px', opacity: 0.4 }}>🔔</div>
              <small>No notifications yet.</small>
            </div>
          ) : (
            <ul
              className="list-unstyled mb-0"
              style={{ overflowY: 'auto', maxHeight: '400px', flex: 1 }}
            >
              {notifications.map((n) => {
                const style = getStyle(n.type);
                return (
                  <li
                    key={n.id}
                    id={`notification-item-${n.id}`}
                    className="px-3 py-3"
                    onClick={() => !n.isRead && markAsRead(n.id)}
                    style={{
                      cursor: n.isRead ? 'default' : 'pointer',
                      borderBottom: '1px solid #f5f5f5',
                      borderLeft: !n.isRead ? `3px solid ${style.color}` : '3px solid transparent',
                      background: !n.isRead
                        ? 'linear-gradient(90deg, rgba(106,13,173,0.03) 0%, transparent 100%)'
                        : 'transparent',
                      opacity: n.isRead ? 0.65 : 1,
                      transition: 'all 0.2s ease',
                    }}
                  >
                    <div className="d-flex justify-content-between align-items-center mb-1">
                      <span style={{ fontSize: '0.7rem', fontWeight: 600, color: style.color }}>
                        {style.icon} {(n.type || 'General').replace(/([A-Z])/g, ' $1').trim()}
                      </span>
                      <small className="text-muted" style={{ fontSize: '0.68rem' }}>
                        {formatTime(n.createdAt)}
                      </small>
                    </div>
                    <p
                      className="mb-0 text-dark"
                      style={{ fontSize: '0.82rem', lineHeight: '1.45', fontWeight: n.isRead ? 400 : 500 }}
                    >
                      {n.message}
                    </p>
                  </li>
                );
              })}
            </ul>
          )}
        </div>
      )}

      {/* Pulse animation for the badge */}
      <style>{`
        @keyframes pulse {
          0% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.5); }
          70% { box-shadow: 0 0 0 6px rgba(220, 53, 69, 0); }
          100% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0); }
        }
      `}</style>
    </div>
  );
}
