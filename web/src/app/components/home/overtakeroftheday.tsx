"use client";

import React from "react";
import styles from "./overtakeroftheday.module.css";
import { overtakerOfTheRace } from "../../utils/api/ergast";
import { useEffect, useState } from "react";
import { getDriverHeadshot } from "@/app/utils/api/overtake";
import HamsterLoader from "../loaders/hamsterloader";

interface Overtaker {
  driverId: string;
  firstName: string;
  lastName: string;
  overtakes: number;
  headshot?: string;
}

export default function OvertakerOfTheDay() {
  const [overtaker, setOvertaker] = useState<Overtaker | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    overtakerOfTheRace().then((data) => {
      setOvertaker(data);
      setLoading(false);
    });
  }, []);

  useEffect(() => {
    if (overtaker?.driverId) {
      getDriverHeadshot(overtaker.driverId).then((data) => {
        setOvertaker((prev) => (prev ? { ...prev, headshot: data } : prev));
      });
    }
  }, [overtaker]);

  return (
    <div className={styles.driver}>
      {overtaker && !loading ? (
        <div>
          <div className={styles.title}>OVERTAKER OF THE RACE</div>
          <img
            src="/images/yellowline.png"
            alt="Yellow Line"
            style={{ width: "100%" }}
          />
          <div className={styles.driverInfo}>
            <div className={styles.driverDetails}>
              <div className={styles.driverName}>
                {overtaker.firstName}
                <br />
                {overtaker.lastName}
              </div>
              <div className={styles.driverNoAndFlag}>
                <div className={styles.driverNumber}>{overtaker.driverId}</div>
                <div className={styles.driverFlag}>
                  <p>
                    <span>Overtakes: </span>
                    {overtaker.overtakes}
                  </p>
                </div>
              </div>
            </div>
            <div className={styles.driverPhoto}>
              <img
                src={overtaker.headshot}
                style={{
                  display: "block",
                }}
                alt={`${overtaker.firstName} ${overtaker.lastName}`}
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
      ) : (
        <HamsterLoader />
      )}
    </div>
  );
}