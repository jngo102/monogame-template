using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Animation
{
    /// <summary>
    /// Represents a sprite animation.
    /// </summary>
    internal class Animation
    {
        /// <summary>
        /// An array of all the frames in the animation.
        /// </summary>
        private Texture2D[] _frames;

        /// <summary>
        /// The index of the current frame in the animation.
        /// </summary>
        private int _currentFrameIndex;

        /// <summary>
        /// The number of frames to be displayed per second in the animation.
        /// </summary>
        private float _framesPerSecond;

        /// <summary>
        /// Keeps track of the time elapsed in milliseconds since the last frame change.
        /// </summary>
        private int _frameTimerMs;

        /// <summary>
        /// Whether the animation should loop when reaching the end.
        /// </summary>
        private bool _looping;

        public delegate void AnimationFinish(Animation animation);

        /// <summary>
        /// Raised when the animation finishes playing.
        /// </summary>
        public event AnimationFinish AnimationFinished;

        /// <summary>
        /// The duration of a single frame that plays in the animation in milliseconds.
        /// </summary>
        private float FrameDuration => 1000 / _framesPerSecond;

        /// <summary>
        /// The current frame to display in the animation.
        /// </summary>
        public Texture2D CurrentFrame => _frames[_currentFrameIndex];

        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The number of frames in the animation.
        /// </summary>
        public int NumberOfFrames => _frames.Length;

        /// <summary>
        /// The duration of the animation in seconds.
        /// </summary>
        public float Duration => FrameDuration * NumberOfFrames;

        /// <summary>
        /// Whether the animation is playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        public Animation(string name, Texture2D[] frames, float framesPerSecond, bool looping)
        {
            Name = name;
            _frames = frames;
            _framesPerSecond = framesPerSecond;
            _looping = looping;
        }

        /// <summary>
        /// Play the animation.
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
            _currentFrameIndex = 0;
        }

        /// <summary>
        /// Stop the animation.
        /// </summary>
        public void Stop() => IsPlaying = false;

        /// <summary>
        /// Update the animation.
        /// </summary>
        /// <param name="gameTime">Time data for the current frame.</param>
        public void Update(GameTime gameTime)
        {
            if (!IsPlaying) return;

            _frameTimerMs += gameTime.ElapsedGameTime.Milliseconds;
            
            if (!ShouldIncrementFrame()) return;
            
            _frameTimerMs = 0;
            _currentFrameIndex++;

            if (_currentFrameIndex <= NumberOfFrames) return;

            _currentFrameIndex = 0;
            
            if (_looping) return;
            
            IsPlaying = false;
            AnimationFinished?.Invoke(this);
        }

        /// <summary>
        /// Check whether the current frame index should be incremented.
        /// </summary>
        /// <returns>Whether the current frame index should be incremented.</returns>
        private bool ShouldIncrementFrame() => _frameTimerMs > FrameDuration;
    }
}
