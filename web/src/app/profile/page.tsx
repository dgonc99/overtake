"use client";

import { useEffect, useState, useContext } from "react";
import { IdentityContext } from "../lib/context/identity";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "./profile.module.css";

interface UserPoints {
    username: string;
    totalPoints: number;
    pointsThisSeason: number;
    highestLeaguePoints: number;
    highestLeagueName: string;
}

export default function Profile() {
    const [userPoints, setUserPoints] = useState<UserPoints | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [showEdit, setShowEdit] = useState(false);
    const [profileUserId, setProfileUserId] = useState<string | null>(null);
    const identity = useContext(IdentityContext);

    // Fetch profileUserId from sessionStorage on the client side
    useEffect(() => {
        if (typeof window !== "undefined") {
            const storedUserId = sessionStorage.getItem("profileUserId");
            setProfileUserId(storedUserId);
        }
    }, []);

    useEffect(() => {
        const id = identity.accountInfo?.userId;

        const fetchUserPoints = async () => {
            if (!profileUserId) return;

            setIsLoading(true);
            try {
                const response = await fetch(`http://localhost:8080/api/account/pointsInfo?userId=${encodeURIComponent(profileUserId)}`, {
                    method: "GET",
                    headers: {
                        Authorization: `Bearer ${identity.sessionToken}`,
                        "Content-Type": "application/json",
                    },
                });

                if (response.status === 200) {
                    const data = await response.json();
                    setUserPoints(data);
                } else {
                    console.error(`Failed to fetch user points: ${response.status}`);
                }
            } catch (error) {
                console.error("Error fetching user points:", error);
            } finally {
                setIsLoading(false);
            }
        };

        const isUserProfileOwner = () => {
            setShowEdit(id === Number(profileUserId));
        };

        if (profileUserId) {
            fetchUserPoints();
            isUserProfileOwner();
        }
    }, [identity.sessionToken, profileUserId]);

    // Update the profileUserId when sessionStorage changes
    useEffect(() => {
        if (typeof window !== "undefined") {
            const handleStorageChange = () => {
                const newProfileUserId = sessionStorage.getItem("profileUserId");
                setProfileUserId(newProfileUserId);
            };

            window.addEventListener("storage", handleStorageChange);
            return () => {
                window.removeEventListener("storage", handleStorageChange);
            };
        }
    }, []);

    const handleEditClick = async () => {
        try {
            const emailResponse = await fetch("http://localhost:8080/api/account/send-edit-email", {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${identity.sessionToken}`,
                },
            });

            if (emailResponse.ok) {
                alert("Email sent successfully!");
            } else {
                const error = await emailResponse.text();
                alert(`Failed to send the email: ${error}`);
            }
            
        } catch (error) {
            console.error("Error in handleEditClick:", error);
            alert(`An error occurred while sending the email: ${error}`);
        }
    };

    return (
        <SidebarLayout>
            {isLoading ? (
                <p>Loading...</p>
            ) : userPoints ? (
                <div className={styles.container}>
                    <div className={styles.header}>
                        <h1>{userPoints.username}</h1>
                        {showEdit && (
                                <button onClick={handleEditClick}>EDIT</button>
                        )}
                    </div>
                    <h2>Lifetime Points: {userPoints.totalPoints}</h2>
                    <h2>Points this season: {userPoints.pointsThisSeason}</h2>
                    <h2>Highest Individual League Points this season: {userPoints.highestLeaguePoints} ({userPoints.highestLeagueName})</h2>
                </div>
            ) : (
                <p>Failed to load profile data.</p>
            )}
        </SidebarLayout>
    );
}


