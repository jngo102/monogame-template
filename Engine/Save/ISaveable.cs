namespace Engine.Save
{
    /// <summary>
    /// Interface for objects whose data may be loaded and saved persistently.
    /// </summary>
    internal interface ISaveable
    {
        /// <summary>
        /// Load the object's data from a given save data instance.
        /// </summary>
        /// <param name="saveData">The save data instance to load from.</param>
        public void LoadData(SaveData saveData);

        /// <summary>
        /// Save the object's data to a given save data instance.
        /// </summary>
        /// <param name="saveData">The save data instance to save to.</param>
        public void SaveData(SaveData saveData);
    }
}
