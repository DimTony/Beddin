import type { Metadata } from "next";
import "./globals.css";
import Wrapper from "./wrapper";

export const metadata: Metadata = {
  title: "Beddin Admin",
  description: "All-in-one solution",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`h-full antialiased`}>
      <body className="min-h-full flex flex-col">
        <Wrapper>{children}</Wrapper>
      </body>
    </html>
  );
}
