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
    private Vector2 movement;
    public List<Component> a;
    private bool shallSound;
    private bool justTped;
    public static List<Ball> allBalls = new List<Ball>(5);
    #endregion

    #region Properties
    public BallColors BallColor { 
        get => _ballColor;
        set
        {
            _ballColor = value;
            if (shallSound)
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
            if (shallSound)
                LevelTextures.Bounce.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
            switch (_direction)
            {
                case Direction.Left: movement = new Vector2(-1f, 0f); break;
                case Direction.Up: movement = new Vector2(0f, -1f); break;
                case Direction.Right: movement = new Vector2(1f, 0f); break;
                case Direction.Down: movement = new Vector2(0f, 1f); break;
            }
        }
    }
    public Vector2 Position
    { 
        get => _position;
        set 
        {
            if (!justTped)
            {
                justTped = true;
                if (shallSound)
                    LevelTextures.Tp.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
                _position = value; 
            }    
        }
    }
    #endregion

    public Ball(Vector2 position, Direction direction, BallColors ballColor, bool willSound)
    {
        shallSound = false;
        _position = position;
        MovementDirection = direction;
        BallColor = ballColor;
        allBalls.Add(this);
        shallSound = willSound;
        BallCreated.Invoke(this, new EventArgs());
        zIndex = 7;
        IsEnabled = true;
    }

    public void Bounce() => MovementDirection = Statics.ReverseDirection[MovementDirection];

    public override void Dispose()
    {
        BallDestroyed?.Invoke(this, new EventArgs());
        allBalls.Remove(this);
    }

    public void Update(GameTime gameTime)
    {
        _position += movement;
        if (_position.X is < -10 or > 320 || _position.Y is < -10 or > 256)
        {
            LevelTextures.Explode.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
            this.Dispose();
        }
        if (justTped)
            justTped = false;
    }
}