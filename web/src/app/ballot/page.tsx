"use client";

import { useState, useEffect } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "./ballot.module.css";

export default function Top10GridPrediction() {
    const [selectedBox, setSelectedBox] = useState<number | null>(null); // Tracks selected box
    const [gridPredictions, setGridPredictions] = useState<(string | null)[]>(Array(10).fill(null)); // Stores the top 10 driver predictions
    const [availableDrivers, setAvailableDrivers] = useState<string[]>([]); // List of drivers
    const [actualResults, setActualResults] = useState<string[]>([]); // List of actual top 10 results
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [submitText, setSubmitText] = useState<string>("SUBMIT BALLOT");

    useEffect(() => {
        // Fetch drivers from the Ergast API
        const fetchDrivers = async () => {
            try {
                const response = await fetch("https://ergast.com/api/f1/current/last/drivers.json");
                if (!response.ok) {
                    throw new Error("Failed to fetch drivers");
                }
                const data = await response.json();
                const drivers = data.MRData.DriverTable.Drivers.map(
                    (driver: { givenName: string; familyName: string }) =>
                        `${driver.givenName} ${driver.familyName}`
                );
                setAvailableDrivers(drivers);
                setLoading(false);
            } catch (err: any) {
                setError(err.message);
                setLoading(false);
            }
        };

        fetchDrivers();
    }, []);

    const handleBoxClick = (index: number) => {
        setSelectedBox(index); // Highlight the clicked box
    };

    const handleDriverClick = (driver: string) => {
        if (selectedBox !== null) {
            const updatedPredictions = [...gridPredictions];
            updatedPredictions[selectedBox] = driver;

            // Mark the driver as selected without removing it from the list
            setGridPredictions(updatedPredictions);
            setSelectedBox(null); // Reset the selected box after the driver is placed
        }
    };

    const isDriverSelected = (driver: string) => {
        return gridPredictions.includes(driver);
    };

    const handleSubmit = () => {
        if (gridPredictions.includes(null)) {
            setSubmitText("INVALID BALLOT, TRY AGAIN");
        } else {
            // Fetch actual race results and compare with the predictions
            const fetchResults = async () => {
                try {
                    const response = await fetch("https://ergast.com/api/f1/current/last/results.json");
                    if (!response.ok) {
                        throw new Error("Failed to fetch race results");
                    }
                    const data = await response.json();
                    const results = data.MRData.RaceTable.Races[0].Results.slice(0, 10).map(
                        (result: { Driver: { givenName: string; familyName: string } }) =>
                            `${result.Driver.givenName} ${result.Driver.familyName}`
                    );
                    setActualResults(results);
                } catch (err: any) {
                    setError(err.message);
                }
            };

            fetchResults();
            setSubmitText("TRY AGAIN");
        }
    };

    const getBoxColor = (predictedDriver: string | null, actualPosition: number) => {
        const predictedPosition = gridPredictions.indexOf(predictedDriver);
        if (predictedPosition === actualPosition) return styles.correct;
        if (Math.abs(predictedPosition - actualPosition) === 1) return styles.nearMiss1;
        if (Math.abs(predictedPosition - actualPosition) === 2) return styles.nearMiss2;
        return "";
    };

    const handleTryAgain = () => {
        // Reset everything to the initial state
        setGridPredictions(Array(10).fill(null));
        setActualResults([]);
        setSubmitText("SUBMIT BALLOT");
        setSelectedBox(null);
    };

    if (loading) return <SidebarLayout><p>Loading drivers...</p></SidebarLayout>;
    if (error) return <SidebarLayout><p>Error: {error}</p></SidebarLayout>;

    return (
        <SidebarLayout>
            <div className={styles.container}>
                {/* Left side: Top 10 positions */}
                <div className={styles.ballot}>
                    <h1>Top 10 Grid Prediction</h1>
                    <br />
                    {gridPredictions.map((driver, index) => (
                        <div
                            key={index}
                            className={`${styles.ballotBox} ${selectedBox === index ? styles.selected : ""} ${driver ? styles.filledBox : ""}`}
                            onClick={() => handleBoxClick(index)}
                        >
                            {index + 1}. {driver || "Select Driver"}
                        </div>
                    ))}
                    <button onClick={submitText === "TRY AGAIN" ? handleTryAgain : handleSubmit} className={styles.submitButton}>
                        {submitText}
                    </button>
                </div>

                {/* Right side: Actual or predicted results */}
                <div className={styles.driverList}>
                    <h2>{actualResults.length > 0 ? "Actual Results" : "Available Drivers"}</h2>
                    <br />
                    <div className={styles.driverGrid}>
                        {actualResults.length === 0
                            ? availableDrivers.map((driver, index) => (
                                <li
                                    key={index}
                                    className={isDriverSelected(driver) ? styles.crossedOut : ""}
                                    onClick={() => handleDriverClick(driver)}
                                >
                                    {driver}
                                </li>
                            ))
                            : actualResults.map((driver, index) => (
                                <div
                                    key={index}
                                    className={`${styles.actualResultsBox} ${getBoxColor(driver, index)}`}
                                >
                                    {index + 1}. {driver}
                                </div>
                            ))}
                    </div>
                </div>
            </div>
        </SidebarLayout>
    );
}
