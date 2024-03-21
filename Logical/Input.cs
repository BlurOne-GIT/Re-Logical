using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Logical;

public class ButtonEventArgs : EventArgs
{
    public ButtonEventArgs(string button, Point position) { Button = button; Position = position; }
    public string Button { get; }
    public Point Position { get; }
}

public static class Input
{
    #region Events
    public static event EventHandler<ButtonEventArgs> ButtonDown;
    public static event EventHandler<ButtonEventArgs> ButtonUp;

    public static event EventHandler<InputKeyEventArgs> KeyDown;
    public static event EventHandler<InputKeyEventArgs> KeyUp;
    #endregion

    #region Fields
    private static bool _leftButton;
    private static bool _middleButton;
    private static bool _rightButton;
    private static bool _xButton1;
    private static bool _xButton2;
    private static Point _position;
    private static bool[] _keys = new bool[255];
    #endregion

    #region Properties
    private static bool LeftButton { set => CheckInput(ref _leftButton, value); }
    private static bool MiddleButton { set => CheckInput(ref _middleButton, value); }
    private static bool RightButton { set => CheckInput(ref _rightButton, value); }
    private static bool XButton1 { set => CheckInput(ref _xButton1, value); }
    private static bool XButton2 { set => CheckInput(ref _xButton2, value); }
    #endregion

    #region Methods
    public static void UpdateInputs(MouseState mouseState)
    {
        LeftButton = Convert.ToBoolean(mouseState.LeftButton);
        MiddleButton = Convert.ToBoolean(mouseState.MiddleButton);
        RightButton = Convert.ToBoolean(mouseState.RightButton);
        XButton1 = Convert.ToBoolean(mouseState.XButton1);
        XButton2 = Convert.ToBoolean(mouseState.XButton2);
        _position = mouseState.Position;
    }
    public static void UpdateInputs(KeyboardState keyboardState)
    {
        for (int i = 0; i < 255; i++)
        {
            CheckInput(i, keyboardState.IsKeyDown((Keys)i));
        }
    }

    private static void CheckInput(ref bool button, bool value, [CallerMemberName] string name = null)
    {
        if (button == value)
            return;
        
        button = value;
        if (!Statics.WindowFocused || Statics.MousePoint.X < 0 || Statics.MousePoint.Y < 0 || Statics.MousePoint.X > Configs.Width || Statics.MousePoint.Y > Configs.Height)
            return;
        if (value)
            ButtonDown?.Invoke(null, new ButtonEventArgs(name, _position));
        else
            ButtonUp?.Invoke(null, new ButtonEventArgs(name, _position));
    }
    private static void CheckInput(int i, bool value)
    {
        if (_keys[i] == value)
            return;

        _keys[i] = value;
        if (!Statics.WindowFocused)
            return;
        if (value)
            KeyDown?.Invoke(null, new InputKeyEventArgs((Keys)i));
        else
            KeyUp?.Invoke(null, new InputKeyEventArgs((Keys)i));
    }
    #endregion
}