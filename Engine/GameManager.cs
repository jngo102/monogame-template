using Microsoft.Xna.Framework;
using Engine.Input;
using Engine.Save;

namespace Engine
{
    /// <summary>
    /// Handles the overall game state.
    /// </summary>
    public static class GameManager
    {
        /// <summary>
        /// Initialize the game manager.
        /// </summary>
        public static void Initialize()
        {
            InputManager.Initialize();
            SaveDataManager.Initialize();
        }

        /// <summary>
        /// Update the game manager.
        /// </summary>
        /// <param name="gameTime">Time data for the current frame.</param>
        public static void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
        }
    }
}
