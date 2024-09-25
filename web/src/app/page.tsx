import SidebarLayout from "./ui/sidebar-layout";
import styles from "./home.module.css";

export default function Home() {
  return (
    <SidebarLayout>
      <div className={styles.container}>
        <div className={styles.main}>
          <div className={styles.top}>
            <div className={styles.driver}>
                <div className={styles.title}>OVERTAKE DRIVER OF THE DAY</div>
                <img
                    src="/images/yellowline.png"
                    alt="Yellow Line"
                    style={{ width: "100%" }}
                          />
                 <div className={styles.driverInfo}>
                        <div className={styles.driverDetails}>
                            <div className={styles.driverName}>
                                LEWIS<br />HAMILTON
                            </div>
                        <div className={styles.driverNoAndFlag}>
                            <div className={styles.driverNumber}>44</div>
                            <div className={styles.driverFlag}>
                                <img
                                    src="/images/uk.png"
                                    alt="UK Flag"
                                    style={{
                                        width: "109px",
                                        height: "63px",
                                        borderRadius: "5px",
                                        marginRight: "1rem",
                                    }}
                                />
                            </div>
                        </div>
                    </div>
                    <div className={styles.driverPhoto}>
                        <img
                            src="/images/hamilton.png"
                            style={{
                                width: "240px",
                                height: "240px",
                                display: "block",            
                            }}
                            alt="Lewis Hamilton"
                            className={styles.driverImage}
                        />
                    </div>
                </div>
                <img
                    src="/images/yellowline.png"
                    alt="Yellow Line"
                    style={{ width: "100%" }}
                />
            </div>
            <div className={styles.nextRace}>
              <div className={styles.title}>UPCOMING GRAND PRIX</div>
              <div className={styles.raceInfo}>
                <img
                  src="/images/hungarycircuit.png"
                  style={{ width: "300px", height: "300px", display: "block" }}
                  alt="Hungary Circuit"
                  className={styles.circuitImage}
                />
                <div className={styles.raceDetails}>
                  <div className={styles.raceName}>HUNGARY</div>
                  <div className={styles.raceDate}>
                    JUL 19-21
                    <br />
                    2024
                  </div>
                  <div className={styles.raceFlag}>
                    <img
                      src="/images/hungary.png"
                      alt="Hungary Flag"
                      style={{
                        width: "95px",
                        height: "55px",
                        borderRadius: "15px",
                      }}
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className={styles.bodyBox}>
            <div className={styles.welcome}>
              <h1>WELCOME TO OVERTAKE</h1>
              <p>OVERTAKE is your ultimate F1 companion.</p>
            </div>

            <img
              src="/images/redline.png"
              alt="Red Line"
              style={{ width: "100%" }}
            />

            <div className={styles.section}>
              <h2>RACE LEAGUES</h2>
              <p>
                Create or join a Race League to compete with your friends or
                other fans. Every race weekend, make a set of predictions, and
                win points, trophies, and bragging rights!
              </p>
            </div>
            <div className={styles.section}>
              <h2>FORMULALEARN</h2>
              <p>
                Head over to the FormulaLearn tab to get to know the drivers,
                constructors and circuits. Study historical race data to help
                make your race predictions.
              </p>
            </div>
            <div className={styles.section}>
              <h2>LIVE RACE DATA</h2>
              <p>
                Don’t have time to watch the race? No problem! Visit the Races
                tab to receive live race updates.
              </p>
            </div>

            <img
              src="/images/redline.png"
              alt="Red Line"
              style={{ width: "100%" }}
            />
          </div>
        </div>
      </div>
    </SidebarLayout>
  );
}
