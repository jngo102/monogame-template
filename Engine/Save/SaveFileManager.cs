using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Engine.Save
{
    /// <summary>
    /// Handles saving and loading the contents of save data from disk.
    /// </summary>
    internal class SaveFileManager
    {
        private string _dataDirPath;
        private string _dataFileName;

        public SaveFileManager(string dirPath, string fileName)
        {
            _dataDirPath = dirPath;
            _dataFileName = fileName;
        }

        /// <summary>
        /// Load the data of a given profile from disk.
        /// </summary>
        /// <param name="profileId">The ID of the profile whose data is being loaded.</param>
        /// <returns></returns>
        public SaveData Load(string profileId)
        {
            var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
            SaveData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    using var stream = new FileStream(fullPath, FileMode.Open);
                    using var reader = new StreamReader(stream);
                    var loadedJson = reader.ReadToEnd();

                    loadedData = JsonConvert.DeserializeObject<SaveData>(loadedJson);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error occurred while trying to load data from file: {fullPath}\n{e}");
                    throw;
                }
            }

            return loadedData;
        }

        /// <summary>
        /// Save the data of a given profile to disk.
        /// </summary>
        /// <param name="data">The data to save.</param>
        /// <param name="profileId">The ID of the profile whose data is being saved.</param>
        public void Save(SaveData data, string profileId)
        {
            var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                var dataJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                using var stream = new FileStream(fullPath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.Write(dataJson);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to save data to {fullPath}: {e}");
                throw;
            }
        }

        /// <summary>
        /// Delete a profile.
        /// </summary>
        /// <param name="profileId">The ID of the profile to delete.</param>
        public void Delete(string profileId)
        {
            if (profileId == null) return;

            var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
            try
            {
                if (File.Exists(fullPath))
                {
                    Directory.Delete(Path.GetDirectoryName(fullPath)!, true);
                }
                else
                {
                    Debug.WriteLine($"Tried to delete save data, but data was not found at {fullPath}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Tried to delete save file for profile {profileId} at {fullPath}\n{e}");
            }
        }

        /// <summary>
        /// Load all profiles.
        /// </summary>
        /// <returns>A dictionary containing mappings of a profile's ID with its save data.</returns>
        public Dictionary<string, SaveData> LoadProfiles()
        {
            var profileDict = new Dictionary<string, SaveData>();
            var dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
            foreach (var dirInfo in dirInfos)
            {
                var profileId = dirInfo.Name;
                var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
                if (!File.Exists(fullPath))
                {
                    Debug.WriteLine($"Skipping directory {profileId} when loading all profiles because it does not containing save data.");
                    continue;
                }

                var profileData = Load(profileId);
                if (profileData != null)
                {
                    profileDict.Add(profileId, profileData);
                }
                else
                {
                    Debug.WriteLine($"Error occurred when trying to load profile {profileId}");
                }
            }
            
            return profileDict;
        }
    }
}
