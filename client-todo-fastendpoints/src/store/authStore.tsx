import { createContext, useContext, useState, useCallback, type ReactNode } from 'react';
import { authApi } from '../api/auth';
import type { LoginRequest, RegisterRequest } from '../types/auth';

const TOKEN_KEY = 'accessToken';

interface AuthState {
  token: string | null;
  loading: boolean;
  error: string | null;
}

interface AuthContextValue {
  state: AuthState;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [state, setState] = useState<AuthState>({
    token: localStorage.getItem(TOKEN_KEY),
    loading: false,
    error: null,
  });

  const setLoading = () => setState(s => ({ ...s, loading: true, error: null }));
  const setError = (error: string) => setState(s => ({ ...s, loading: false, error }));

  const login = useCallback(async (data: LoginRequest) => {
    setLoading();
    try {
      const res = await authApi.login(data);
      localStorage.setItem(TOKEN_KEY, res.accessToken);
      setState({ token: res.accessToken, loading: false, error: null });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
    }
  }, []);

  const register = useCallback(async (data: RegisterRequest) => {
    setLoading();
    try {
      await authApi.register(data);
      setState(s => ({ ...s, loading: false, error: null }));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed');
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    setState({ token: null, loading: false, error: null });
  }, []);

  return (
    <AuthContext.Provider
      value={{ state, login, register, logout, isAuthenticated: !!state.token }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};
