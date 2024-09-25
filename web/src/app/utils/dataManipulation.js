//This function has been created using copilot AI
//Propmt: "Create a function that extracts the oldest records for each driver from the data array. The data array contains objects with a driver_number and a date property. The function should return an array of objects, one for each driver, containing the oldest record for that driver. The function should sort the data by date in descending order (latest first) and return the oldest record for each driver."

export default function extractOldestRecords(data) {
  // Parse the date strings into Date objects
  const parsedData = data.map((item) => ({
    ...item,
    date: new Date(item.date),
  }));

  // Sort the data by date in descending order (latest first)
  parsedData.sort((a, b) => b.date - a.date);

  // Create a Map to store the oldest record for each driver
  const oldestRecords = new Map();

  // Iterate through the sorted data
  for (const record of parsedData) {
    const { driver_number } = record;

    // If we haven't seen this driver before, or if this record is older than the stored one
    if (
      !oldestRecords.has(driver_number) ||
      record.date > oldestRecords.get(driver_number).date
    ) {
      oldestRecords.set(driver_number, record);
    }
  }

  console.log(oldestRecords);

  // Convert the Map values to an array and return
  return Array.from(oldestRecords.values());
}
