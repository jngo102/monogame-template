using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Engine.Save
{
    /// <summary>
    /// Manages the saving and loading of game data.
    /// </summary>
    internal static class SaveDataManager
    {
        private const string FileName = "data.save";

        private static SaveData _saveData;
        private static SaveFileManager _fileManager;

        private static string _selectedProfileId = "0";
        
        /// <summary>
        /// Initialize the save data manager.
        /// </summary>
        public static void Initialize()
        {
            var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Engine", "Saves");
            _fileManager = new SaveFileManager(savePath, FileName);
            LoadGame();
        }

        /// <summary>
        /// Start a new game with a new save data instance.
        /// </summary>
        public static void NewGame() => _saveData = new SaveData();

        /// <summary>
        /// Load save data.
        /// </summary>
        public static void LoadGame()
        {
            _saveData = _fileManager.Load(_selectedProfileId);

            if (_saveData == null)
            {
                Debug.WriteLine("No save data was found. Creating new save data.");
                NewGame();
            }
        }

        /// <summary>
        /// Save data.
        /// </summary>
        public static void SaveGame()
        {
            
            
            _fileManager.Save(_saveData, _selectedProfileId);
        }

        /// <summary>
        /// Delete a profile.
        /// </summary>
        /// <param name="profileId">The ID of the profile to delete.</param>
        public static void DeleteProfile(string profileId) => _fileManager.Delete(profileId);

        /// <summary>
        /// Fetch all the profiles that exist on disk.
        /// </summary>
        /// <returns>A dictionary containing mappings from a profile's ID with its save data.</returns>
        public static Dictionary<string, SaveData> GetAllProfilesSaveData() => _fileManager.LoadProfiles();
    }
}
