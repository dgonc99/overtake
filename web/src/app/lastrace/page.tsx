"use client";

import {
  JSXElementConstructor,
  Key,
  ReactElement,
  ReactNode,
  ReactPortal,
  useEffect,
  useState,
} from "react";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "../home.module.css";
import RaceCountdown from "../components/racecountdown";
import { getNextRace } from "../utils/api/ergast";

export default function LastRace() {
  const [raceResults, setRaceResults] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [nextRace, setNextRace] = useState<null | {
    raceName: any;
    circuitName: any;
    circuitLocation: string;
    raceTimeDate: Date;
  }>(null);
  const [imageUrl, setImageUrl] = useState("");

  // Fetch race results when the component mounts
  useEffect(() => {
    const fetchRaceResults = async () => {
      try {
        const response = await fetch(
          "https://ergast.com/api/f1/current/last/results.json"
        );
        if (!response.ok) {
          throw new Error("Failed to fetch race results");
        }
        const data = await response.json();
        setRaceResults(data.MRData.RaceTable.Races[0]); // Set the last race results
        setLoading(false);
      } catch (err: any) {
        setError(err.message);
        setLoading(false);
      }
    };

    let driverId = 14;

    const fetchDriverHeadshots = async () => {
      const response2 = await fetch(
        "http://localhost:8080/api/driver/headshot/" + driverId,
        {
          method: "GET",
        }
      );
      const data2 = await response2.json();
      console.log(data2);
      setImageUrl(data2);
    };

    fetchRaceResults();
    fetchDriverHeadshots();
  }, []);

  useEffect(() => {
    async function fetchData() {
      const race = await getNextRace();
      setNextRace(race);
    }

    fetchData();
  }, []);

  if (loading)
    return (
      <SidebarLayout>
        <p>Loading...</p>
      </SidebarLayout>
    );
  if (error)
    return (
      <SidebarLayout>
        <p>Error: {error}</p>
      </SidebarLayout>
    );

  // Destructure the race data
  const { raceName, Circuit, date, Results } = raceResults ?? ({} as any);
  const { circuitName, Location } = Circuit || {};

  return (
    <SidebarLayout>
      <div className={styles.driversResults}>
        <h1 className={styles.title}>Last Race Results</h1>
        <div>
          <h2>{raceName}</h2>
          {/* Method available on Date objects that converts the date to a string using the local settings of the user's browser */}
          <p>Date: {new Date(date).toLocaleDateString()}</p>
          <p>Circuit: {circuitName}</p>
          <p>
            Location: {Location.locality}, {Location.country}
          </p>

          <h3>Race Results:</h3>
          <table className="table-fixed border-separate border-spacing-1 border border-slate-500 border-spacing-2 p-5">
            <thead>
              <tr>
                <th className="border border-slate-700 w-16 p-1">Position</th>
                <th className="border border-slate-700 w-40 p-1">Driver</th>
                <th className="border border-slate-700 w-40 p-1">
                  Constructor
                </th>
                <th className="border border-slate-700 w-24 p-1">Time</th>
              </tr>
            </thead>
            <tbody>
              {Results.map(
                (result: {
                  Driver: {
                    driverId: Key | null | undefined;
                    givenName: string;

                    familyName: string;
                  };
                  position: number | undefined;
                  Constructor: {
                    name: string | undefined;
                  };
                  Time: { time: any };
                }) => (
                  <tr key={result.Driver.driverId}>
                    <td className="border border-slate-700 px-2">
                      {result.position}
                    </td>
                    <td className="border border-slate-700 px-2">
                      {result.Driver.givenName} {result.Driver.familyName}
                    </td>
                    <td className="border border-slate-700 px-2">
                      {result.Constructor.name}
                    </td>
                    <td className="border border-slate-700 px-2">
                      {result.Time?.time || "+1 Lap"}
                    </td>
                  </tr>
                )
              )}
            </tbody>
          </table>
        </div>
      </div>
      <br></br>
      <div className={styles.driversResults}>
        <h1>Next F1 Race</h1>
        {nextRace ? (
          <div>
            <p>
              <strong>Race: </strong>
              {(nextRace as any)?.raceName}
            </p>
            <p>
              <strong>Circuit: </strong>
              {(nextRace as any)?.circuitName}
            </p>
            <p>
              <strong>Location: </strong>
              {(nextRace as any)?.circuitLocation}
            </p>
            <p>
              <strong>Time & Date: </strong>
              {nextRace.raceTimeDate.toString()}
            </p>
          </div>
        ) : (
          <p>Loading race data...</p>
        )}
      </div>
      <br></br>
      <div className={styles.driversResults}>
        <h1>Time to next race:</h1>
        <RaceCountdown />
        <div>
          <img
            src={imageUrl}
            style={{
              display: "block",
            }}
            alt="Lewis Hamilton"
            className={styles.driverImage}
          />
        </div>
      </div>
    </SidebarLayout>
  );
}
