using System;
using System.Collections.Generic;
using System.Linq;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Spinner : Block, IReloadable, IFixable
{
    #region Fields
    public static readonly List<Spinner> ExplodedSpinners = new(0);
    public static event EventHandler AllDone;
    public static event EventHandler ConditionClear;
    private static SoundEffect _popInSfx;
    private static SoundEffect _popOutSfx;
    private static SoundEffect _spinSfx;
    private static SoundEffect _explodeSfx;
    private readonly Button _spinButton;
    private bool _hasExploded;
    private readonly Button[] _slotButtons = new Button[4];
    private bool[] _closedPipes = new bool[4];
    private readonly List<BallColors?> _slotBalls = new(4) { null, null, null, null };
    
    #region Coordinates
    
    private static readonly Vector2[] ClosedPipeOffsets = {
        new( 0f, 10f), // Left
        new(10f,  0f), // Up
        new(32f, 10f), // Right
        new(10f, 32f)  // Down
    };
    private readonly Animation<Vector2>[] _ballOffsetAnimations = {
        new(new Vector2[]{
            new(11f,  6f),
            new(8f),
            new( 6f, 11f),
            new( 5f, 14f)
        }, false), // Left
        new(new Vector2[]{
            new(22f, 11f),
            new(20f,  8f),
            new(17f,  6f),
            new(14f,  5f)
        }, false), // Up
        new(new Vector2[]{
            new(17f, 22f),
            new(20f),
            new(22f, 17f),
            new(23f, 14f)
        }, false), // Right
        new(new Vector2[]{
            new( 6f, 17f),
            new( 8f, 20f),
            new(11f, 22f),
            new(14f, 23f)
        }, false) // Down
    };
    /*private readonly Vector2[] _buttonOffsets =
    {
        new(4f, 13f), // Left
        new(13f, 4f), // Up
        new(23f, 13f), // Right
        new(13f, 23f) // Down
    };*/
    private readonly Vector2[] _registers = {
        new( 0f, 13f), // Left
        new(13f,  0f), // Up
        new(26f, 13f), // Right
        new(13f, 26f)  // Down
    };

    private static readonly Rectangle[] BallRectangles =
    {
        new(0, 0, 8, 8),
        new(8, 0, 8, 8),
        new(16, 0, 8, 8),
        new(24, 0, 8, 8),
        new(32, 0, 8, 8)
    };
    private readonly Vector2 _spinTextureOffset = new(5f);
    private readonly Vector2 _explodeTextureOffset = new(4f);
    #endregion

    private readonly Animation<Rectangle> _spinAnimation = Animation<Rectangle>.TextureAnimation(new Point(26), new Point(78, 26), false, 1);
    private readonly Animation<Rectangle> _explodeAnimation = Animation<Rectangle>.TextureAnimation(new Point(28), new Point(196, 28), false, 1);

    #region Textures
    private static Texture2D _spinningTexture;
    private static Texture2D _explodingTexture;
    private static Texture2D _ballsTexture;
    private static Texture2D[] _closedPipeTextures;
    #endregion
    
    #endregion

    public Spinner(Game game, Point arrayPosition, byte xx, byte yy) 
        : base(game, "Spinner", arrayPosition, xx, yy)
    {
        DefaultRectangle = new Rectangle(0, 0, 36, 36);
        _spinButton = new Button(game, new Rectangle(Position.ToPoint(), new Point(36)));
        _spinButton.RightClicked += Spin;
        if (Pos.Y is 0)
            _registers[(int)Direction.Up] = new Vector2(13f, -13f);
        ExplodedSpinners.Capacity++;
    }

    protected override void LoadContent()
    {
        _spinningTexture ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerSpin");
        _explodingTexture ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerExplode");
        _ballsTexture ??= Game.Content.Load<Texture2D>("SpinnerBalls");
        if (_closedPipeTextures is null)
        {
            _closedPipeTextures = new Texture2D[4];
            for (var i = 0; i < 4; i++)
                _closedPipeTextures[i] = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerClosed{(Direction)i}");
        }
        _popInSfx ??= Game.Content.Load<SoundEffect>("Sfx/PopIn");
        _popOutSfx ??= Game.Content.Load<SoundEffect>("Sfx/PopOut");
        _spinSfx ??= Game.Content.Load<SoundEffect>("Sfx/Spin");
        _explodeSfx ??= Game.Content.Load<SoundEffect>("Sfx/Explode");
        base.LoadContent();
    }

    public void Reload(Block[,] blocks)
    {
        _closedPipes = new[]
        {
            Pos.X == 0 || !HorizontalAttachables.Contains(blocks[Pos.X - 1, Pos.Y].FileValue), // Left
            Pos.Y != 0 && !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y - 1].FileValue),   // Up
            Pos.X == 7 || !HorizontalAttachables.Contains(blocks[Pos.X + 1, Pos.Y].FileValue), // Right
            Pos.Y == 4 || !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y + 1].FileValue)    // Down
        };
        
        if (!_closedPipes[(int)Direction.Left])
        {
            _slotButtons[0] = new Button(Game, new Rectangle((Position + new Vector2(4f, 13f)).ToPoint(), new Point(10, 9))) {Enabled = false};
            _slotButtons[0].LeftClicked += PopOut;
        }
        if (!_closedPipes[(int)Direction.Up] && Pos.Y != 0 && blocks[Pos.X, Pos.Y-1].FileValue is not 0x16)
        {
            _slotButtons[1] = new Button(Game, new Rectangle((Position + new Vector2(13f, 4f)).ToPoint(), new Point(9, 10))) {Enabled = false};
            _slotButtons[1].LeftClicked += PopOut;
        }
        if (!_closedPipes[(int)Direction.Right])
        {
            _slotButtons[2] = new Button(Game, new Rectangle((Position + new Vector2(23f, 13f)).ToPoint(), new Point(10, 9))) {Enabled = false};
            _slotButtons[2].LeftClicked += PopOut;
        }
        if (!_closedPipes[(int)Direction.Down] && blocks[Pos.X, Pos.Y+1].FileValue is not 0x16)
        {
            _slotButtons[3] = new Button(Game, new Rectangle((Position + new Vector2(13f, 23f)).ToPoint(), new Point(9, 10))) {Enabled = false};
            _slotButtons[3].LeftClicked += PopOut;
        }
    }

    private void Spin(object s, EventArgs e)
    {
        _spinButton.Enabled = false;
        foreach (var button in _slotButtons)
            if (button is not null)
                button.Enabled = false;
        var f = _slotBalls.First();
        _slotBalls.RemoveAt(0);
        _slotBalls.Add(f);
        _spinAnimation.Start();
        
        for (int i = 0; i < 4; i++)
            if (_hasExploded || _slotBalls[i] is not null)
                _ballOffsetAnimations[i].Start();
        
        _spinSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        if (LevelState.ColorJobLayout.Count != 0)
            Check();
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        _spinButton.Enabled = Enabled;
        if (!Enabled)
        {
            foreach (var button in _slotButtons)
                if (button is not null)
                    button.Enabled = false;
        }
        else
            for (int i = 0; i < 4; i++)
                if (_slotBalls[i] is not null && _slotButtons[i] is not null)
                    _slotButtons[i].Enabled = true;
        base.OnEnabledChanged(sender, args);
    }

    private void PopOut(object sender, EventArgs e)
    {
        if (LevelState.MovesLeft == 0)
            return;

        var index = Array.IndexOf(_slotButtons, sender as Button);

        if (index is -1)
            throw new ArgumentException("External button is subscribed to the spinner event.");
        
        _slotButtons[index].Enabled = false;
        _ = new Ball(Game, _registers[index] + Position, (Direction)index, (BallColors)_slotBalls[index]!, true);
        _slotBalls[index] = null;
        _popOutSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
    }

    public override void Update(GameTime gameTime)
    {
        for (int i = 0; i < 4; i++)
            foreach (var ball in Ball.AllBalls.ToArray().Where(ball => ball.Position == _registers[i] + Position && ball.MovementDirection != (Direction)i))
                if (_slotBalls[i] is null && _spinAnimation.IsAtEnd)
                {
                    _slotBalls[i] = ball.BallColor;
                    if (_slotButtons[i] is not null)
                        _slotButtons[i].Enabled = true;
                    _popInSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                    ball.Dispose();
                    Check();
                }
                else if (Pos.Y != 0 || i != 1)
                    ball.Bounce();

        if (_spinButton.Enabled || !_spinAnimation.IsAtEnd) return;
        
        _spinButton.Enabled = true;
        for (int i = 0; i < 4; i++)
            if (_slotButtons[i] is not null && _slotBalls[i] is not null)
                _slotButtons[i].Enabled = true;
    }

    public void Check()
    {
        if (LevelState.ColorJobLayout.Count != 0 && _slotBalls.SequenceEqual(LevelState.ColorJobLayout.Cast<BallColors?>()))
        {
            Explode();
            LevelState.ColorJobLayout.Clear();
            ConditionClear?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (LevelState.ColorJobLayout.Count != 0 || LevelState.TrafficLights.Count != 0 && _slotBalls[0] != LevelState.TrafficLights[0])
            return;

        if (_slotBalls.Any(a => a != _slotBalls[0]) || _slotBalls[0] is null) return;
        
        Explode();
        if (LevelState.TrafficLights.Count == 0) return;
        
        LevelState.TrafficLights.RemoveAt(0);
        ConditionClear?.Invoke(this, EventArgs.Empty);
    }

    private void Explode(bool fb = false)
    {
        foreach (var button in _slotButtons)
            if (button is not null)
                button.Enabled = false;
        _explodeAnimation.Start();
        _hasExploded = true;
        for (int i = 0; i < 4; i++)
            _slotBalls[i] = null;
        if (!fb && !ExplodedSpinners.Contains(this))
            ExplodedSpinners.Add(this);
        _explodeSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        if (!fb && ExplodedSpinners.Count == ExplodedSpinners.Capacity)
            AllDone?.Invoke(ExplodedSpinners, EventArgs.Empty);
    }

    public int FinalBoom()
    {
        Explode(true);
        return _slotBalls.Count(a => a is not null);
    }

    public static void ClearList()
    {
        ExplodedSpinners.Clear();
        ExplodedSpinners.Capacity = 0;
    }

    protected override void Dispose(bool disposing)
    {
        _spinButton.RightClicked -= Spin;
        _spinButton.Dispose();
        foreach (var button in _slotButtons)
        {
            if (button is not null)
                button.LeftClicked -= PopOut;
            button?.Dispose();
        }
        base.Dispose(disposing);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        // Spinning Animation
        if (!_spinAnimation.IsAtEnd)
            DrawAnotherTexture(_spinningTexture, _spinTextureOffset, 1, _spinAnimation.NextFrame());

        // Slots
        for (int i = 0; i < 4; i++)
            if (_slotBalls[i] is not null)
                DrawAnotherTexture(_ballsTexture, _ballOffsetAnimations[i].NextFrame(), 2, BallRectangles[(int)_slotBalls[i]]);
            else if (_hasExploded)
                DrawAnotherTexture(_ballsTexture, _ballOffsetAnimations[i].NextFrame(), 2, BallRectangles[4]);

        // Closed Pipes
        for (int i = 0; i < 4; i++)
            if (_closedPipes[i])
                DrawAnotherTexture(_closedPipeTextures[i], ClosedPipeOffsets[i], 1);
        
        // Explode Animation
        if (!_explodeAnimation.IsAtEnd)
            DrawAnotherTexture(_explodingTexture, _explodeTextureOffset, 3, _explodeAnimation.NextFrame());
    }

    protected override void UnloadContent()
    {
        _popInSfx = _popOutSfx = _spinSfx = _explodeSfx = null;
        _spinningTexture = null;
        _explodingTexture = null;
        _ballsTexture = null;
        _closedPipeTextures = null;
        Game.Content.UnloadAssets(new []
        {
            "Sfx/PopIn", "Sfx/PopOut", "Sfx/Spin", "Sfx/Explode",
            "SpinnerBalls",
            $"{Configs.GraphicSet}/SpinnerSpin",
            $"{Configs.GraphicSet}/SpinnerExplode",
            $"{Configs.GraphicSet}/SpinnerClosedLeft", 
            $"{Configs.GraphicSet}/SpinnerClosedUp", 
            $"{Configs.GraphicSet}/SpinnerClosedRight", 
            $"{Configs.GraphicSet}/SpinnerClosedDown"
        });
        base.UnloadContent();
    }

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Fixed;
    
    public void Fix(IFixable.FidelityLevel _)
    {
        DefaultRectangle = new Rectangle(36, 0, 36, 36);
        // TODO: Implement fixes for the other problems
    }
}