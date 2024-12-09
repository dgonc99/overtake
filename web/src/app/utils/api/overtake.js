export async function getDriverHeadshot(driver_number) {
  const apiUrl = `http://localhost:8080/api/images/driver/headshot/${driver_number}`;
  try {
    const response = await fetch(apiUrl);
    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error fetching driver image url: ", error);
    return null;
  }
}

export async function getDriverImages(driver_number) {
  const apiUrl = `http://localhost:8080/api/images/all/${driver_number}`;
  try {
    const response = await fetch(apiUrl);
    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error fetching driver image url: ", error);
    return null;
  }
}

export async function getTrackLayoutImage(round_number) {
  const apiUrl = `http://localhost:8080/api/images/track/${round_number}`;
  try {
    const response = await fetch(apiUrl);
    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error fetching track layout image url: ", error);
    return null;
  }
}

export async function getTeamData(constructor_id) {
  const apiUrl = `http://localhost:8080/api/images/team/${constructor_id}`;
  try {
    const response = await fetch(apiUrl);
    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error fetching team meta data: ", error);
    return null;
  }
}

export async function getNext2025() {
  try {
    const raceName = "Formula 1 Australian Grand Prix 2025";
    const circuitName = "Albert Park Grand Prix Circuit";
    const raceDate = "2025-03-16";
    const raceTime = "06:00:00Z";
    const roundNumber = 1;
    const raceTimeDate = new Date(`${raceDate}T${raceTime}`);
    const circuitLocation = "Melbourne";
    let gpName = raceName;

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
