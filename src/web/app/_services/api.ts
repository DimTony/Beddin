import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { signOut } from "next-auth/react";
import { showApiError } from "./api-error";
import Cookies from "js-cookie";

export const getClientInfo = async () => {
  const userAgent = typeof window !== "undefined" ? navigator.userAgent : "";

  let ipAddress = "";
  try {
    const res = await fetch("https://api.ipify.org?format=json");
    const data = await res.json();
    ipAddress = data.ip ?? "";
  } catch {
    ipAddress = "";
  }

  return { ipAddress, userAgent };
};

const api = axios.create({
  baseURL: "http://localhost:5171",
});

let accessToken: string | null = null;

const PUBLIC_AUTH_ROUTES = [
  "/Authentication/Login",
  "/Authentication/Register",
  "/Authentication/Refresh",
  "/Authentication/Reset",
  "/Authentication/SetPassword",
  "/Authentication/ConfirmEmail",
  "/Authentication/ResendConfirmation",
];

api.interceptors.request.use(
  async (config) => {
    const skipAuth = PUBLIC_AUTH_ROUTES.some((route) => config.url === route);
    if (accessToken && !skipAuth) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }

    // console.log("Request sent:", config);
    return config;
  },
  (error) => Promise.reject(error),
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = getRefreshTokenFromCookie();

        if (!refreshToken) {
          throw new Error("Refresh token not found");
        }

        const { data } = await axios.post(
          "http://localhost:5171/Authentication/Refresh",
          { refreshToken },
        );

        if (typeof window === "undefined") {
          try {
            const { cookies: getServerCookies } = await import("next/headers");
            const cookieStore = await getServerCookies();
            cookieStore.set("refreshToken", data.data.refreshToken, {
              secure: process.env.NODE_ENV === "production",
              sameSite: "strict",
              path: "/",
            });
          } 
          catch (cookieErr) {
            // console.error("[AUTH] Failed to set server cookie:", cookieErr);
            throw cookieErr;
          }
        } else {
          Cookies.set("refreshToken", data.data.refreshToken, {
            secure: process.env.NODE_ENV === "production",
            sameSite: "strict",
            path: "/",
          });
        }

        accessToken = data.data.accessToken;
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;

        return api(originalRequest);
      } catch (refreshError) {
        console.error("[AUTH] Token refresh failed:", refreshError);
        logoutUser();
      }
    }

    if (error.response?.status !== 401) {
      showApiError(error);
    }

    return Promise.reject(error);
  },
);

export default api;

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

export const login = async (credentials: {
  email: string;
  password: string;
}) => {
  
  try {
    const { ipAddress, userAgent } = await getClientInfo();
    
    const payload = {
      ...credentials,
      ipAddress,
      userAgent,
    };
    const { data } = await api.post("/Authentication/Login", payload);

    if (typeof window === "undefined") {
      try {
        const { cookies: getServerCookies } = await import("next/headers");
        const cookieStore = await getServerCookies();
        cookieStore.set("refreshToken", data.data.refreshToken, {
          secure: process.env.NODE_ENV === "production",
          sameSite: "strict",
          path: "/",
        });
      } catch (cookieErr) {
        // console.error(
        //   "[AUTH] Failed to set server cookie:",
        //   cookieErr,
        // );
        throw cookieErr
      }
    } else {
      Cookies.set("refreshToken", data.data.refreshToken, {
        secure: process.env.NODE_ENV === "production",
        sameSite: "strict",
        path: "/",
      });
    }

    accessToken = data.data.accessToken;

    const decoded = jwtDecode<Record<string, string>>(data.data.accessToken);

    if (!decoded) {
      throw new Error("Invalid token");
    }

    return {
      id: decoded.sub || "",
      email: decoded.email || "",
      role: decoded.role || "",
      firstName: decoded.given_name || "",
      lastName: decoded.family_name || "",
      accessToken: data.data.accessToken,
    };
  } catch (error) {
    // console.error("Login failed:", error);
    throw error;
  }
};

export const confirmEmail = async (credentials: {
  email: string;
  token: string;
}) => {
  
  try {
    const { ipAddress, userAgent } = await getClientInfo();

    const payload = {
      ...credentials,
      ipAddress,
      userAgent,
    };
    const { data } = await api.post("/Authentication/ConfirmEmail", payload);

    if (typeof window === "undefined") {
      try {
        const { cookies: getServerCookies } = await import("next/headers");
        const cookieStore = await getServerCookies();
        cookieStore.set("refreshToken", data.data.refreshToken, {
          secure: process.env.NODE_ENV === "production",
          sameSite: "strict",
          path: "/",
        });
      } catch (cookieErr) {
        throw cookieErr;
        // console.error("[AUTH] Failed to set server cookie:", cookieErr);
      }
    } else {
      Cookies.set("refreshToken", data.data.refreshToken, {
        secure: process.env.NODE_ENV === "production",
        sameSite: "strict",
        path: "/",
      });
    }

    accessToken = data.data.accessToken;

    const decoded = jwtDecode<Record<string, string>>(data.data.accessToken);

    if (!decoded) {
      throw new Error("Invalid token");
    }

    return {
      id: decoded.sub || "",
      email: decoded.email || "",
      role: decoded.role || "",
      firstName: decoded.given_name || "",
      lastName: decoded.family_name || "",
      accessToken: data.data.accessToken,
    };
  } catch (error) {
    // console.error("Email confirmation failed:", error);
    throw error;
  }
};

const getRefreshTokenFromCookie = (): string | null => {
  const token = Cookies.get("refreshToken") || null;
  return token;
};

const logoutUser = () => {
  Cookies.remove("refreshToken");
  accessToken = null;
  if (typeof window !== "undefined") {
    signOut({ callbackUrl: "/auth/signin" });
  }
};

export const logout = async (logoutAllSessions = false) => {
  const response = await api.post("/Authentication/Logout", {
    logoutAllSessions,
  });
  logoutUser();
  return response;
};
