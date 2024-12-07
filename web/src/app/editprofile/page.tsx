"use client";

import SidebarLayout from "../ui/sidebar-layout";
import { useEffect, useState, useContext } from "react";
import styles from "./editprofile.module.css";
import { IdentityContext } from "../lib/context/identity";
import { useRouter } from "next/navigation";

export default function EditProfile() {
    const identity = useContext(IdentityContext);
    const router = useRouter();
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await fetch("http://localhost:8080/api/account/update-profile", {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${identity.sessionToken}`,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    firstName: firstName || undefined,
                    lastName: lastName || undefined,
                    username: username || undefined,
                    password: password || undefined,
                }),
            });

            if (response.ok) {
                setMessage("Profile updated successfully!");
                router.push("/");
            } else {
                const error = await response.json();
                setMessage(error.message || "Failed to update profile.");
            }
        } catch (error) {
            console.error("Error updating profile:", error);
            setMessage("An error occurred.");
        }
    };

    return (
        <div className="container">
            <h1>Edit Profile</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>First Name:</label>
                    <input
                        type="text"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                    />
                </div>
                <div>
                    <label>Last Name:</label>
                    <input
                        type="text"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                    />
                </div>
                <div>
                    <label>Username:</label>
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>
                <button type="submit">Update Profile</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
}