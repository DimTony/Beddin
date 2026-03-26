"use client";

import { SessionProvider, useSession } from "next-auth/react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { signOut } from "next-auth/react";

const INACTIVITY_TIMEOUT = 1 * 60 * 1000;

declare global {
  interface Window {
    inactivityTimeout: ReturnType<typeof setTimeout>;
  }
}

function Guard({ children }: { children: React.ReactNode }) {
  const { data: session, status } = useSession();
  const router = useRouter();

  useEffect(() => {
    if (status === "loading") return;

    if (!session) {
      router.push("/auth/signin");
      return;
    }

    const handleActivity = () => {
      clearTimeout(window.inactivityTimeout);
      window.inactivityTimeout = setTimeout(() => {
        signOut();
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
  }, [session, status, router]);

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
