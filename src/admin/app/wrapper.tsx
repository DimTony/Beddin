"use client";

import { SessionProvider, useSession } from "next-auth/react";
import { useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import { signOut } from "next-auth/react";
import { setAccessToken, logout } from "./_services/api";

const INACTIVITY_TIMEOUT = 30 * 60 * 1000; // 30 minutes of inactivity will trigger sign out

declare global {
  interface Window {
    inactivityTimeout: ReturnType<typeof setTimeout>;
  }
}

function Guard({ children }: { children: React.ReactNode }) {
  const { data: session, status } = useSession();
  const router = useRouter();
  const pathname = usePathname();
  const isAuthPage = pathname.startsWith("/auth/");

  useEffect(() => {
    if (status === "loading") return;

    if (!session) {
      setAccessToken(null);
      if (!isAuthPage) {
        router.push("/auth/signin");
      }
      return;
    }

    // Hydrate the API module with the accessToken from the session
    setAccessToken(
      (session as unknown as { accessToken?: string }).accessToken ?? null,
    );

    const handleActivity = () => {
      clearTimeout(window.inactivityTimeout);
      window.inactivityTimeout = setTimeout(async () => {
        try {
          await logout();
          signOut();
        } catch {
          signOut();
        }
      }, INACTIVITY_TIMEOUT);
    };

    window.addEventListener("mousemove", handleActivity);
    window.addEventListener("keydown", handleActivity);

    handleActivity();

    return () => {
      clearTimeout(window.inactivityTimeout);
      window.removeEventListener("mousemove", handleActivity);
      window.removeEventListener("keydown", handleActivity);
    };
  }, [session, status, router, isAuthPage]);

  if (status === "loading") return null;

  return <>{children}</>;
}

export default function Wrapper({ children }: { children: React.ReactNode }) {
  return (
    <SessionProvider>
      <Guard>{children}</Guard>
    </SessionProvider>
  );
}
