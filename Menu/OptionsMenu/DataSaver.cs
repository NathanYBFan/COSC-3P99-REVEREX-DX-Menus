using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Author: Nathan Fan
/// Description: Saves any object to a local file on the pc
/// </summary>
public sealed class DataSaver<T>
{
    public static string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Reverex";
    public static string savedDirectory = "/Reverex";

    /// <summary>
    /// Convert a object to a json file
    /// </summary>
    /// <param name="data">Object to convert</param>
    /// <returns>Object in json string format</returns>
    public static string ConvertToJson(T data)
    {
        string convertedData = JsonUtility.ToJson(data);
        return convertedData;
    }

    /// <summary>
    /// Convert a json file to an object
    /// </summary>
    /// <param name="data">Json file as string to convert from</param>
    /// <returns>Object that has been converted</returns>
    public static T ConvertFromJson(string data)
    {
        T convertedData = JsonUtility.FromJson<T>(data);
        return convertedData;
    }

    /// <summary>
    /// Gets a json file from a specified file location
    /// </summary>
    /// <param name="fileLocation">The local path to get to a file</param>
    /// <param name="fileName">Actual file name to retrieve</param>
    /// <returns>Retrieved file if it exists</returns>
    public static string GetFileFromLocation(string fileLocation, string fileName)
    {
        Directory.CreateDirectory(fileLocation);
        string data = string.Empty;

        try
        {
            FileStream file = File.Open(fileLocation + fileName, FileMode.OpenOrCreate);
            file.Close();
            data = File.ReadAllText(fileLocation + fileName);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        
        return data;
    }

    /// <summary>
    /// Save a file to a specified location
    /// </summary>
    /// <param name="saveLocationPath">Local path to save file to</param>
    /// <param name="jsonDataToSave">File data to save</param>
    public static void SaveToFileLocation(string saveLocationPath, string jsonDataToSave)
    {  
        StreamWriter writer = new StreamWriter(saveLocationPath, false);

        try
        {
            writer.Write(jsonDataToSave);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        writer.Close();
    }

    /// <summary>
    /// Cheeck to ensure a data bath exists, if not create it
    /// </summary>
    /// <param name="saveLocation">File path to save to</param>
    /// <param name="fileData">File to create and deposit at location</param>
    public static void EnsureFilePath(string saveLocation, string fileData)
    {
        // Create all directory
        System.IO.Directory.CreateDirectory(saveLocation);

        // If file exists
        if (File.Exists(saveLocation + fileData)) return;
        else // Create file
        {
            FileStream fs = File.Create(saveLocation + fileData);
            fs.Close();
        }
    }
}
