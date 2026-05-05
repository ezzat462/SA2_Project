import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';

export default function useFavorites() {
  const { user } = useAuth();
  const storageKey = user?.sub || user?.nameid || user?.id ? `favorites_${user.sub || user.nameid || user.id}` : 'favorites_guest';

  const [favorites, setFavorites] = useState(() => {
    try {
      const stored = localStorage.getItem(storageKey);
      return stored ? JSON.parse(stored) : [];
    } catch { return []; }
  });

  useEffect(() => {
    try {
      localStorage.setItem(storageKey, JSON.stringify(favorites));
    } catch {}
  }, [favorites, storageKey]);

  useEffect(() => {
    try {
      const stored = localStorage.getItem(storageKey);
      setFavorites(stored ? JSON.parse(stored) : []);
    } catch { setFavorites([]); }
  }, [storageKey]);

  const toggleFavorite = (car) => {
    setFavorites(prev => {
      const exists = prev.some(f => f.id === car.id);
      if (exists) return prev.filter(f => f.id !== car.id);
      return [...prev, car];
    });
  };

  const isFavorite = (carId) => favorites.some(f => f.id === carId);

  return { favorites, toggleFavorite, isFavorite };
}
