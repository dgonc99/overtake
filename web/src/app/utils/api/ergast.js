export async function getNextRace() {
  const apiUrl = "https://api.jolpi.ca/ergast/f1/current/next.json";

  try {
    const response = await fetch(apiUrl);
    const data = await response.json();

    const raceInfo = data.MRData.RaceTable.Races[0];

    const raceName = raceInfo.raceName;
    const circuitName = raceInfo.Circuit.circuitName;
    const raceDate = raceInfo.date;
    const raceTime = raceInfo.time;
    const roundNumber = raceInfo.round;
    const raceTimeDate = new Date(`${raceDate}T${raceTime}`);
    const circuitLocation = raceInfo.Circuit.Location.locality;
    let gpName = raceName;
    try {
      const apiUrlGPName = `https://api.openf1.org/v1/meetings?year=2024&location=${raceInfo.Circuit.Location.country}`;
      const responseGPName = await fetch(apiUrlGPName);
      const dataGPName = await responseGPName.json();
      gpName = dataGPName[0].meeting_official_name;
    } catch (error) {
      console.error("Error fetching GP name: ", error);
    }

    return {
      raceName,
      circuitName,
      circuitLocation,
      raceTimeDate,
      gpName,
      roundNumber,
    };
  } catch (error) {
    console.error("Error fetching race data: ", error);
    return null;
  }
}

export async function getRaceResults(season, round) {
  const apiUrl = `https://ergast.com/api/f1/${season}/${round}/results.json`;

  try {
    const response = await fetch(apiUrl);
    const data = await response.json();

    const raceInfo = data.MRData.RaceTable.Races[0];
    const raceName = raceInfo.raceName;
    const raceResults = raceInfo.Results;
    const season = raceInfo.season;
    const round = raceInfo.round;

    return {
      raceName,
      raceResults,
      season,
      round,
    };
  } catch (error) {
    console.error("Error fetching race data: ", error);
    return null;
  }
}

export async function getPrevRace() {
  const apiUrl = "https://ergast.com/api/f1/current/last/results.json";

  try {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error("Failed to fetch race results");
    }

    const data = await response.json();
    const raceResults = data.MRData.RaceTable.Races[0].Results.slice(0, 10).map(
      (result) => `${result.Driver.givenName} ${result.Driver.familyName}`
    );

    console.log("Fetched previous race results:", raceResults);
    return raceResults;
  } catch (error) {
    console.error("Error fetching previous race results:", error);
    return [];
  }
}

export async function getSeasonRounds(season) {
  const apiUrl = `https://ergast.com/api/f1/${season}.json`;

  try {
    const response = await fetch(apiUrl);
    const data = await response.json();

    const seasonRounds = data.MRData.RaceTable.Races.map(
      (race) => race.raceName
    );
    return seasonRounds;
  } catch (error) {
    console.error("Error fetching season rounds: ", error);
    return null;
  }
}

export async function getDrivers() {
  const apiUrl = "https://ergast.com/api/f1/current/last/drivers.json";

  try {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error("Failed to fetch drivers");
    }

    const data = await response.json();
    const ergastDrivers = data.MRData.DriverTable.Drivers.map((driver) => ({
      driverId: driver.driverId,
      permanentNumber: driver.permanentNumber,
      code: driver.code,
      givenName: driver.givenName,
      familyName: driver.familyName,
      fullName: `${driver.givenName} ${driver.familyName}`,
      dateOfBirth: driver.dateOfBirth,
      nationality: driver.nationality,
      url: driver.url,
    }));

    return ergastDrivers;
  } catch (error) {
    console.error("Error fetching drivers data: ", error);
    return null;
  }
}

export async function getConstructors() {
  const apiUrl = "https://ergast.com/api/f1/current/constructors.json";

  try {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error("Failed to fetch constructors");
    }

    const data = await response.json();
    const ergastConstructors = data.MRData.ConstructorTable.Constructors.map(
      (constructor) => ({
        constructorId: constructor.constructorId,
        url: constructor.url,
        name: constructor.name,
        nationality: constructor.nationality,
      })
    );

    return ergastConstructors;
  } catch (error) {
    console.error("Error fetching constructors data: ", error);
    return null;
  }
}

export async function getCircuits() {
  const apiUrl = "https://ergast.com/api/f1/current.json";

  try {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error("Failed to fetch circuits");
    }

    const data = await response.json();
    const ergastCircuits = data.MRData.RaceTable.Races.map((race) => ({
      circuitId: race.Circuit.circuitId,
      url: race.Circuit.url,
      circuitName: race.Circuit.circuitName,
      location: {
        lat: race.Circuit.Location.lat,
        long: race.Circuit.Location.long,
        locality: race.Circuit.Location.locality,
        country: race.Circuit.Location.country,
      },
    }));

    return ergastCircuits;
  } catch (error) {
    console.error("Error fetching circuits data: ", error);
    return null;
  }
}

export async function overtakerOfTheRace() {
  const apiUrl = "https://ergast.com/api/f1/current/last/results.json";

  try {
    const response = await fetch(apiUrl);
    if (!response.ok) {
      throw new Error("Failed to fetch");
    }
    const data = await response.json();
    const overtakes = data.MRData.RaceTable.Races[0].Results.map((result) => ({
      driverId: result.number,
      firstName: result.Driver.givenName,
      lastName: result.Driver.familyName,
      finishPosition: result.position,
      startingPosition: result.grid,
      overtakes: result.grid - result.position,
    }));
    overtakes.sort((a, b) => b.overtakes - a.overtakes);
    console.log("Fetched overtakes:", overtakes);
    return overtakes[0];
  } catch (error) {
    console.error("Error fetching data: ", error);
    return null;
  }
}
