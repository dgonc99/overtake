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
