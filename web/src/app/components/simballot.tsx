"use client";

import { useState, useEffect, useContext, useRef } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "./simballot.module.css";
import { getNextRace } from "../utils/api/ergast";
import { timeNow } from "../utils/api/worldtime";
import { getDrivers } from "../utils/api/ergast";
import { Driver } from "../ballot/ballotDriverType";
import BallotDriverSelect from "../components/ballotdriverselect";
import BallotList from "../components/ballotlist";
import BallotLeagueSelect from "../components/ballotleageselect";
import BallotSubmission from "../components/ballotsubmission";
import PodiumDisplay from "../components/ballotpodiumdisplay";
import BallotScore from "../components/ballotscore";
import BallotActualResults from "../components/ballotactualresults";
import Image from 'next/image';
import { useRouter } from 'next/navigation';

export default function SimBallot() {

    const [drivers, setDrivers] = useState<Driver[]>([]);

    const [creatorName, setCreatorName] = useState<string | null>(null);

    const [selectedBox, setSelectedBox] = useState<number | null>(null);
    const [isEditing, setIsEditing] = useState(false);

    const [gridPredictions, setGridPredictions] = useState<(string | null)[]>(Array(10).fill(null));

    const [submissionSuccess, setSubmissionSuccess] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [loadingDrivers, setLoadingDrivers] = useState(true);

    const router = useRouter();

    // Driver fetch
    useEffect(() => {

        async function fetchDrivers() {

            try {
                const driverData = await getDrivers();
                await new Promise((resolve) => setTimeout(resolve, 1000));
                if (driverData) setDrivers(driverData);
            } catch (error) {
                setError("Failed to fetch drivers");
            } finally {
                setLoadingDrivers(false);
            }

        }

        fetchDrivers();

    }, []);

    const handleSubmit = async () => {

        if (gridPredictions.includes(null)) {
            return;
        }

        if (!creatorName) {
            return;
        }

        const response = await fetch("http://localhost:8080/api/sim/create", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                DriverPredictions: gridPredictions,
                Username: creatorName,
            }),
        });

        if (response.status !== 200) {
            console.error(`Non-successful status code: ${response.status}`);
        } else {
            return;
        }
    };

    const handleDriverClick = (driverId: string) => {

        if (selectedBox !== null) {

            // Map driverId to driver's full name
            const selectedDriver = drivers.find(driver => driver.driverId === driverId);
            const driverName = selectedDriver ? `${selectedDriver.givenName} ${selectedDriver.familyName}` : "";

            // Prevent duplicate by checking if the driver's name is already in gridPredictions
            if (!gridPredictions.includes(driverName)) {

                const updatedPredictions = [...gridPredictions];
                updatedPredictions[selectedBox] = driverName;
                setGridPredictions(updatedPredictions);

                // Automatically select the next empty position
                const nextAvailablePosition = updatedPredictions.findIndex((driver) => driver === null);
                setSelectedBox(nextAvailablePosition !== -1 ? nextAvailablePosition : null);

            } else {
                console.log(`Driver ${driverName} is already selected in the ballot.`);
            }

        }

    };

    const handlePositionSelect = (position: number) => {
        setSelectedBox(position);
    };

    const getLoadingImagePath = () => {
        return `/images/loading.svg`;
    };

    return (
        <div className={styles.pageContainer}>

            {/* Left Column: Name Entry and Ballot List */}

            <div className={styles.leftColumn}>

                <input className={styles.inputBox} placeholder="Enter your first and last name" onChange={(e) => setCreatorName(e.target.value)}></input>
                <BallotList
                    onDriverSelect={handlePositionSelect}
                    gridPredictions={gridPredictions}
                    selectedBox={selectedBox}
                    submissionSuccess={submissionSuccess}
                />
                <div className={styles.submissionContainer}>
                    <button
                        onClick={handleSubmit}
                        className={styles.submitButton}
                        disabled={!creatorName || gridPredictions.includes(null)}
                    >
                        Submit Ballot
                    </button>
                </div>
            </div>

            {/* Right Column: Driver Select */}

            <div className={styles.rightColumn}>

                <div className={styles.podiumAndReturn}>

                    <div className={styles.podiumDisplayContainer}>
                        <PodiumDisplay
                            drivers={drivers}
                            gridPredictions={gridPredictions}
                        />
                    </div>

                </div>

                {loadingDrivers ? (
                    <div className={styles.loadingImageContainer}>
                        <Image
                            src={getLoadingImagePath()}
                            alt={'Loading Drivers...'}
                            className={styles.loadingImage}
                            width={90}
                            height={90}
                        />
                    </div>
                ) : error ? (
                    <p>{error}</p>
                ) : (
                    <BallotDriverSelect
                        drivers={drivers}
                        onDriverClick={handleDriverClick}
                        gridPredictions={gridPredictions}
                    />
                )}

            </div>

        </div>
    );

}