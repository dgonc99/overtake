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


  return (
    <SidebarLayout>
    <div className={styles.container}>
      <div className={styles.upcomingTop}>
        <div className={styles.upcomingRace}>
          <div className={styles.title}>UPCOMING GRAND PRIX</div>
          
          <div className={styles.raceInfo}>
            <img
                  src="/images/usacircuit.png"
                  style={{ width: "300px", height: "300px", display: "block" }}
                  alt="USA Circuit"
                  className={styles.circuitImage}
                 />
            <div className={styles.driverDetails}>
              <div className={styles.raceName}>
                {nextRace?.firstPractice.toString()}
              </div>
              <div className={styles.upcomingRaceDate}>
                   {nextRace?.raceTimeDate.toDateString()}
              </div>
              <div className={styles.raceFlag}>
                    
                </div>
              <div className={styles.practiceInfo}>
                <div>19 | Practice 1 | {nextRace?.firstPractice.toString()}</div>
                <div>19 | Practice 2 | {nextRace?.secondPractice.toString()}</div>
                <div>19 | Practice 3 | {nextRace?.thirdPractice.toString()}</div>
                <div> <img
                    src="/images/yellowline.png"
                    alt="Yellow Line"
                    style={{ width: "100%" }}
                /></div>
                <div>20 | Qualifying | {nextRace?.qualifyTime.toString()}</div>
                <div>20 | Race | {nextRace?.raceTimeDate.toLocaleTimeString()}</div>
            </div>

            </div>
            
          </div>
       
        </div>
      </div>
       {/* Race cards code */}
      <div className={styles.raceMain}>
        {/*Card One*/}
        <div className={styles.raceCards}>
          
          <div className={styles.raceCardInfo}>
          
            <div>
            <img
                  src="/images/usacircuit.png"
                  style={{ width: "200px", height: "200px", display: "block" }}
                  alt="USA Circuit"
                  className={styles.circuitImage}
                 />
            </div>
            <div className={styles.raceCardDetails}>
              <div className={styles.raceCardFeature1}>GREAT BRITAIN</div>
              <div className={styles.raceCardFeature2}>
                < div><img
                      src="/images/hungary.png"
                      alt="Hungary Flag"
                      style={{
                        width: "95px",
                        height: "55px",
                        borderRadius: "15px",
                        marginLeft:"15px",
                      }}
                    />
                    </div>
                    <div className={styles.raceCardDate}>
                      July 05-07 2024
                    </div>
              </div>
              <div className={styles.raceCardFeature3}>
                      FORMULA 1 QATAR AIRWAYS
                      BRITISH GRAND PRIX 2024
              </div>
            </div>
          </div>
          {/*Podium Finishes*/}
          <div className={styles.podiumOne}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "240px",
                              width: "310px",
                            }}
                        /></div>
          <div className={styles.podiumTwo}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "220px",
                              width: "310px",
                              marginTop: "20px",
                            }}
                        /></div>
          <div className={styles.podiumThree}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "200px",
                              width: "310px",
                              marginTop: "40px",
                            }}
                        /></div>

        </div>
        {/*Card Two*/}
        <div className={styles.raceCards}>
          
          <div className={styles.raceCardInfo}>
          
            <div>
            <img
                  src="/images/usacircuit.png"
                  style={{ width: "200px", height: "200px", display: "block" }}
                  alt="USA Circuit"
                  className={styles.circuitImage}
                 />
            </div>
            <div className={styles.raceCardDetails}>
              <div className={styles.raceCardFeature1}>GREAT BRITAIN</div>
              <div className={styles.raceCardFeature2}>
                < div><img
                      src="/images/hungary.png"
                      alt="Hungary Flag"
                      style={{
                        width: "95px",
                        height: "55px",
                        borderRadius: "15px",
                        marginLeft:"15px",
                      }}
                    />
                    </div>
                    <div className={styles.raceCardDate}>
                      July 05-07 2024
                    </div>
              </div>
              <div className={styles.raceCardFeature3}>
                      FORMULA 1 QATAR AIRWAYS
                      BRITISH GRAND PRIX 2024
              </div>
            </div>
          </div>
          {/*Podium finishes*/}
          <div className={styles.podiumOne}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "240px",
                              width: "310px",
                            }}
                        /></div>
          <div className={styles.podiumTwo}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "220px",
                              width: "310px",
                              marginTop: "20px",
                            }}
                        /></div>
          <div className={styles.podiumThree}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "200px",
                              width: "310px",
                              marginTop: "40px",
                            }}
                        /></div>

        </div>
        {/*Card Three*/}
        <div className={styles.raceCards}>
          
          <div className={styles.raceCardInfo}>
          
            <div>
            <img
                  src="/images/usacircuit.png"
                  style={{ width: "200px", height: "200px", display: "block" }}
                  alt="USA Circuit"
                  className={styles.circuitImage}
                 />
            </div>
            <div className={styles.raceCardDetails}>
              <div className={styles.raceCardFeature1}>GREAT BRITAIN</div>
              <div className={styles.raceCardFeature2}>
                < div><img
                      src="/images/hungary.png"
                      alt="Hungary Flag"
                      style={{
                        width: "95px",
                        height: "55px",
                        borderRadius: "15px",
                        marginLeft:"15px",
                      }}
                    />
                    </div>
                    <div className={styles.raceCardDate}>
                      July 05-07 2024
                    </div>
              </div>
              <div className={styles.raceCardFeature3}>
                      FORMULA 1 QATAR AIRWAYS
                      BRITISH GRAND PRIX 2024
              </div>
            </div>
          </div>
          {/* Podium Finishes*/}
          <div className={styles.podiumOne}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "240px",
                              width: "310px",
                            }}
                        /></div>
          <div className={styles.podiumTwo}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "220px",
                              width: "310px",
                              marginTop: "20px",
                            }}
                        /></div>
          <div className={styles.podiumThree}> <img
                            src="/images/hamilton.png"
                            alt="Lewis Hamilton"
                            style={{
                              height: "200px",
                              width: "310px",
                              marginTop: "40px",
                            }}
                        /></div>

        </div>
      </div>









    </div>
    </SidebarLayout>
  );
}