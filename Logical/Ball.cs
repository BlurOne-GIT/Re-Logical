using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical;

public class Ball : SimpleImage
{
    #region Fields
    public static event EventHandler BallCreated;
    public static event EventHandler BallDestroyed;
    private static SoundEffect _bounceSfx;
    private static SoundEffect _colorChangeSfx;
    private static SoundEffect _tpSfx;
    private static SoundEffect _explodeSfx;
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
                _colorChangeSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            DefaultRectangle = new Rectangle(10 * (int)_ballColor, 0, 10, 10);
        }
    }
    public Direction MovementDirection
    {
        get => _direction;
        set
        {
            _direction = value;
            if (_shallSound)
                _bounceSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
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
                _tpSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            base.Position = value;
        }
    }
    #endregion

    public Ball(Game game, Vector2 position, Direction direction, BallColors ballColor, bool willSound)
        : base(game, game.Content.Load<Texture2D>("Balls"), position, 7)
    {
        _shallSound = false;
        MovementDirection = direction;
        BallColor = ballColor;
        AllBalls.Add(this);
        _shallSound = willSound;
        BallCreated?.Invoke(this, EventArgs.Empty);
    }

    protected override void LoadContent()
    {
        if (_bounceSfx is null || _bounceSfx.IsDisposed)
            _bounceSfx = Game.Content.Load<SoundEffect>("Sfx/Bounce");
        if (_colorChangeSfx is null || _colorChangeSfx.IsDisposed)
            _colorChangeSfx = Game.Content.Load<SoundEffect>("Sfx/ColorChange");
        if (_tpSfx is null || _tpSfx.IsDisposed)
            _tpSfx = Game.Content.Load<SoundEffect>("Sfx/Tp");
        if (_explodeSfx is null || _explodeSfx.IsDisposed)
            _explodeSfx = Game.Content.Load<SoundEffect>("Sfx/Explode");
        base.LoadContent();
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
            _explodeSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
            Dispose();
        }
        if (_justTeleported)
            _justTeleported = false;
    }
}