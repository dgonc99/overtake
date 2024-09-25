export async function getNextRace() {
  const apiUrl = "https://ergast.com/api/f1/current/next.json";

  try {
    const response = await fetch(apiUrl);
    const data = await response.json();

    const raceInfo = data.MRData.RaceTable.Races[0];

    const raceName = raceInfo.raceName;
    const circuitName = raceInfo.Circuit.circuitName;
    const raceDate = raceInfo.date;
    const raceTime = raceInfo.time;
    const raceTimeDate = new Date(`${raceDate}T${raceTime}`);
    const circuitLocation = `${raceInfo.Circuit.Location.locality}, ${raceInfo.Circuit.Location.country}`;
    const firstPractice = raceInfo.FirstPractice ? raceInfo.FirstPractice.time : null;
    const secondPractice = raceInfo.SecondPractice ? raceInfo.SecondPractice.time : null;
    const thirdPractice = raceInfo.Sprint ? raceInfo.Sprint.time : null;
    const qualifyTime = raceInfo.Qualifying ? raceInfo.Qualifying.time : null;
    

    


    return {
      raceName,
      circuitName,
      circuitLocation,
      raceTimeDate,
      firstPractice,
      secondPractice,
      thirdPractice,
      qualifyTime,
      
    };
  } catch (error) {
    console.error("Error fetching race data: ", error);
    return null;
  }
}
