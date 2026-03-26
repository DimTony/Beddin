import NextAuth, { DefaultSession } from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";
import { NextAuthOptions } from "next-auth";

declare module "next-auth" {
  interface User {
    role: string;
  }
  interface Session {
    user: {
      id: string;
      role: string;
    };
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    id: string;
    role: string;
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
        
        const user = await authenticateUser(
          credentials.email,
          credentials.password,
        );
        if (user) {
          return user;
        } else {
          return null;
        }
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.id = user.id;
        token.role = user.role;
      }
      return token;
    },
    async session({ session, token }) {
      if (token) {
        session.user.id = token.id;
        session.user.role = token.role;
      }
      return session;
    },
  },
  session: {
    strategy: "jwt",
    maxAge: 2 * 60, // 2 minutes
  },
  pages: {
    signIn: "/auth/signin",
    signOut: "/auth/signout",
    error: "/auth/error",
  },
};

async function authenticateUser(email: string, password: string) {
  // Replace this with your actual user authentication logic
  if (email === "buyer@example.com" && password === "password") {
    return { id: "1", name: "Buyer User", email, role: "buyer" } as const;
  } else if (email === "owner@example.com" && password === "password") {
    return { id: "2", name: "Owner User", email, role: "owner" } as const;
  }
  return null;
}

export const GET = NextAuth(options);
export const POST = NextAuth(options);
export const PUT = NextAuth(options);
export const DELETE = NextAuth(options);
export const PATCH = NextAuth(options);
