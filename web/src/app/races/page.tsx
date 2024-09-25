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
import { getNextRace } from "../utils/api/ergast";



export default function Races() {

  const [nextRace, setNextRace] = useState<null | {
    raceName: any;
    circuitName: any;
    circuitLocation: string;
    raceTimeDate: Date;
    firstPractice: any;
    secondPractice: any;
    thirdPractice: any;
    qualifyTime: any;
  }>(null);


  const [country, setCountry] = useState<string>('');
  

  useEffect(() => {
    async function fetchData() {
      const race = await getNextRace();
      setNextRace(race);
      const countryPart = race?.circuitLocation.split(', ').pop();
                setCountry(countryPart || '');
    }

    fetchData();
  }, []);

  
  return (
    <SidebarLayout>
      <div className={styles.container}>
      <div className={styles.upcomingTop}>
      <div className={styles.upcomingRace}>
          
              <div className={styles.title}>UPCOMING GRAND PRIX</div>
              {nextRace ? ( 
              <div className={styles.raceInfo}>
                
                <img
                  src="/images/usacircuit.png"
                  style={{ width: "300px", height: "300px", display: "block" }}
                  alt="Hungary Circuit"
                  className={styles.circuitImage}
                />
                <div className={styles.raceDetails}>

                  <div className={styles.raceName}>
                    {country}
                  </div>
                  
                 
                  <div className={styles.upcomingRaceDate}>
                   {nextRace.raceTimeDate.toDateString()}
                  </div>
                
                  

              

                  <div className={styles.raceFlag}>
                    <img
                      src="/images/usa.png"
                      alt="USA Flag"
                      style={{
                        width: "195px",
                        height: "155px",
                        borderRadius: "15px",
                      }}
                    />
                  </div>
                </div>
                <div className={styles.practiceInfo}>
            <div>19 | Practice 1 | {nextRace.firstPractice.toString()}</div>
          <div>19 | Practice 2 | {nextRace.secondPractice.toString()}</div>
          <div>19 | Practice 3 | {nextRace.thirdPractice.toString()}</div>
          <div> <img
                    src="/images/yellowline.png"
                    alt="Yellow Line"
                    style={{ width: "100%" }}
                /></div>
          <div>20 | Qualifying | {nextRace.qualifyTime.toString()}</div>
          <div>20 | Race | {nextRace.raceTimeDate.toLocaleTimeString()}</div>
            </div>
              </div>
              ):null }
            </div>
           
            </div>
            <div></div>




            </div>
    </SidebarLayout>
                    );
}

