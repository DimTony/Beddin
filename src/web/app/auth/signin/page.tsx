"use client";

import { signIn, useSession } from "next-auth/react";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { z } from "zod/v4";
import Image from "next/image";

const signInSchema = z.object({
  email: z.email("Please enter a valid email address"),
  password: z.string().min(1, "Password is required"),
});

type FieldErrors = Partial<Record<"email" | "password", string>>;

const SignIn = () => {
  const { status } = useSession();
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (status === "authenticated") {
      router.replace("/");
    }
  }, [status, router]);

  if (status === "loading" || status === "authenticated") return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFieldErrors({});

    const result = signInSchema.safeParse({ email, password });
    if (!result.success) {
      const errors: FieldErrors = {};
      for (const issue of result.error.issues) {
        const field = issue.path[0] as "email" | "password";
        if (!errors[field]) {
          errors[field] = issue.message;
        }
      }
      setFieldErrors(errors);
      return;
    }

    setIsSubmitting(true);
    const signInResult = await signIn("credentials", {
      redirect: false,
      email: result.data.email,
      password: result.data.password,
    });

    setIsSubmitting(false);

    if (signInResult?.error) {
      toast.error(signInResult.error);
    } else {
      window.location.replace("/");
    }
  };

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
        <div className="flex flex-col items-center gap-6 text-center sm:items-start sm:text-left">
          <h1 className="max-w-xs text-3xl font-semibold leading-10 tracking-tight text-black dark:text-zinc-50">
            Welcome
          </h1>
          <p className="mt-2 text-sm text-zinc-500 dark:text-zinc-400">
            Sign in to your account
          </p>
        </div>

        <form onSubmit={handleSubmit} className="flex flex-col gap-5">
          <div className="flex flex-col gap-1.5">
            <label
              htmlFor="email"
              className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
            >
              Email
            </label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
                if (fieldErrors.email)
                  setFieldErrors((prev) => ({ ...prev, email: undefined }));
              }}
              placeholder="you@example.com"
              className={`w-full rounded-lg border px-3 py-2.5 text-sm text-black outline-none transition-colors placeholder:text-zinc-400 focus:ring-2 focus:ring-black/10 dark:bg-zinc-800 dark:text-zinc-50 dark:placeholder:text-zinc-500 dark:focus:ring-white/20 ${
                fieldErrors.email
                  ? "border-red-500 focus:ring-red-500/20"
                  : "border-zinc-300 dark:border-zinc-700"
              }`}
            />
            {fieldErrors.email && (
              <p className="text-xs text-red-500">{fieldErrors.email}</p>
            )}
          </div>

          <div className="flex flex-col gap-1.5">
            <label
              htmlFor="password"
              className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
            >
              Password
            </label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value);
                if (fieldErrors.password)
                  setFieldErrors((prev) => ({ ...prev, password: undefined }));
              }}
              placeholder="••••••••"
              className={`w-full rounded-lg border px-3 py-2.5 text-sm text-black outline-none transition-colors placeholder:text-zinc-400 focus:ring-2 focus:ring-black/10 dark:bg-zinc-800 dark:text-zinc-50 dark:placeholder:text-zinc-500 dark:focus:ring-white/20 ${
                fieldErrors.password
                  ? "border-red-500 focus:ring-red-500/20"
                  : "border-zinc-300 dark:border-zinc-700"
              }`}
            />
            {fieldErrors.password && (
              <p className="text-xs text-red-500">{fieldErrors.password}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="mt-2 flex h-11 w-full items-center justify-center rounded-lg bg-black text-sm font-medium text-white transition-colors hover:bg-zinc-800 disabled:cursor-not-allowed disabled:opacity-50 dark:bg-white dark:text-black dark:hover:bg-zinc-200"
          >
            {isSubmitting ? "Signing in..." : "Sign In"}
          </button>
        </form>

        <div>
          <p className="mt-6 text-center text-sm text-zinc-500 dark:text-zinc-400">
            Don&apos;t have an account?{" "}
            <a
              href="/auth/register"
              className="text-black dark:text-white hover:underline"
            >
              Sign up
            </a>
          </p>
        </div>
      </main>
    </div>
  );
};

export default SignIn;
