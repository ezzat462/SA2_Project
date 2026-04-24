import { useContext } from 'react';
import { NotificationContext } from '../context/NotificationContext';

/**
 * Convenience hook — delegates entirely to NotificationContext.
 * Kept for backward compatibility with any components that import useNotifications.
 */
export const useNotifications = () => {
  return useContext(NotificationContext);
};

export default useNotifications;
