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
<<<<<<< HEAD
    const circuitLocation = `${raceInfo.Circuit.Location.locality}, ${raceInfo.Circuit.Location.country}`;
    const firstPractice = raceInfo.FirstPractice ? raceInfo.FirstPractice.time : null;
    const secondPractice = raceInfo.SecondPractice ? raceInfo.SecondPractice.time : null;
    const thirdPractice = raceInfo.Sprint ? raceInfo.Sprint.time : null;
    const qualifyTime = raceInfo.Qualifying ? raceInfo.Qualifying.time : null;
    

    

=======
    const circuitLocation = raceInfo.Circuit.Location.country;

    const apiUrlGPName = `https://api.openf1.org/v1/meetings?year=2024&country_name=${raceInfo.Circuit.Location.country}`;

    const responseGPName = await fetch(apiUrlGPName);
    const dataGPName = await responseGPName.json();
    const gpName = dataGPName[0].meeting_official_name;
>>>>>>> 07136863f76b6deebce21e7553c34f541d3c5bb0

    return {
      raceName,
      circuitName,
      circuitLocation,
      raceTimeDate,
<<<<<<< HEAD
      firstPractice,
      secondPractice,
      thirdPractice,
      qualifyTime,
      
=======
      gpName,
>>>>>>> 07136863f76b6deebce21e7553c34f541d3c5bb0
    };
  } catch (error) {
    console.error("Error fetching race data: ", error);
    return null;
  }
}
