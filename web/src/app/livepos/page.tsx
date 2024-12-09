"use client";
// "use strict";

import SidebarLayout from "../ui/sidebar-layout";
import Positions from "../components/positions";
import { useEffect, useState } from "react";
import styles from "./livepos.module.css";
import { timeNow } from "../utils/api/worldtime";

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
  const [liveEvent, setLiveEvent] = useState<boolean>(true);

  const apiurl = "https://api.openf1.org/v1/race_control?session_key=latest";

  console.log(events);
  console.log(liveFlag);

  useEffect(() => {
    // setLiveEvent(true);
    //const timeNow = new Date(new Date().getTime() - 50400000);
    const timeNowc = new Date(new Date().getTime() - 5000);
    console.log(new Date(timeNowc));
    const fetchData = async () => {
      //const data = await fetch(`${apiurl}&date>${timeNow.toISOString()}`);
      const data = await fetch(apiurl);
      const json = await data.json();
      json.sort((a: any, b: any) => {
        return new Date(b.date).getTime() - new Date(a.date).getTime();
      });
      //filters events of category Flag
      let flagEvents = json.filter((event: any) => event.flag !== null);
      setEvents(json);
      setLiveFlag(flagEvents[0].flag);

      const currentTime = await timeNow();
      console.log(currentTime);
      const lastTime = new Date(flagEvents[0].date);
      console.log(lastTime);

      // if (currentTime.getTime() > lastTime.getTime() + 7200000) {
      //   setLiveEvent(false);
      // } else {
      //   setLiveEvent(true);
      // }
    };

    fetchData(); // Initial fetch
    const intervalId = setInterval(fetchData, 10000); // Fetch every 15 seconds

    return () => clearInterval(intervalId); // Cleanup on unmount
  }, []);

  return (
    <SidebarLayout>
      <div>
        {liveEvent ? (
          <div className={styles.container}>
            <div>
              <h1>Live Positions</h1>
              <h2>Live Flag: {liveFlag}</h2>
              <h2>Live Event: {liveEvent == true ? "Yes" : "No"}</h2>
            </div>
            <div>
              <Positions />
            </div>
          </div>
        ) : (
          <div className={styles.container}>
            <h1>No Live event running</h1>
          </div>
        )}
      </div>
    </SidebarLayout>
  );
}
