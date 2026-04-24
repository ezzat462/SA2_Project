import { createContext, useState, useEffect, useContext } from "react";
import { jwtDecode } from "jwt-decode"; // make sure jwt-decode is installed

export const AuthContext = createContext();

export default function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check for token on initial load
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decodedUser = jwtDecode(token);
        
        // ASP.NET Core often puts roles in a specific claim URI. 
        // We'll extract it to a simple 'role' property for ease of use.
        const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        const role = decodedUser[roleClaim] || decodedUser.role;

        setUser({ ...decodedUser, role, token });
      } catch (error) {
        console.error("Invalid token:", error);
        localStorage.removeItem("token");
      }
    }

    const handleStatusUpdate = (e) => {
      if (user) {
        setUser(prev => ({ ...prev, ...e.detail }));
      }
    };

    window.addEventListener('USER_STATUS_UPDATED', handleStatusUpdate);
    setLoading(false);

    return () => {
      window.removeEventListener('USER_STATUS_UPDATED', handleStatusUpdate);
    };
  }, [user?.id]);

  const login = (token) => {
    localStorage.setItem("token", token);
    const decodedUser = jwtDecode(token);
    
    const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    const role = decodedUser[roleClaim] || decodedUser.role;

    setUser({ ...decodedUser, role, token });
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

// Custom hook helper
export const useAuth = () => {
  return useContext(AuthContext);
};