import React, { createContext, useContext, useEffect, useState } from "react";
import { authApi } from "../api/authApi";
import i18n from "../i18n";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem("agriledger_user");
    return stored ? JSON.parse(stored) : null;
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (user?.preferredLanguage) i18n.changeLanguage(user.preferredLanguage);
  }, [user]);

  const persistSession = (authResponse) => {
    localStorage.setItem("agriledger_token", authResponse.token);
    localStorage.setItem("agriledger_user", JSON.stringify(authResponse));
    setUser(authResponse);
  };

  const login = async (credentials) => {
    setLoading(true);
    try {
      const { data } = await authApi.login(credentials);
      persistSession(data.data);
      return data.data;
    } finally {
      setLoading(false);
    }
  };

  const register = async (payload) => {
    setLoading(true);
    try {
      const { data } = await authApi.register(payload);
      persistSession(data.data);
      return data.data;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("agriledger_token");
    localStorage.removeItem("agriledger_user");
    setUser(null);
  };

  const updateUser = (updatedFields) => {
    const newUser = { ...user, ...updatedFields };
    localStorage.setItem("agriledger_user", JSON.stringify(newUser));
    setUser(newUser);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, updateUser, isAuthenticated: !!user }}>
      {children}
    </AuthContext.Provider>
  );
}


export const useAuth = () => useContext(AuthContext);
