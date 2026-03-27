import type { Metadata } from "next";
import "./globals.css";
import Wrapper from "./wrapper";
import { Toaster } from "sonner";

export const metadata: Metadata = {
  title: "Beddin",
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
        <Toaster richColors position="top-right" closeButton />
        <Wrapper>{children}</Wrapper>
      </body>
    </html>
  );
}
