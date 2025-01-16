using UnityEngine;

/// <summary>
/// Author: Nathan Fan
/// Description: Saves specified options data at location utilizing the DataSaver util class
/// </summary>
public sealed class OptionsDataSaver : MonoBehaviour
{
    public static string SaveOptionsFileName = "/optionsSettings.txt";

    /// <summary>
    /// Gets the specified options data from saved filee and converts it to a object
    /// </summary>
    /// <returns>Settings that have been packaged into an object</returns>
    public static SteamSettings GetOptionsData()
    {
        string data = DataSaver<SteamSettings>.GetFileFromLocation(DataSaver<SteamSettings>.saveLocation, SaveOptionsFileName);
        SteamSettings convertedData;

        // No data found
        if (data == null || data == string.Empty)
        {
            // Create new data
            convertedData = new SteamSettings(1, 1, false);
            // Save data there
            SaveOptionsData(convertedData);
        }
        else
        {
            convertedData = JsonUtility.FromJson<SteamSettings>(data);
        }

        return convertedData;
    }

    /// <summary>
    /// Save an object to a location
    /// </summary>
    /// <param name="data">Object to save</param>
    public static void SaveOptionsData(SteamSettings data)
    {
        string convertedData = DataSaver<SteamSettings>.ConvertToJson(data);
        // Check if file exists
        DataSaver<SteamSettings>.EnsureFilePath(DataSaver<SteamSettings>.saveLocation, SaveOptionsFileName);
        DataSaver<SteamSettings>.SaveToFileLocation(DataSaver<SteamSettings>.saveLocation + SaveOptionsFileName, convertedData);
    }
}
