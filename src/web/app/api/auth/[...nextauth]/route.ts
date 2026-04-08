import NextAuth from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";
import { NextAuthOptions } from "next-auth";
import { confirmEmail, login } from "@/app/_services/api";
import { extractApiError } from "@/app/_services/api-error";

declare module "next-auth" {
  interface User {
    role: string;
    accessToken: string;
  }
  interface Session {
    user: {
      id: string;
      role: string;
    };
    accessToken: string;
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    id: string;
    role: string;
    accessToken: string;
  }
}

const options: NextAuthOptions = {
  providers: [
    CredentialsProvider({
      id: "credentials",
      name: "Credentials",
      credentials: {
        email: { label: "Email", type: "text" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials) {
        if (!credentials) return null;

        try {
          const response = await login(credentials);

          if (response) {
            return response;
          } else {
            return null;
          }
        } catch (error) {
          throw new Error(extractApiError(error));
        }
      },
    }),
    CredentialsProvider({
      id: "confirm-email",
      name: "Confirm Email",
      credentials: {
        email: { label: "Email", type: "text" },
        token: { label: "Token", type: "text" },
      },
      async authorize(credentials) {
        if (!credentials) return null;

        try {
          const response = await confirmEmail(credentials);

          if (response) {
            return response;
          } else {
            return null;
          }
        } catch (error) {
          throw new Error(extractApiError(error));
        }
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.id = user.id;
        token.role = user.role;
        token.accessToken = user.accessToken;
      }
      return token;
    },
    async session({ session, token }) {
      if (token) {
        session.user.id = token.id;
        session.user.role = token.role;
        session.accessToken = token.accessToken;
      }
      return session;
    },
  },
  session: {
    strategy: "jwt",
    maxAge: 30 * 60, // 30 minutes of inactivity invalidates client session
  },
  pages: {
    signIn: "/auth/signin",
    signOut: "/auth/signout",
    error: "/auth/error",
  },
};

export const GET = NextAuth(options);
export const POST = NextAuth(options);
export const PUT = NextAuth(options);
export const DELETE = NextAuth(options);
export const PATCH = NextAuth(options);
