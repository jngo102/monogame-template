using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Engine.Input
{
    /// <summary>
    /// Manager for keyboard and game pad inputs.
    /// </summary>
    public static class InputManager
    {
        #region Internal Input Binding
        /// <summary>
        /// Represents a binding between an input and a game action.
        /// </summary>
        private class InputBinding
        {
            /// <summary>
            /// A list of game pad controls bound to a certain game action.
            /// </summary>
            public List<GamePadButton> GamePadButtons { get; private set; } = new();
            /// <summary>
            /// A list of keyboard keys bound to a certain game action.
            /// </summary>
            public List<Keys> KeyboardKeys { get; private set; } = new();

            /// <summary>
            /// Bind game pad buttons to <see cref="GameAction"/>
            /// </summary>
            /// <param name="buttons">The game pad buttons to bind.</param>
            public void AddGamePadButtons(params GamePadButton[] buttons)
            {
                foreach (var button in buttons)
                {
                    GamePadButtons.Add(button);
                }
            }

            /// <summary>
            /// Bind keyboard keys to <see cref="GameAction"/>
            /// </summary>
            /// <param name="keys">The keyboard keys to bind.</param>
            public void AddKeyboardKeys(params Keys[] keys)
            {
                foreach (var key in keys)
                {
                    KeyboardKeys.Add(key);
                }
            }

            /// <summary>
            /// Bind a unique keyboard key.
            /// </summary>
            /// <param name="key">The keyboard key to bind to.</param>
            public void BindKeyboardKey(Keys key)
            {
                KeyboardKeys.Clear();
                KeyboardKeys.Add(key);
            }

            /// <summary>
            /// Bind a unique game pad button.
            /// </summary>
            /// <param name="gamePadButton">The game pad button to bind to.</param>
            public void BindGamePadButton(GamePadButton gamePadButton)
            {
                GamePadButtons.Clear();
                GamePadButtons.Add(gamePadButton);
            }
        }
        #endregion

        /// <summary>
        /// The threshold at which an analog input is considered pressed.
        /// </summary>
        private const float AnalogThreshold = 0.5f;
        
        /// <summary>
        /// An array of all input bindings mapped to a game action.
        /// </summary>
        private static InputBinding[] _inputBindings;

        #region Input States
        /// <summary>
        /// The current state of the connected keyboard.
        /// </summary>
        private static KeyboardState _currentKeyboardState;

        /// <summary>
        /// The state of the connected keyboard in the previous frame.
        /// </summary>
        private static KeyboardState _previousKeyboardState;

        /// <summary>
        /// The current state of the connected game pad.
        /// </summary>
        private static GamePadState _currentGamePadState;

        /// <summary>
        /// The state of the connected game pad in the previous frame.
        /// </summary>
        private static GamePadState _previousGamePadState;
        #endregion

        /// <summary>
        /// Initialize the input manager.
        /// </summary>
        internal static void Initialize()
        {
            ResetBindingsToDefaults();
        }

        #region Game Action Bindings
        /// <summary>
        /// Reset all actions to their default bindings.
        /// </summary>
        public static void ResetBindingsToDefaults()
        {
            _inputBindings = new InputBinding[(int)GameAction.TotalActions];

            _inputBindings[(int)GameAction.Up] = new InputBinding();
            _inputBindings[(int)GameAction.Up].AddKeyboardKeys(Keys.W, Keys.Up);
            _inputBindings[(int)GameAction.Up].AddGamePadButtons(GamePadButton.LeftStickUp, GamePadButton.DPadUp);

            _inputBindings[(int)GameAction.Down] = new InputBinding();
            _inputBindings[(int)GameAction.Down].AddKeyboardKeys(Keys.S, Keys.Down);
            _inputBindings[(int)GameAction.Down].AddGamePadButtons(GamePadButton.LeftStickDown, GamePadButton.DPadDown);

            _inputBindings[(int)GameAction.Left] = new InputBinding();
            _inputBindings[(int)GameAction.Left].AddKeyboardKeys(Keys.A, Keys.Left);
            _inputBindings[(int)GameAction.Left].AddGamePadButtons(GamePadButton.LeftStickLeft, GamePadButton.DPadLeft);

            _inputBindings[(int)GameAction.Right] = new InputBinding();
            _inputBindings[(int)GameAction.Right].AddKeyboardKeys(Keys.D, Keys.Right);
            _inputBindings[(int)GameAction.Right].AddGamePadButtons(GamePadButton.LeftStickRight, GamePadButton.DPadRight);

            _inputBindings[(int)GameAction.Jump] = new InputBinding();
            _inputBindings[(int)GameAction.Jump].AddKeyboardKeys(Keys.Space, Keys.Z);
            _inputBindings[(int)GameAction.Jump].AddGamePadButtons(GamePadButton.A);

            _inputBindings[(int)GameAction.Cancel] = new InputBinding();
            _inputBindings[(int)GameAction.Cancel].AddKeyboardKeys(Keys.Escape);
            _inputBindings[(int)GameAction.Cancel].AddGamePadButtons(GamePadButton.Back);
        }

        /// <summary>
        /// Rebind a game action to a different keyboard key.
        /// </summary>
        /// <param name="gameAction">The game action that is being rebound.</param>
        /// <param name="key">The new keyboard key to bind to.</param>
        public static void Rebind(GameAction gameAction, Keys key)
        {
            _inputBindings[(int)gameAction].BindKeyboardKey(key);
        }

        /// <summary>
        /// Rebind a game action to a different keyboard key.
        /// </summary>
        /// <param name="gameAction">The game action that is being rebound.</param>
        /// <param name="gamePadButton">The new game pad button to bind to.</param>
        public static void Rebind(GameAction gameAction, GamePadButton gamePadButton)
        {
            _inputBindings[(int)gameAction].BindGamePadButton(gamePadButton);
        }

        /// <summary>
        /// Check whether a game action is pressed.
        /// </summary>
        /// <param name="action">The game action to check.</param>
        /// <returns>Whether the checked game action is pressed.</returns>
        public static bool IsActionPressed(GameAction action) => IsInputBindingPressed(_inputBindings[(int)action]);

        /// <summary>
        /// Check whether a game action is just pressed.
        /// </summary>
        /// <param name="action">The game action to check.</param>
        /// <returns>Whether the checked game action is just pressed.</returns>
        public static bool IsActionJustPressed(GameAction action) =>
            IsInputBindingJustPressed(_inputBindings[(int)action]);

        /// <summary>
        /// Check whether a game action is just released.
        /// </summary>
        /// <param name="action">The game action to check.</param>
        /// <returns>Whether the checked game action is just released.</returns>
        public static bool IsActionJustReleased(GameAction action) =>
            IsInputBindingJustReleased(_inputBindings[(int)action]);

        /// <summary>
        /// Check whether an input binding is pressed.
        /// </summary>
        /// <param name="inputBinding">The input binding to check.</param>
        /// <returns>Whether the checked input binding is pressed.</returns>
        private static bool IsInputBindingPressed(InputBinding inputBinding)
        {
            foreach (var key in inputBinding.KeyboardKeys)
            {
                if (IsKeyPressed(key))
                {
                    return true;
                }
            }

            foreach (var button in inputBinding.GamePadButtons)
            {
                if (IsGamePadButtonPressed(button))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether an input binding is just pressed.
        /// </summary>
        /// <param name="inputBinding">The input binding to check.</param>
        /// <returns>Whether the checked input binding is just pressed.</returns>
        private static bool IsInputBindingJustPressed(InputBinding inputBinding)
        {
            foreach (var key in inputBinding.KeyboardKeys)
            {
                if (IsKeyJustPressed(key))
                {
                    return true;
                }
            }

            foreach (var button in inputBinding.GamePadButtons)
            {
                if (IsGamePadButtonJustPressed(button))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether an input binding is just released.
        /// </summary>
        /// <param name="inputBinding">The input binding to check.</param>
        /// <returns>Whether the checked input binding is just released.</returns>
        private static bool IsInputBindingJustReleased(InputBinding inputBinding)
        {
            foreach (var key in inputBinding.KeyboardKeys)
            {
                if (IsKeyJustReleased(key))
                {
                    return true;
                }
            }

            foreach (var button in inputBinding.GamePadButtons)
            {
                if (IsGamePadButtonJustReleased(button))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Keyboard Keys
        /// <summary>
        /// Check whether a key is currently being pressed.
        /// </summary>
        /// <param name="key">The key whose pressed state is being checked.</param>
        /// <returns>Whether the checked key is pressed.</returns>
        private static bool IsKeyPressed(Keys key) => _currentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Check whether a key was only just pressed in the current frame.
        /// </summary>
        /// <param name="key">The key whose just pressed state is being checked.</param>
        /// <returns>Whether the checked key was just pressed in the current frame.</returns>
        private static bool IsKeyJustPressed(Keys key) =>
            _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Check whether a key was only just released in the current frame.
        /// </summary>
        /// <param name="key">The key whose just released state is being checked.</param>
        /// <returns>Whether the checked key was just released in the current frame..</returns>
        private static bool IsKeyJustReleased(Keys key) =>
            !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
        #endregion

        #region Game Pad Button Pressed
        /// <summary>
        /// Check whether the A button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the A button is pressed.</returns>
        private static bool IsGamePadAPressed() => _currentGamePadState.Buttons.A == ButtonState.Pressed;

        /// <summary>
        /// Check whether the B button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the B button is pressed.</returns>
        private static bool IsGamePadBPressed() => _currentGamePadState.Buttons.B == ButtonState.Pressed;

        /// <summary>
        /// Check whether the X button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the X button is pressed.</returns>
        private static bool IsGamePadXPressed() => _currentGamePadState.Buttons.X == ButtonState.Pressed;

        /// <summary>
        /// Check whether the Y button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the Y button is pressed.</returns>
        private static bool IsGamePadYPressed() => _currentGamePadState.Buttons.Y == ButtonState.Pressed;

        /// <summary>
        /// Check whether the start button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the start button is pressed.</returns>
        private static bool IsGamePadStartPressed() => _currentGamePadState.Buttons.Start == ButtonState.Pressed;

        /// <summary>
        /// Check whether the Back button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the Back button is pressed.</returns>
        private static bool IsGamePadBackPressed() => _currentGamePadState.Buttons.Back == ButtonState.Pressed;

        /// <summary>
        /// Check whether the left shoulder button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the left shoulder button is pressed.</returns>
        private static bool IsGamePadLeftShoulderPressed() =>
            _currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;

        /// <summary>
        /// Check whether the right shoulder button on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the right shoulder button is pressed.</returns>
        private static bool IsGamePadRightShoulderPressed() =>
            _currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed;

        /// <summary>
        /// Check whether the left trigger on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the left trigger is pressed.</returns>
        private static bool IsGamePadLeftTriggerPressed() => _currentGamePadState.Triggers.Left >= AnalogThreshold;

        /// <summary>
        /// Check whether the right trigger on the game pad is pressed.
        /// </summary>
        /// <returns>Whether the right trigger is pressed.</returns>
        private static bool IsGamePadRightTriggerPressed() => _currentGamePadState.Triggers.Right >= AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is pressed upward.
        /// </summary>
        /// <returns>Whether the left joystick is pressed upward.</returns>
        private static bool IsGamePadLeftStickPressedUp() => _currentGamePadState.ThumbSticks.Left.Y >= AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is pressed downward.
        /// </summary>
        /// <returns>Whether the left joystick is pressed downward.</returns>
        private static bool IsGamePadLeftStickPressedDown() => _currentGamePadState.ThumbSticks.Left.Y <= -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is pressed to the left.
        /// </summary>
        /// <returns>Whether the left joystick is pressed to the left.</returns>
        private static bool IsGamePadLeftStickPressedLeft() => _currentGamePadState.ThumbSticks.Left.X <= -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is pressed to the right.
        /// </summary>
        /// <returns>Whether the left joystick is pressed to the right.</returns>
        private static bool IsGamePadLeftStickPressedRight() => _currentGamePadState.ThumbSticks.Left.X >= AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is pressed upward.
        /// </summary>
        /// <returns>Whether the right joystick is pressed upward.</returns>
        private static bool IsGamePadRightStickPressedUp() => _currentGamePadState.ThumbSticks.Right.Y >= AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is pressed downward.
        /// </summary>
        /// <returns>Whether the right joystick is pressed downward.</returns>
        private static bool IsGamePadRightStickPressedDown() => _currentGamePadState.ThumbSticks.Right.Y <= -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is pressed to the left.
        /// </summary>
        /// <returns>Whether the right joystick is pressed to the left.</returns>
        private static bool IsGamePadRightStickPressedLeft() => _currentGamePadState.ThumbSticks.Right.X <= -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is pressed to the right.
        /// </summary>
        /// <returns>Whether the right joystick is pressed to the right.</returns>
        private static bool IsGamePadRightStickPressedRight() => _currentGamePadState.ThumbSticks.Right.X >= AnalogThreshold;

        /// <summary>
        /// Check whether the up button on the game pad's directional pad is pressed.
        /// </summary>
        /// <returns>Whether the D-pad up button is pressed.</returns>
        private static bool IsGamePadDPadUpPressed() => _currentGamePadState.DPad.Up == ButtonState.Pressed;

        /// <summary>
        /// Check whether the down button on the game pad's directional pad is pressed.
        /// </summary>
        /// <returns>Whether the D-pad down button is pressed.</returns>
        private static bool IsGamePadDPadDownPressed() => _currentGamePadState.DPad.Down == ButtonState.Pressed;

        /// <summary>
        /// Check whether the left button on the game pad's directional pad is pressed.
        /// </summary>
        /// <returns>Whether the D-pad left button is pressed.</returns>
        private static bool IsGamePadDPadLeftPressed() => _currentGamePadState.DPad.Left == ButtonState.Pressed;

        /// <summary>
        /// Check whether the right button on the game pad's directional pad is pressed.
        /// </summary>
        /// <returns>Whether the D-pad right button is pressed.</returns>
        private static bool IsGamePadDPadRightPressed() => _currentGamePadState.DPad.Right == ButtonState.Pressed;

        /// <summary>
        /// Check whether a game pad button is pressed.
        /// </summary>
        /// <param name="gamePadButton">The game pad button to check.</param>
        /// <returns>Whether the checked game pad button is pressed.</returns>
        private static bool IsGamePadButtonPressed(GamePadButton gamePadButton)
        {
            return gamePadButton switch
            {
                GamePadButton.A => IsGamePadAPressed(),
                GamePadButton.B => IsGamePadBPressed(),
                GamePadButton.X => IsGamePadXPressed(),
                GamePadButton.Y => IsGamePadYPressed(),
                GamePadButton.Start => IsGamePadStartPressed(),
                GamePadButton.Back => IsGamePadBackPressed(),
                GamePadButton.LeftShoulder => IsGamePadLeftShoulderPressed(),
                GamePadButton.RightShoulder => IsGamePadRightShoulderPressed(),
                GamePadButton.LeftTrigger => IsGamePadLeftTriggerPressed(),
                GamePadButton.RightTrigger => IsGamePadRightTriggerPressed(),
                GamePadButton.LeftStickUp => IsGamePadLeftStickPressedUp(),
                GamePadButton.LeftStickDown => IsGamePadLeftStickPressedDown(),
                GamePadButton.LeftStickLeft => IsGamePadLeftStickPressedLeft(),
                GamePadButton.LeftStickRight => IsGamePadLeftStickPressedRight(),
                GamePadButton.RightStickUp => IsGamePadRightStickPressedUp(),
                GamePadButton.RightStickDown => IsGamePadRightStickPressedDown(),
                GamePadButton.RightStickLeft => IsGamePadRightStickPressedLeft(),
                GamePadButton.RightStickRight => IsGamePadRightStickPressedRight(),
                GamePadButton.DPadUp => IsGamePadDPadUpPressed(),
                GamePadButton.DPadDown => IsGamePadDPadDownPressed(),
                GamePadButton.DPadLeft => IsGamePadDPadLeftPressed(),
                GamePadButton.DPadRight => IsGamePadDPadRightPressed(),
                _ => false
            };
        }
        #endregion

        #region Game Pad Button Just Pressed

        /// <summary>
        /// Check whether the A button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the A button is just pressed.</returns>
        private static bool IsGamePadAJustPressed() =>
            IsGamePadAPressed() && _previousGamePadState.Buttons.A == ButtonState.Released;

        /// <summary>
        /// Check whether the B button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the B button is just pressed.</returns>
        private static bool IsGamePadBJustPressed() =>
            IsGamePadBPressed() && _previousGamePadState.Buttons.B == ButtonState.Released;

        /// <summary>
        /// Check whether the X button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the X button is just pressed.</returns>
        private static bool IsGamePadXJustPressed() =>
            IsGamePadXPressed() && _previousGamePadState.Buttons.X == ButtonState.Released;

        /// <summary>
        /// Check whether the Y button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the Y button is just pressed.</returns>
        private static bool IsGamePadYJustPressed() =>
            IsGamePadYPressed() && _previousGamePadState.Buttons.Y == ButtonState.Released;

        /// <summary>
        /// Check whether the start button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the start button is just pressed.</returns>
        private static bool IsGamePadStartJustPressed() =>
            IsGamePadStartPressed() && _previousGamePadState.Buttons.Start == ButtonState.Released;

        /// <summary>
        /// Check whether the Back button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the Back button is just pressed.</returns>
        private static bool IsGamePadBackJustPressed() =>
            IsGamePadBackPressed() && _previousGamePadState.Buttons.Start == ButtonState.Released;

        /// <summary>
        /// Check whether the left shoulder button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the left shoulder button is just pressed.</returns>
        private static bool IsGamePadLeftShoulderJustPressed() => IsGamePadLeftShoulderPressed() &&
                                                                  _previousGamePadState.Buttons.LeftShoulder ==
                                                                  ButtonState.Released;

        /// <summary>
        /// Check whether the right shoulder button on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the right shoulder button is just pressed.</returns>
        private static bool IsGamePadRightShoulderJustPressed() => IsGamePadRightShoulderPressed() &&
                                                                   _previousGamePadState.Buttons.RightShoulder ==
                                                                   ButtonState.Released;

        /// <summary>
        /// Check whether the left trigger on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the left trigger is just pressed.</returns>
        private static bool IsGamePadLeftTriggerJustPressed() =>
            IsGamePadLeftTriggerPressed() && _previousGamePadState.Triggers.Left < AnalogThreshold;

        /// <summary>
        /// Check whether the right trigger on the game pad is just pressed.
        /// </summary>
        /// <returns>Whether the right trigger is just pressed.</returns>
        private static bool IsGamePadRightTriggerJustPressed() => IsGamePadRightTriggerPressed() &&
                                                                  _previousGamePadState.Triggers.Right < AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just pressed upward.
        /// </summary>
        /// <returns>Whether the left joystick is just pressed upward.</returns>
        private static bool IsGamePadLeftStickJustPressedUp() => IsGamePadLeftStickPressedUp() &&
                                                                 _previousGamePadState.ThumbSticks.Left.Y <
                                                                 AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just pressed downward.
        /// </summary>
        /// <returns>Whether the left joystick is just pressed downward.</returns>
        private static bool IsGamePadLeftStickJustPressedDown() => IsGamePadLeftStickPressedDown() &&
                                                                   _previousGamePadState.ThumbSticks.Left.Y >
                                                                   -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just pressed to the left.
        /// </summary>
        /// <returns>Whether the left joystick is just pressed to the left.</returns>
        private static bool IsGamePadLeftStickJustPressedLeft() => IsGamePadLeftStickPressedLeft() &&
                                                                   _previousGamePadState.ThumbSticks.Left.X >
                                                                   -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just pressed to the right.
        /// </summary>
        /// <returns>Whether the left joystick is just pressed to the right.</returns>
        private static bool IsGamePadLeftStickJustPressedRight() => IsGamePadLeftStickPressedRight() &&
                                                                    _previousGamePadState.ThumbSticks.Left.X <
                                                                    AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just pressed upward.
        /// </summary>
        /// <returns>Whether the right joystick is just pressed upward.</returns>
        private static bool IsGamePadRightStickJustPressedUp() => IsGamePadRightStickPressedUp() &&
                                                                  _previousGamePadState.ThumbSticks.Right.Y <
                                                                  AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just pressed downward.
        /// </summary>
        /// <returns>Whether the right joystick is just pressed downward.</returns>
        private static bool IsGamePadRightStickJustPressedDown() => IsGamePadRightStickPressedDown() &&
                                                                    _previousGamePadState.ThumbSticks.Right.Y >
                                                                    -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just pressed to the left.
        /// </summary>
        /// <returns>Whether the right joystick is just pressed to the left.</returns>
        private static bool IsGamePadRightStickJustPressedLeft() => IsGamePadRightStickPressedLeft() &&
                                                                    _previousGamePadState.ThumbSticks.Right.X >
                                                                    -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just pressed to the right.
        /// </summary>
        /// <returns>Whether the right joystick is just pressed to the right.</returns>
        private static bool IsGamePadRightStickJustPressedRight() => IsGamePadRightStickPressedRight() &&
                                                                     _previousGamePadState.ThumbSticks.Right.X <
                                                                     AnalogThreshold;

        /// <summary>
        /// Check whether the up button on the game pad's directional pad is just pressed.
        /// </summary>
        /// <returns>Whether the D-pad up button is just pressed.</returns>
        private static bool IsGamePadDPadUpJustPressed() =>
            IsGamePadDPadUpPressed() && _previousGamePadState.DPad.Up == ButtonState.Released;

        /// <summary>
        /// Check whether the down button on the game pad's directional pad is just pressed.
        /// </summary>
        /// <returns>Whether the D-pad down button is just pressed.</returns>
        private static bool IsGamePadDPadDownJustPressed() =>
            IsGamePadDPadDownPressed() && _previousGamePadState.DPad.Down == ButtonState.Released;

        /// <summary>
        /// Check whether the left button on the game pad's directional pad is just pressed.
        /// </summary>
        /// <returns>Whether the D-pad left button is just pressed.</returns>
        private static bool IsGamePadDPadLeftJustPressed() =>
            IsGamePadDPadLeftPressed() && _previousGamePadState.DPad.Left == ButtonState.Released;

        /// <summary>
        /// Check whether the right button on the game pad's directional pad is just pressed.
        /// </summary>
        /// <returns>Whether the D-pad right button is just pressed.</returns>
        private static bool IsGamePadDPadRightJustPressed() =>
            IsGamePadDPadRightPressed() && _previousGamePadState.DPad.Right == ButtonState.Released;

        /// <summary>
        /// Check whether a game pad button is just pressed.
        /// </summary>
        /// <param name="gamePadButton">The game pad button to check.</param>
        /// <returns>Whether the checked game pad button is just pressed.</returns>
        private static bool IsGamePadButtonJustPressed(GamePadButton gamePadButton)
        {
            return gamePadButton switch
            {
                GamePadButton.A => IsGamePadAJustPressed(),
                GamePadButton.B => IsGamePadBJustPressed(),
                GamePadButton.X => IsGamePadXJustPressed(),
                GamePadButton.Y => IsGamePadYJustPressed(),
                GamePadButton.Start => IsGamePadStartJustPressed(),
                GamePadButton.Back => IsGamePadBackJustPressed(),
                GamePadButton.LeftShoulder => IsGamePadLeftShoulderJustPressed(),
                GamePadButton.RightShoulder => IsGamePadRightShoulderJustPressed(),
                GamePadButton.LeftTrigger => IsGamePadLeftTriggerJustPressed(),
                GamePadButton.RightTrigger => IsGamePadRightTriggerJustPressed(),
                GamePadButton.LeftStickUp => IsGamePadLeftStickJustPressedUp(),
                GamePadButton.LeftStickDown => IsGamePadLeftStickJustPressedDown(),
                GamePadButton.LeftStickLeft => IsGamePadLeftStickJustPressedLeft(),
                GamePadButton.LeftStickRight => IsGamePadLeftStickJustPressedRight(),
                GamePadButton.RightStickUp => IsGamePadRightStickJustPressedUp(),
                GamePadButton.RightStickDown => IsGamePadRightStickJustPressedDown(),
                GamePadButton.RightStickLeft => IsGamePadRightStickJustPressedLeft(),
                GamePadButton.RightStickRight => IsGamePadRightStickJustPressedRight(),
                GamePadButton.DPadUp => IsGamePadDPadUpJustPressed(),
                GamePadButton.DPadDown => IsGamePadDPadDownJustPressed(),
                GamePadButton.DPadLeft => IsGamePadDPadLeftJustPressed(),
                GamePadButton.DPadRight => IsGamePadDPadRightJustPressed(),
                _ => false
            };
        }
        #endregion

        #region Game Pad Button Just Released

        /// <summary>
        /// Check whether the A button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the A button is just released.</returns>
        private static bool IsGamePadAJustReleased() => _currentGamePadState.Buttons.A == ButtonState.Released &&
                                                        _previousGamePadState.Buttons.A == ButtonState.Pressed;

        /// <summary>
        /// Check whether the B button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the B button is just released.</returns>
        private static bool IsGamePadBJustReleased() => _currentGamePadState.Buttons.B == ButtonState.Released &&
                                                        _previousGamePadState.Buttons.B == ButtonState.Pressed;

        /// <summary>
        /// Check whether the X button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the X button is just released.</returns>
        private static bool IsGamePadXJustReleased() => _currentGamePadState.Buttons.X == ButtonState.Released &&
                                                        _previousGamePadState.Buttons.X == ButtonState.Pressed;

        /// <summary>
        /// Check whether the Y button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the Y button is just released.</returns>
        private static bool IsGamePadYJustReleased() => _currentGamePadState.Buttons.Y == ButtonState.Released &&
                                                        _previousGamePadState.Buttons.Y == ButtonState.Pressed;

        /// <summary>
        /// Check whether the start button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the start button is just released.</returns>
        private static bool IsGamePadStartJustReleased() => _currentGamePadState.Buttons.Start == ButtonState.Released &&
                                                           _previousGamePadState.Buttons.Start == ButtonState.Pressed;

        /// <summary>
        /// Check whether the Back button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the Back button is just released.</returns>
        private static bool IsGamePadBackJustReleased() => _currentGamePadState.Buttons.Back == ButtonState.Released &&
                                                          _previousGamePadState.Buttons.Start == ButtonState.Pressed;

        /// <summary>
        /// Check whether the left shoulder button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the left shoulder button is just released.</returns>
        private static bool IsGamePadLeftShoulderJustReleased() =>
            _currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed &&
            _previousGamePadState.Buttons.LeftShoulder == ButtonState.Released;

        /// <summary>
        /// Check whether the right shoulder button on the game pad is just released.
        /// </summary>
        /// <returns>Whether the right shoulder button is just released.</returns>
        private static bool IsGamePadRightShoulderJustReleased() =>
            _currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed &&
            _previousGamePadState.Buttons.RightShoulder == ButtonState.Released;

        /// <summary>
        /// Check whether the left trigger on the game pad is just released.
        /// </summary>
        /// <returns>Whether the left trigger is just released.</returns>
        private static bool IsGamePadLeftTriggerJustReleased() => _currentGamePadState.Triggers.Left < AnalogThreshold &&
                                                                  _previousGamePadState.Triggers.Left >= AnalogThreshold;

        /// <summary>
        /// Check whether the right trigger on the game pad is just released.
        /// </summary>
        /// <returns>Whether the right trigger is just released.</returns>
        private static bool IsGamePadRightTriggerJustReleased() =>
            _currentGamePadState.Triggers.Right < AnalogThreshold &&
            _previousGamePadState.Triggers.Right >= AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just released upward.
        /// </summary>
        /// <returns>Whether the left joystick is just released upward.</returns>
        private static bool IsGamePadLeftStickJustReleasedUp() =>
            _currentGamePadState.ThumbSticks.Left.Y < AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Left.Y >= AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just released downward.
        /// </summary>
        /// <returns>Whether the left joystick is just released downward.</returns>
        private static bool IsGamePadLeftStickJustReleasedDown() =>
            _currentGamePadState.ThumbSticks.Left.Y > -AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Left.Y <= -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just released to the left.
        /// </summary>
        /// <returns>Whether the left joystick is just released to the left.</returns>
        private static bool IsGamePadLeftStickJustReleasedLeft() =>
            _currentGamePadState.ThumbSticks.Left.X > -AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Left.X <= -AnalogThreshold;

        /// <summary>
        /// Check whether the left joystick on the game pad is just released to the right.
        /// </summary>
        /// <returns>Whether the left joystick is just released to the right.</returns>
        private static bool IsGamePadLeftStickJustReleasedRight() =>
            _currentGamePadState.ThumbSticks.Left.X < AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Left.X >= AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just released upward.
        /// </summary>
        /// <returns>Whether the right joystick is just released upward.</returns>
        private static bool IsGamePadRightStickJustReleasedUp() =>
            _currentGamePadState.ThumbSticks.Right.Y < AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Right.Y >= AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just released downward.
        /// </summary>
        /// <returns>Whether the right joystick is just released downward.</returns>
        private static bool IsGamePadRightStickJustReleasedDown() =>
            _currentGamePadState.ThumbSticks.Right.Y > -AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Right.Y <= -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just released to the left.
        /// </summary>
        /// <returns>Whether the right joystick is just released to the left.</returns>
        private static bool IsGamePadRightStickJustReleasedLeft() =>
            _currentGamePadState.ThumbSticks.Right.X > -AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Right.X <= -AnalogThreshold;

        /// <summary>
        /// Check whether the right joystick on the game pad is just released to the right.
        /// </summary>
        /// <returns>Whether the right joystick is just released to the right.</returns>
        private static bool IsGamePadRightStickJustReleasedRight() =>
            _currentGamePadState.ThumbSticks.Right.X < AnalogThreshold &&
            _previousGamePadState.ThumbSticks.Right.X >= AnalogThreshold;

        /// <summary>
        /// Check whether the up button on the game pad's directional pad is just released.
        /// </summary>
        /// <returns>Whether the D-pad up button is just released.</returns>
        private static bool IsGamePadDPadUpJustReleased() => _currentGamePadState.DPad.Up == ButtonState.Released &&
                                                             _previousGamePadState.DPad.Up == ButtonState.Pressed;

        /// <summary>
        /// Check whether the down button on the game pad's directional pad is pressed.
        /// </summary>
        /// <returns>Whether the D-pad down button is pressed.</returns>
        private static bool IsGamePadDPadDownJustReleased() => _currentGamePadState.DPad.Down == ButtonState.Released &&
                                                               _previousGamePadState.DPad.Down == ButtonState.Pressed;

        /// <summary>
        /// Check whether the left button on the game pad's directional pad is just released.
        /// </summary>
        /// <returns>Whether the D-pad left button is just released.</returns>
        private static bool IsGamePadDPadLeftJustReleased() => _currentGamePadState.DPad.Left == ButtonState.Released &&
                                                               _previousGamePadState.DPad.Left == ButtonState.Pressed;

        /// <summary>
        /// Check whether the right button on the game pad's directional pad is just released.
        /// </summary>
        /// <returns>Whether the D-pad right button is just released.</returns>
        private static bool IsGamePadDPadRightJustReleased() => _currentGamePadState.DPad.Right == ButtonState.Released &&
                                                                _previousGamePadState.DPad.Right == ButtonState.Pressed;

        /// <summary>
        /// Check whether a game pad button is just released.
        /// </summary>
        /// <param name="gamePadButton">The game pad button to check.</param>
        /// <returns>Whether the checked game pad button is just released.</returns>
        private static bool IsGamePadButtonJustReleased(GamePadButton gamePadButton)
        {
            return gamePadButton switch
            {
                GamePadButton.A => IsGamePadAJustReleased(),
                GamePadButton.B => IsGamePadBJustReleased(),
                GamePadButton.X => IsGamePadXJustReleased(),
                GamePadButton.Y => IsGamePadYJustReleased(),
                GamePadButton.Start => IsGamePadStartJustReleased(),
                GamePadButton.Back => IsGamePadBackJustReleased(),
                GamePadButton.LeftShoulder => IsGamePadLeftShoulderJustReleased(),
                GamePadButton.RightShoulder => IsGamePadRightShoulderJustReleased(),
                GamePadButton.LeftTrigger => IsGamePadLeftTriggerJustReleased(),
                GamePadButton.RightTrigger => IsGamePadRightTriggerJustReleased(),
                GamePadButton.LeftStickUp => IsGamePadLeftStickJustReleasedUp(),
                GamePadButton.LeftStickDown => IsGamePadLeftStickJustReleasedDown(),
                GamePadButton.LeftStickLeft => IsGamePadLeftStickJustReleasedLeft(),
                GamePadButton.LeftStickRight => IsGamePadLeftStickJustReleasedRight(),
                GamePadButton.RightStickUp => IsGamePadRightStickJustReleasedUp(),
                GamePadButton.RightStickDown => IsGamePadRightStickJustReleasedDown(),
                GamePadButton.RightStickLeft => IsGamePadRightStickJustReleasedLeft(),
                GamePadButton.RightStickRight => IsGamePadRightStickJustReleasedRight(),
                GamePadButton.DPadUp => IsGamePadDPadUpJustReleased(),
                GamePadButton.DPadDown => IsGamePadDPadDownJustReleased(),
                GamePadButton.DPadLeft => IsGamePadDPadLeftJustReleased(),
                GamePadButton.DPadRight => IsGamePadDPadRightJustReleased(),
                _ => false
            };
        }
        #endregion

        /// <summary>
        /// Update the input manager.
        /// </summary>
        /// <param name="gameTime">Time data for the current frame.</param>
        internal static void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousGamePadState = _currentGamePadState;
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
