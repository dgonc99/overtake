"use client";

import { MouseEvent, useContext, useState } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import { IdentityContext } from "../lib/context/identity";
import { useRouter } from "next/navigation";
import { Session } from "../lib/models/session";
import SignupLoginTabs from "../ui/signup-login-tabs";

export default function Login() {
  const router = useRouter();

  const identity = useContext(IdentityContext);

  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleSubmit = async (e: MouseEvent) => {
    e.preventDefault();

    const response = await fetch("http://10.0.0.152:8080/api/account/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username,
        password,
      }),
    });

    if (response.status !== 200) {
      setErrorMessage("Invalid login credentials. Please try again.");
    } else {
      const session = (await response.json()) as Session;
      identity.setSessionToken(session.token);

      router.push("/");
    }
  };

  return (
    <SidebarLayout>
      <div className="w-full h-full flex flex-col">
        <SignupLoginTabs isLoginActive />
        <form className="flex flex-col rounded-2xl p-4 gap-4 bg-accent grow">
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
              <label>Password:</label>
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
          {errorMessage && <p className="text-red-500">{errorMessage}</p>}{" "}
          {/* Display error message */}
          <div>
            <input
              className="px-4 py-2 bg-primary text-white cursor-pointer"
              onClick={handleSubmit}
              type="submit"
              value="LOGIN"
            />
          </div>
        </form>
      </div>
    </SidebarLayout>
  );
}
