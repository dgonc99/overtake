"use client";

import { IdentityContext } from "../lib/context/identity";
import { useState, useContext, useEffect } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "./raceleague.module.css";
import CreateRaceLeague from "../components/createleague";
import JoinRaceLeague from "../components/joinleague";
import UserRaceLeagues from "../components/userraceleagues";
import UserBallot from "../components/userballot";
import LeagueHeader from "../components/leagueheader"
import StyledLine from "../components/styledline";

export default function RaceLeague() {
    const [showCreateLeague, setShowCreateLeague] = useState(false);
    const [showJoinLeague, setShowJoinLeague] = useState(false);
    const [showConfirmation, setShowConfirmation] = useState(false);
    const [confirmationType, setConfirmationType] = useState<"created" | "joined" | null>(null);

    const handleCreateLeagueClick = () => {
        setShowCreateLeague(true);
        setShowJoinLeague(false);
    };

    const handleJoinLeagueClick = () => {
        setShowJoinLeague(true);
        setShowCreateLeague(false);
    }

    const handleCloseModal = () => {
        setShowCreateLeague(false);
        setShowJoinLeague(false);
        setShowConfirmation(false);
        setConfirmationType(null);
    }

    const handleLeagueCreated = () => {
        setShowCreateLeague(false);
        setShowConfirmation(true);
        setConfirmationType("created");
    }

    const handleLeagueJoined = () => {
        setShowJoinLeague(false);
        setShowConfirmation(true);
        setConfirmationType("joined");
    }

    return (
    <SidebarLayout>
            <LeagueHeader onCreateLeagueClick={handleCreateLeagueClick} onJoinLeagueClick={handleJoinLeagueClick} />
        <StyledLine color="yellow" size="thick"/>
            <div className={styles.container}>
                {showCreateLeague? (
                    <div >
                        <CreateRaceLeague onLeagueCreated={handleLeagueCreated}/>
                    </div>
                ) : showJoinLeague ? (
                    <div>
                            <JoinRaceLeague onLeagueJoined={handleLeagueJoined}/>
                    </div>
                ) : showConfirmation && confirmationType === "created" ? (
                    <div>
                        <h2> League Created Successfully!</h2>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            CLOSE
                        </button>
                    </div>
                ) : showConfirmation && confirmationType === "joined" ? (
                    <div>
                        <h2> League Joined Successfully!</h2>
                        <button className={styles.closeButton} onClick={handleCloseModal}>
                            CLOSE
                        </button>
                    </div>
                ) : (
                    <div>
                        <UserRaceLeagues />
                        <UserBallot />
                    </div>
                )}
            </div>
    </SidebarLayout>
  );
} 
