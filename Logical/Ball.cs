using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Ball : Component, IUpdateable
{
    #region Fields
    public static event EventHandler BallCreated;
    public static event EventHandler BallDestroyed;
    private BallColors _ballColor;
    private Direction _direction;
    private Vector2 _movement;
    public List<Component> a;
    private readonly bool _shallSound;
    private bool _justTeleported;
    public static readonly List<Ball> AllBalls = new(5);
    #endregion

    #region Properties
    public BallColors BallColor { 
        get => _ballColor;
        set
        {
            _ballColor = value;
            if (_shallSound)
                LevelTextures.ColorChange.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
            _texture = LevelTextures.Ball[(int)_ballColor];
        }
    }
    public Direction MovementDirection
    {
        get => _direction;
        set
        {
            _direction = value;
            if (_shallSound)
                LevelTextures.Bounce.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
            switch (_direction)
            {
                case Direction.Left: _movement = new Vector2(-1f, 0f); break;
                case Direction.Up: _movement = new Vector2(0f, -1f); break;
                case Direction.Right: _movement = new Vector2(1f, 0f); break;
                case Direction.Down: _movement = new Vector2(0f, 1f); break;
            }
        }
    }
    public Vector2 Position
    { 
        get => _position;
        set 
        {
            if (!_justTeleported)
            {
                _justTeleported = true;
                if (_shallSound)
                    LevelTextures.Tp.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
                _position = value; 
            }    
        }
    }
    #endregion

    public Ball(Vector2 position, Direction direction, BallColors ballColor, bool willSound)
    {
        _shallSound = false;
        _position = position;
        MovementDirection = direction;
        BallColor = ballColor;
        AllBalls.Add(this);
        _shallSound = willSound;
        BallCreated?.Invoke(this, EventArgs.Empty);
        zIndex = 7;
        IsEnabled = true;
    }

    public void Bounce() => MovementDirection = Statics.ReverseDirection[MovementDirection];

    public override void Dispose()
    {
        BallDestroyed?.Invoke(this, EventArgs.Empty);
        AllBalls.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        _position += _movement;
        if (_position.X is < -10 or > 320 || _position.Y is < -10 or > 256)
        {
            LevelTextures.Explode.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            Dispose();
        }
        if (_justTeleported)
            _justTeleported = false;
    }
}