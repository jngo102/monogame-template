namespace Engine.Input
{
    #region Game Actions
    /// <summary>
    /// Represents an in-game action that is bound to an input.
    /// </summary>
    public enum GameAction
    {
        /// <summary>
        /// Action for navigating upwards.
        /// </summary>
        Up,

        /// <summary>
        /// Action for navigating downwards.
        /// </summary>
        Down,

        /// <summary>
        /// Action for navigating to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Action for navigating to the right.
        /// </summary>
        Right,

        /// <summary>
        /// Action for jumping.
        /// </summary>
        Jump,

        /// <summary>
        /// Action for canceling/exiting.
        /// </summary>
        Cancel,

        /// <summary>
        /// Used as a reference for the total number of game actions.
        /// </summary>
        TotalActions,
    }
    #endregion

    #region Game Pad Buttons
    /// <summary>
    /// An enumeration of buttons on a typical game controller.
    /// </summary>
    public enum GamePadButton
    {
        /// <summary>
        /// The A button on a game controller.
        /// </summary>
        A,

        /// <summary>
        /// The B button on a game controller.
        /// </summary>
        B,

        /// <summary>
        /// The X button on a game controller.
        /// </summary>
        X,

        /// <summary>
        /// The Y button on a game controller.
        /// </summary>
        Y,

        /// <summary>
        /// The left shoulder button on a game controller.
        /// </summary>
        LeftShoulder,

        /// <summary>
        /// The right shoulder button on a game controller.
        /// </summary>
        RightShoulder,

        /// <summary>
        /// The left trigger on a game controller.
        /// </summary>
        LeftTrigger,

        /// <summary>
        /// The right trigger on a game controller.
        /// </summary>
        RightTrigger,

        /// <summary>
        /// The back button on a game controller.
        /// </summary>
        Back,

        /// <summary>
        /// The start button on a game controller.
        /// </summary>
        Start,

        /// <summary>
        /// The left joystick of a game controller.
        /// </summary>
        LeftStick,

        /// <summary>
        /// The right joystick of a game controller.
        /// </summary>
        RightStick,

        /// <summary>
        /// The upward input of the left joystick on a game controller.
        /// </summary>
        LeftStickUp,

        /// <summary>
        /// The downward input of the left joystick on a game controller.
        /// </summary>
        LeftStickDown,

        /// <summary>
        /// The left input of the left joystick on a game controller.
        /// </summary>
        LeftStickLeft,

        /// <summary>
        /// The right input of the left joystick on a game controller.
        /// </summary>
        LeftStickRight,

        /// <summary>
        /// The upward input of the right joystick on a game controller.
        /// </summary>
        RightStickUp,

        /// <summary>
        /// The downward input of the right joystick on a game controller.
        /// </summary>
        RightStickDown,

        /// <summary>
        /// The left input of the right joystick on a game controller.
        /// </summary>
        RightStickLeft,

        /// <summary>
        /// The right input of the right joystick on a game controller.
        /// </summary>
        RightStickRight,

        /// <summary>
        /// The up button on a game controller's directional pad.
        /// </summary>
        DPadUp,

        /// <summary>
        /// The down button on a game controller's directional pad.
        /// </summary>
        DPadDown,

        /// <summary>
        /// The left button on a game controller's directional pad.
        /// </summary>
        DPadLeft,

        /// <summary>
        /// The right button on a game controller's directional pad.
        /// </summary>
        DPadRight,
    }
    #endregion
}
