"use client";
// "use strict";

import SidebarLayout from "../ui/sidebar-layout";
import Positions from "../components/positions";
import { useEffect, useState } from "react";
import styles from "../home.module.css";

export default function LivePos() {
  interface Events {
    date: string;
    category: string;
    lap_number: number;
    message: string;
    driver: string;
    scope: string;
    sector: string;
  }

  const [events, setEvents] = useState<Events>();
  const [liveFlag, setLiveFlag] = useState<string>("");
  const [lapNumber, setLapNumber] = useState<number>(0);

  const apiurl = "https://api.openf1.org/v1/race_control?session_key=latest";

  console.log(events);
  console.log(liveFlag);

  useEffect(() => {
    //const timeNow = new Date(new Date().getTime() - 50400000);
    const timeNow = new Date(new Date().getTime() - 5000);
    console.log(new Date(timeNow));
    const fetchData = async () => {
      //const data = await fetch(`${apiurl}&date>${timeNow.toISOString()}`);
      const data = await fetch(apiurl);
      const json = await data.json();
      json.sort((a: any, b: any) => {
        return new Date(b.date).getTime() - new Date(a.date).getTime();
      });
      //filters events of category Flag
      let flagEvents = json.filter((event: any) => event.flag !== null);
      let lapNumber = json.filter((event: any) => event.lap_number !== null);
      setEvents(json);
      setLiveFlag(flagEvents[0].flag);
      setLapNumber(lapNumber[0].lap_number ?? 0);
    };

    fetchData(); // Initial fetch
    const intervalId = setInterval(fetchData, 10000); // Fetch every 15 seconds

    return () => clearInterval(intervalId); // Cleanup on unmount
  }, []);

  return (
    <SidebarLayout>
      <div className={styles.driversResults}>
        <h1>Live Positions</h1>
        <h2>Live Flag: {liveFlag}</h2>
        <h2>Lap Number: {lapNumber}</h2>
      </div>
      <div>
        <Positions />
      </div>
    </SidebarLayout>
  );
}
