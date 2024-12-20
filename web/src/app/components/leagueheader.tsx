import styles from "./leagueheader.module.css";
import React from 'react';

interface LeagueHeaderProps {
    onCreateLeagueClick: () => void;
    onJoinLeagueClick: () => void;
    onReturnClick: () => void;
}

const LeagueHeader: React.FC<LeagueHeaderProps> = ({ onCreateLeagueClick, onJoinLeagueClick, onReturnClick }) => {

    return (
        <div className={styles.container}>
            <button className={styles.returnButton} onClick={onReturnClick}>
                {'<'}
            </button>
            <button className={styles.button} onClick={onCreateLeagueClick}>
                CREATE A RACE LEAGUE
            </button>
            <button className={styles.button} onClick={onJoinLeagueClick}>
                JOIN A RACE LEAGUE
            </button>
        </div>
    );
};

export default LeagueHeader;