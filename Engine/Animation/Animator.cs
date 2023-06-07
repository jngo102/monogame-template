using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.Animation
{
    /// <summary>
    /// Manages the animation of a sprite.
    /// </summary>
    internal class Animator
    {
        /// <summary>
        /// A list of all the animations in the animator.
        /// </summary>
        private readonly List<Animation> _animations;

        /// <summary>
        /// The current animation being played.
        /// </summary>
        private Animation _currentAnimation;
        
        private Animator(List<Animation> animations)
        {
            _animations = animations;
        }

        /// <summary>
        /// Play an animation.
        /// </summary>
        /// <param name="animationName">The name of the animation to play.</param>
        public void Play(string animationName)
        {
            if (animationName == _currentAnimation.Name) return;

            var animationToPlay = _animations.FirstOrDefault(animation => animation.Name == animationName);
            if (animationToPlay != null)
            {
                _currentAnimation = animationToPlay;
                animationToPlay.Play();
            }
            
            Debug.WriteLine($"Animation {animationName} does not exist.");
        }

        /// <summary>
        /// Stop the current animation.
        /// </summary>
        public void Stop() => _currentAnimation.Stop();
    }
}
