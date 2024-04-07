using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical;

public class Ball : SimpleImage
{
    #region Fields
    public static event EventHandler BallCreated;
    public static event EventHandler BallDestroyed;
    private BallColors _ballColor;
    private Direction _direction;
    private Vector2 _movement;
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
                LevelResources.ColorChange.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            ChangeTexture(LevelResources.Ball[(int)_ballColor]);
        }
    }
    public Direction MovementDirection
    {
        get => _direction;
        set
        {
            _direction = value;
            if (_shallSound)
                LevelResources.Bounce.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            _movement = _direction switch
            {
                Direction.Left => new Vector2(-1f, 0f),
                Direction.Up => new Vector2(0f, -1f),
                Direction.Right => new Vector2(1f, 0f),
                Direction.Down => new Vector2(0f, 1f),
                _ => _movement
            };
        }
    }
    public new Vector2 Position
    { 
        get => base.Position;
        set
        {
            if (_justTeleported) return;
            
            _justTeleported = true;
            if (_shallSound)
                LevelResources.Tp.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            base.Position = value;
        }
    }
    #endregion

    public Ball(Game game, Vector2 position, Direction direction, BallColors ballColor, bool willSound) : base(game, LevelResources.Ball[(int)ballColor], position, 7)
    {
        _shallSound = false;
        MovementDirection = direction;
        BallColor = ballColor;
        AllBalls.Add(this);
        _shallSound = willSound;
        BallCreated?.Invoke(this, EventArgs.Empty);
    }

    public void Bounce() => MovementDirection = Statics.ReverseDirection[MovementDirection];

    protected override void Dispose(bool disposing)
    {
        BallDestroyed?.Invoke(this, EventArgs.Empty);
        AllBalls.Remove(this);
        base.Dispose(disposing);
    }

    public override void Update(GameTime gameTime)
    {
        base.Position += _movement;
        if (Position.X is < -10 or > 320 || Position.Y is < -10 or > 256)
        {
            LevelResources.Explode.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            Dispose();
        }
        if (_justTeleported)
            _justTeleported = false;
    }
}