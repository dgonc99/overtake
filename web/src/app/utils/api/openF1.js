"use client";

import { useState, useEffect } from "react";

export default function OpenF1() {
  const [intervalData, setIntervalData] = useState([]);
  //const apiUrl = "https://api.openf1.org/v1/intervals?session_key=latest";
  const apiUrl = "https://api.openf1.org/v1/position?session_key=9599";
  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(apiUrl);
        const data = await response.json();
        const last20Data = data.slice(-20); // Get the last 20 indexes
        setIntervalData(last20Data);
      } catch (error) {
        console.error("Error fetching live data: ", error);
      }
    };

    fetchData(); // Initial fetch
    const intervalId = setInterval(fetchData, 15000); // Fetch every 15 seconds

    return () => clearInterval(intervalId); // Cleanup on unmount
  }, []);

  return (
    <div>
      <h2>Live Data</h2>
      {intervalData.length === 0 && <p>Loading...</p>}
      {intervalData}
      {/* <ul>
        {intervalData.map((item, index) => (
          <li key={index}>
            Driver Number: {item.driver_number}, Gap to Leader:{" "}
            {item.gap_to_leader}
          </li>
        ))}
      </ul> */}
    </div>
  );
}

export async function getDrivers() {
  const apiUrl = "https://api.openf1.org/v1/drivers?session_key=latest";
  const response = await fetch(apiUrl);
  const data = await response.json();
  console.log(data);
  return data;
}
