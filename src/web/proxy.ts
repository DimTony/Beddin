import { withAuth } from "next-auth/middleware";
import { NextResponse } from "next/server";

export default withAuth(
  function proxy(req) {
    const { pathname } = req.nextUrl;
    const token = req.nextauth.token;

    if (pathname === "/auth/signin" && token) {
      return NextResponse.redirect(new URL("/", req.url));
    }

    return NextResponse.next();
  },
  {
    callbacks: {
      authorized({ token, req }) {
        const { pathname } = req.nextUrl;

        if (pathname === "/auth/signin" || pathname === "/auth/register") {
          return true;
        }

        return !!token;
      },
    },
  },
);

export const config = {
  matcher: ["/", "/auth/signin", "/auth/register"],
};
