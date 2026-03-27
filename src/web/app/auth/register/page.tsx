"use client";

import { useSession } from "next-auth/react";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { z } from "zod/v4";
import Image from "next/image";
import api from "@/app/_services/api";
import { extractApiError } from "@/app/_services/api-error";

const registerSchema = z
  .object({
    firstName: z.string().min(1, "First name is required"),
    lastName: z.string().min(1, "Last name is required"),
    email: z.email("Please enter a valid email address"),
    password: z.string().min(8, "Password must be at least 8 characters"),
    confirmPassword: z.string().min(1, "Please confirm your password"),
    role: z.string().uuid("Please select a role"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

type FormFields =
  | "firstName"
  | "lastName"
  | "email"
  | "password"
  | "confirmPassword"
  | "role";
type FieldErrors = Partial<Record<FormFields, string>>;

interface Role {
  id: string;
  name: string;
}

const Register = () => {
  const { status } = useSession();
  const router = useRouter();
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [role, setRole] = useState("");
  const [roles, setRoles] = useState<Role[]>([]);
  const [rolesLoading, setRolesLoading] = useState(true);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (status === "authenticated") {
      router.replace("/");
    }
  }, [status, router]);

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const { data } = await api.get("/Roles");
        setRoles(data.data ?? data);
      } catch {
        toast.error("Failed to load roles");
      } finally {
        setRolesLoading(false);
      }
    };
    fetchRoles();
  }, []);

  if (status === "loading" || status === "authenticated") return null;

  const clearError = (field: FormFields) => {
    if (fieldErrors[field]) {
      setFieldErrors((prev) => ({ ...prev, [field]: undefined }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFieldErrors({});

    const result = registerSchema.safeParse({
      firstName,
      lastName,
      email,
      password,
      confirmPassword,
      role,
    });

    if (!result.success) {
      const errors: FieldErrors = {};
      for (const issue of result.error.issues) {
        const field = issue.path[0] as FormFields;
        if (!errors[field]) {
          errors[field] = issue.message;
        }
      }
      setFieldErrors(errors);
      return;
    }

    setIsSubmitting(true);
    try {
      await api.post("/Authentication/Register", {
        firstName: result.data.firstName,
        lastName: result.data.lastName,
        email: result.data.email,
        password: result.data.password,
        role: result.data.role,
      });
      toast.success(
        "Registration successful! Please check your email to confirm your account.",
      );
      router.push("/auth/signin");
    } catch (error) {
      toast.error(extractApiError(error));
    } finally {
      setIsSubmitting(false);
    }
  };

  const inputClass = (field: FormFields) =>
    `w-full rounded-lg border px-3 py-2.5 text-sm text-black outline-none transition-colors placeholder:text-zinc-400 focus:ring-2 focus:ring-black/10 dark:bg-zinc-800 dark:text-zinc-50 dark:placeholder:text-zinc-500 dark:focus:ring-white/20 ${
      fieldErrors[field]
        ? "border-red-500 focus:ring-red-500/20"
        : "border-zinc-300 dark:border-zinc-700"
    }`;

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
            Create an account
          </h1>
          <p className="mt-2 text-sm text-zinc-500 dark:text-zinc-400">
            Fill in your details to get started
          </p>
        </div>

        <form onSubmit={handleSubmit} className="flex flex-col gap-5 w-full">
          <div className="flex gap-4">
            <div className="flex flex-1 flex-col gap-1.5">
              <label
                htmlFor="firstName"
                className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
              >
                First Name
              </label>
              <input
                type="text"
                id="firstName"
                value={firstName}
                onChange={(e) => {
                  setFirstName(e.target.value);
                  clearError("firstName");
                }}
                placeholder="John"
                className={inputClass("firstName")}
              />
              {fieldErrors.firstName && (
                <p className="text-xs text-red-500">{fieldErrors.firstName}</p>
              )}
            </div>

            <div className="flex flex-1 flex-col gap-1.5">
              <label
                htmlFor="lastName"
                className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
              >
                Last Name
              </label>
              <input
                type="text"
                id="lastName"
                value={lastName}
                onChange={(e) => {
                  setLastName(e.target.value);
                  clearError("lastName");
                }}
                placeholder="Doe"
                className={inputClass("lastName")}
              />
              {fieldErrors.lastName && (
                <p className="text-xs text-red-500">{fieldErrors.lastName}</p>
              )}
            </div>
          </div>

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
                clearError("email");
              }}
              placeholder="you@example.com"
              className={inputClass("email")}
            />
            {fieldErrors.email && (
              <p className="text-xs text-red-500">{fieldErrors.email}</p>
            )}
          </div>

          <div className="flex flex-col gap-1.5">
            <label
              htmlFor="role"
              className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
            >
              Role
            </label>
            <select
              id="role"
              value={role}
              onChange={(e) => {
                setRole(e.target.value);
                clearError("role");
              }}
              disabled={rolesLoading}
              className={inputClass("role")}
            >
              <option value="">
                {rolesLoading ? "Loading roles..." : "Select a role"}
              </option>
              {roles.map((r) => (
                <option key={r.id} value={r.id}>
                  {r.name}
                </option>
              ))}
            </select>
            {fieldErrors.role && (
              <p className="text-xs text-red-500">{fieldErrors.role}</p>
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
                clearError("password");
              }}
              placeholder="••••••••"
              className={inputClass("password")}
            />
            {fieldErrors.password && (
              <p className="text-xs text-red-500">{fieldErrors.password}</p>
            )}
          </div>

          <div className="flex flex-col gap-1.5">
            <label
              htmlFor="confirmPassword"
              className="text-sm font-medium text-zinc-700 dark:text-zinc-300"
            >
              Confirm Password
            </label>
            <input
              type="password"
              id="confirmPassword"
              value={confirmPassword}
              onChange={(e) => {
                setConfirmPassword(e.target.value);
                clearError("confirmPassword");
              }}
              placeholder="••••••••"
              className={inputClass("confirmPassword")}
            />
            {fieldErrors.confirmPassword && (
              <p className="text-xs text-red-500">
                {fieldErrors.confirmPassword}
              </p>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="mt-2 flex h-11 w-full items-center justify-center rounded-lg bg-black text-sm font-medium text-white transition-colors hover:bg-zinc-800 disabled:cursor-not-allowed disabled:opacity-50 dark:bg-white dark:text-black dark:hover:bg-zinc-200"
          >
            {isSubmitting ? "Creating account..." : "Create Account"}
          </button>
        </form>

        <div>
          <p className="mt-6 text-center text-sm text-zinc-500 dark:text-zinc-400">
            Already have an account?{" "}
            <a
              href="/auth/signin"
              className="text-black dark:text-white hover:underline"
            >
              Log in
            </a>
          </p>
        </div>
      </main>
    </div>
  );
};

export default Register;
