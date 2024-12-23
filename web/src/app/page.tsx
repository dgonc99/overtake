import SidebarLayout from "./ui/sidebar-layout";
import styles from "./home.module.css";
import RaceCountdown from "./components/home/racecountdown";
import OvertakerOfTheDay from "./components/home/overtakeroftheday";
import StyledLine from "./components/styledline";

export default function Home() {

    return (

        <SidebarLayout>

            <div className={styles.container}>
                <div className={styles.main}>
                    <div className={styles.top}>

                        <OvertakerOfTheDay />

                        <div className={styles.nextRace}>

                            <div className={styles.title}>UPCOMING GRAND PRIX</div>

                            <StyledLine color="yellow" size="overtaker" />
                            
                            <RaceCountdown />

                            <StyledLine color="yellow" size="overtaker" />

                        </div>

                    </div>


                    <div className={styles.bodyBox}>
                        <div className={styles.welcome}>
                            <h1>WELCOME TO OVERTAKE</h1>
                            <p>OVERTAKE is your ultimate F1 companion.</p>
                        </div>

                        <StyledLine color="red" size="thick" />

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

                        <StyledLine color="red" size="thick" />

                    </div>
                </div>
            </div>

        </SidebarLayout>

    );

}
