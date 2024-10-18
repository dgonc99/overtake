"use client"

import { IdentityContext } from "../lib/context/identity";
import { useState, useContext, useEffect } from "react";

interface RaceLeagueInfo {
    leagueId: number;
    ownerId: number;
    name: string;
    isPublic: boolean;
}

interface JoinRaceLeagueProps {
    onLeagueJoined: (leagueId: number | null) => void;
}
export default function JoinRaceLeague({ onLeagueJoined }: JoinRaceLeagueProps) {
    const identity = useContext(IdentityContext);
    const [search, setSearch] = useState("");
    const [leagues, setLeagues] = useState<RaceLeagueInfo[]>([]);
    const [selectedLeagueId, setSelectedLeagueId] = useState<number | null>(null);

    useEffect(() => {
        const fetchLeagues = async () => {
            const response = await fetch("http://localhost:8080/api/league/populatepublic", {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${identity.sessionToken}`,
                    "Content-Type": "application/json",
                },
            });

            if (response.status === 200) {
                const data = await response.json();

                console.log(data);
                setLeagues(data);
            } else {
                console.error(`non-successful status code: ${response.status}`)
            }
        };


        if (identity.sessionToken) {
            fetchLeagues();
        }
    }, [identity.sessionToken]);

    const handleLeagueSelect = (leagueId: number) => {
        setSelectedLeagueId(leagueId);
    }

    const handleJoinLeague = async () => {
        if (!selectedLeagueId) return;

        const response = await fetch("http://localhost:8080/api/league/join", {
            method: "POST",
            headers: {
                Authorization: `Bearer ${identity.sessionToken}`,
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ leagueId: selectedLeagueId }),
        });

        if (response.status === 200) {
            const data = await response.json();
            onLeagueJoined(selectedLeagueId);
        } else {
            console.error(`Failed to join league, status code: ${response.status}`)
        }
    };

    return (
        <div style={{ padding: 10 }}>
            {/* Header with search bar */}
            <div>
                <input
                    type="text"
                    placeholder="Search leagues"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)} // Updated to work with HTML input
                    style={{ height: 40, borderColor: 'gray', borderWidth: 1, marginBottom: 10 }}
                />
            </div>

            {/* Scrollable list of leagues */}
            <div style={{ height: '300px', overflowY: 'scroll' }}>
                {leagues.map((league) => (
                    <div
                        key={league.leagueId}
                        onClick={() => handleLeagueSelect(league.leagueId)}
                        style={{ padding: 10, backgroundColor: selectedLeagueId === league.leagueId ? '#ddd' : '#fff', marginBottom: 5, cursor: 'pointer' }}
                    >
                        {league.name}
                    </div>
                ))}
            </div>

            {/* Join League button */}
            <button
                onClick={handleJoinLeague} // Use the prop for handling league joining
                disabled={!selectedLeagueId} // Disable the button if no league is selected
            >
                JOIN LEAGUE
            </button>
        </div>
    );
    
}