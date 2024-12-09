"use client";

import SidebarLayout from "../ui/sidebar-layout";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./formulalearn.module.css";
import { getDrivers, getConstructors, getCircuits } from "../utils/api/ergast";
import { Driver, Constructor, Circuit } from "./formulaLearnTypes";
import DriverCard from "../components/driverCard";
import ConstructorCard from "../components/constructorCard";
import CircuitCard from "../components/circuitCard";
import HamsterLoader from "../components/loaders/hamsterloader";

export default function FormulaLearn() {
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [constructors, setConstructors] = useState<Constructor[]>([]);
  const [circuits, setCircuits] = useState<Circuit[]>([]);

  const [currentPage, setCurrentPage] = useState(1);
  const [driversPerPage, setDriversPerPage] = useState(5);
  const cardWidth = 300;

  const [loadingDrivers, setLoadingDrivers] = useState(true);
  const [loadingConstructors, setLoadingConstructors] = useState(true);
  const [loadingCircuits, setLoadingCircuits] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const router = useRouter();

  // Driver fetch
  useEffect(() => {
    async function fetchDrivers() {
      try {
        const driverData = await getDrivers();
        if (driverData) setDrivers(driverData);
      } catch (error) {
        setError("Failed to fetch drivers");
      } finally {
        setLoadingDrivers(false);
      }
    }

    fetchDrivers();
  }, []);

  // Constructor fetch
  useEffect(() => {
    async function fetchConstructors() {
      try {
        const constructorData = await getConstructors();
        if (constructorData) setConstructors(constructorData);
      } catch (error) {
        setError("Failed to fetch constructors");
      } finally {
        setLoadingConstructors(false);
      }
    }

    fetchConstructors();
  }, []);

  // Circuit fetch
  useEffect(() => {
    async function fetchCircuits() {
      try {
        const circuitData = await getCircuits();
        if (circuitData) setCircuits(circuitData);
      } catch (error) {
        setError("Failed to fetch circuits");
      } finally {
        setLoadingCircuits(false);
      }
    }

    fetchCircuits();
  }, []);

  useEffect(() => {
    // Function to calculate the number of drivers per page based on window width
    const calculateDriversPerPage = () => {
      const windowWidth = window.innerWidth;
      const driversThatFit = Math.floor((windowWidth - 400) / cardWidth); // Subtract margins and divide by card width
      setDriversPerPage(driversThatFit);
    };

    // Initial calculation of drivers per page
    calculateDriversPerPage();

    // Listen for window resize events to recalculate the number of drivers that can fit
    window.addEventListener("resize", calculateDriversPerPage);

    // Cleanup listener on unmount
    return () => {
      window.removeEventListener("resize", calculateDriversPerPage);
    };
  }, []);

  // Calculate the index of the drivers to display on the current page
  const indexOfLastDriver = currentPage * driversPerPage;
  const indexOfFirstDriver = indexOfLastDriver - driversPerPage;
  const currentDrivers = drivers.slice(indexOfFirstDriver, indexOfLastDriver);

  // Handle next page
  const handleNextPage = () => {
    if (currentPage < Math.ceil(drivers.length / driversPerPage)) {
      setCurrentPage((prev) => prev + 1);
    } else {
      setCurrentPage(1); // Loop back to the first page
    }
  };

  // Handle previous page
  const handlePreviousPage = () => {
    if (currentPage > 1) {
      setCurrentPage((prev) => prev - 1);
    } else {
      setCurrentPage(Math.ceil(drivers.length / driversPerPage)); // Loop back to the last page
    }
  };

  const handleDriverClick = (permanentNumber: number) => {
    sessionStorage.setItem("selectedDriverNumber", permanentNumber.toString());
    router.push("./driverdetails");
    console.log(`Driver with permanent number, ${permanentNumber}, clicked!`);
  };

  const handleConstructorClick = (constructorId: string) => {
    sessionStorage.setItem("selectedConstructorId", constructorId);
    router.push("./constructordetails");
    console.log(`Constructor with constructor id, ${constructorId}, clicked!`);
  };

  return (
    <SidebarLayout>
      {error && <p className={styles.error}>{error}</p>}

      {/* Drivers Display */}
      <div className={styles.drivers}>
        <div className={styles.header}>
          <h1>DRIVERS</h1>
        </div>

        {drivers.length === 0 ? (
          <HamsterLoader />
        ) : (
          <>
            <div className={styles.driverGridContainer}>
              {/* Prev Button */}
              <button onClick={handlePreviousPage} className={styles.navButton}>
                {"<"}
              </button>

              {/* Drivers Grid */}
              <div className={styles.driverGrid}>
                {currentDrivers.map((driver) => (
                  <DriverCard
                    key={driver.driverId}
                    givenName={driver.givenName}
                    familyName={driver.familyName}
                    permanentNumber={driver.permanentNumber}
                    nationality={driver.nationality}
                    onClick={() => handleDriverClick(driver.permanentNumber)}
                  />
                ))}
              </div>

              {/* Next Button */}
              <button onClick={handleNextPage} className={styles.navButton}>
                {">"}
              </button>
            </div>
          </>
        )}
      </div>

      <div className={styles.constructorsAndCircuitsContainer}>
        {/* Constructors Display */}
        <div className={styles.constructors}>
          <div className={styles.header}>
            <h1>CONSTRUCTORS</h1>
          </div>
          <div className={styles.constructorGrid}>
            {constructors.map((constructor) => (
              <ConstructorCard
                key={constructor.constructorId}
                constructorId={constructor.constructorId}
                name={constructor.name}
                nationality={constructor.nationality}
                onClick={() =>
                  handleConstructorClick(constructor.constructorId)
                }
              />
            ))}
          </div>
        </div>

        {/* Circuits Display */}
        <div className={styles.circuits}>
          <div className={styles.header}>
            <h1>CIRCUITS</h1>
          </div>

          {loadingCircuits ? (
            <HamsterLoader />
          ) : (
            <>
              <div className={styles.circuitsGrid}>
                {circuits.map((circuit, index) => (
                  <CircuitCard
                    key={circuit.circuitId}
                    circuitId={circuit.circuitId}
                    circuitName={circuit.circuitName}
                    location={circuit.location}
                    roundNumber={index + 1}
                  />
                ))}
              </div>
            </>
          )}
        </div>
      </div>
    </SidebarLayout>
  );
}
