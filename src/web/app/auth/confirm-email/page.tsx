"use client";

import { useSearchParams, useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";
import Image from "next/image";
import { extractApiError } from "../../_services/api-error";
import { signIn } from "next-auth/react";

type Status = "loading" | "success" | "error";

const ConfirmEmail = () => {
  const searchParams = useSearchParams();
  const router = useRouter();
  const email = searchParams.get("email");
  const token = searchParams.get("token");
  const hasParams = Boolean(email && token);

  const [status, setStatus] = useState<Status>(hasParams ? "loading" : "error");
  const [errorMessage, setErrorMessage] = useState(
    hasParams ? "" : "Missing email or token in the URL.",
  );
  const calledRef = useRef(false);

  useEffect(() => {
    if (calledRef.current || !email || !token) return;
    calledRef.current = true;

    const confirmEmail = async () => {
      try {
        const confirmationResult = await signIn("confirm-email", {
          redirect: false,
          email,
          token,
        });

        // await api.post("/Authentication/ConfirmEmail", { email, token });
        // setStatus("success");

        if (confirmationResult?.error) {
          toast.error(confirmationResult.error);
        } else {
          toast.success("Email confirmed successfully!");
          window.location.replace("/");
        }
      } catch (error) {
        setStatus("error");
        setErrorMessage(extractApiError(error));
      }
    };

    confirmEmail();
  }, [email, token]);

  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex flex-1 w-full max-w-3xl flex-col items-center justify-between py-32 px-16 bg-white dark:bg-black sm:items-start">
        <Image
          className="dark:invert"
          src="/next.svg"
          alt="Next.js logo"
          width={100}
          height={20}
          priority
        />

        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left w-full">
          {status === "loading" && (
            <>
              <h1 className="text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
                Confirming your email...
              </h1>
              <p className="text-sm text-zinc-500 dark:text-zinc-400">
                Please wait while we verify your email address.
              </p>
              <div className="mt-4 h-8 w-8 animate-spin rounded-full border-4 border-zinc-300 border-t-black dark:border-zinc-700 dark:border-t-white" />
            </>
          )}

          {status === "success" && (
            <>
              <h1 className="text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
                Email confirmed!
              </h1>
              <p className="text-sm text-zinc-500 dark:text-zinc-400">
                Your email has been verified. You can now sign in to your
                account.
              </p>
              <button
                onClick={() => router.push("/auth/signin")}
                className="mt-4 flex h-11 w-full max-w-xs items-center justify-center rounded-lg bg-black text-sm font-medium text-white transition-colors hover:bg-zinc-800 dark:bg-white dark:text-black dark:hover:bg-zinc-200"
              >
                Go to Sign In
              </button>
            </>
          )}

          {status === "error" && (
            <>
              <h1 className="text-3xl font-semibold leading-10 tracking-tight text-red-600 dark:text-red-400">
                Confirmation failed
              </h1>
              <p className="text-sm text-zinc-500 dark:text-zinc-400">
                {errorMessage}
              </p>
              <button
                onClick={() => router.push("/auth/signin")}
                className="mt-4 flex h-11 w-full max-w-xs items-center justify-center rounded-lg bg-black text-sm font-medium text-white transition-colors hover:bg-zinc-800 dark:bg-white dark:text-black dark:hover:bg-zinc-200"
              >
                Go to Sign In
              </button>
            </>
          )}
        </div>

        <div />
      </main>
    </div>
  );
};

export default ConfirmEmail;
