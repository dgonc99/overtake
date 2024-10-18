"use client";

import { MouseEvent, useContext, useState } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import { IdentityContext } from "../lib/context/identity";
import { useRouter } from "next/navigation";
import { Session } from "../lib/models/session";
import SignupLoginTabs from "../ui/signup-login-tabs";

export default function Signup() {
  const router = useRouter();

  const identity = useContext(IdentityContext);

  const [firstName, setFirstName] = useState<string>("");
  const [lastName, setLastName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleSubmit = async (e: MouseEvent) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match.");
      return;
    }

    const response = await fetch("http://localhost:8080/api/account/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username,
        firstName,
        lastName,
        email,
        password,
      }),
    });

    if (response.status !== 200) {
        setErrorMessage("Failed to create account. Please try again.");
    } else {
      const session = (await response.json()) as Session;
      identity.setSessionToken(session.token);

      router.push("/");
    }
  };

  return (
    <SidebarLayout>
      <div className="w-full h-full flex flex-col">
        <SignupLoginTabs isLoginActive={false} />
        <form className="flex flex-col rounded-2xl p-4 gap-4 bg-accent grow">
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>First Name:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="text"
              value={firstName}
              onChange={(e) => {
                setFirstName(e.target.value);
              }}
            />
          </div>
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>Last Name:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="text"
              value={lastName}
              onChange={(e) => {
                setLastName(e.target.value);
              }}
            />
          </div>
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>Email Address:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="text"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
              }}
            />
          </div>
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>Username:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="text"
              value={username}
              onChange={(e) => {
                setUsername(e.target.value);
              }}
            />
          </div>
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>Enter Password:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="password"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value);
              }}
            />
          </div>
          <div className="flex flex-col gap-2">
            <p className="text-white">
              <label>Confirm Password:</label>
            </p>
            <input
              className="px-2 py-1 rounded-lg bg-input max-w-sm"
              type="password"
              value={confirmPassword}
              onChange={(e) => {
                setConfirmPassword(e.target.value);
              }}
            />
          </div>
          {errorMessage && <p className="text-red-500">{errorMessage}</p>}
          <div>
            <input
              className="px-4 py-2 bg-primary text-white cursor-pointer"
              onClick={handleSubmit}
              type="submit"
              value="SIGN UP"
            />
          </div>
        </form>
      </div>
    </SidebarLayout>
  );
}
