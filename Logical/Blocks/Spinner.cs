using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Spinner : Block, IReloadable
{
    #region Fields
    private static readonly List<Spinner> ExplodedSpinners = new(0);
    public static event EventHandler AllDone;
    public static event EventHandler ConditionClear;
    private static SoundEffect _popInSfx;
    private static SoundEffect _popOutSfx;
    private static SoundEffect _spinSfx;
    private static SoundEffect _explodeSfx;
    private readonly Button _spinButton;
    private bool _hasExploded;
    private readonly Button[] _slotButtons = new Button[4];
    private readonly bool[] _closedPipes = new bool[4];
    private readonly List<BallColors?> _slotBalls = new(4) { null, null, null, null };
    
    #region Coordinates
    
    private static readonly Vector2[] ClosedPipeOffsets = {
        new(0f, 10f),  // Left
        new(10f, 0f),  // Up
        new(32f, 10f), // Right
        new(10f, 32f)  // Down
    };
    private static readonly Animation<Vector2>[] BallOffsetAnimations = {
        new(new Vector2[]{
            new(11f, 6f),
            new(8f),
            new(6f, 11f),
            new(5f, 14f)
        }, false), // Left
        new(new Vector2[]{
            new(22f, 11f),
            new(20f, 8f),
            new(17f, 6f),
            new(14f, 5f)
        }, false), // Up
        new(new Vector2[]{
            new(17f, 22f),
            new(20f),
            new(22f, 17f),
            new(23f, 14f)
        }, false), // Right
        new(new Vector2[]{
            new(6f, 17f),
            new(8f, 20f),
            new(11f, 22f),
            new(14f, 23f)
        }, false) // Down
    };
    private readonly Vector2[] _buttonOffsets =
    {
        new(4f, 13f), // Left
        new(13f, 4f), // Up
        new(23f, 13f), // Right
        new(13f, 23f) // Down
    };
    private readonly Vector2[] _registers = {
        new(0f, 13f),  // Left
        new(13f, 0f),  // Up
        new(26f, 13f), // Right
        new(13f, 26f)  // Down
    };
    private readonly Vector2 _spinTextureOffset = new(5f);
    private readonly Vector2 _explodeTextureOffset = new(4f);
    #endregion

    private readonly Animation<Rectangle> _spinAnimation = Animation<Rectangle>.TextureAnimation(new Point(26), new Point(78, 26), false, 1);
    private readonly Animation<Texture2D> _explodeAnimation = new(LevelResources.SpinnerExplode, false);

    #region Textures
    private static Texture2D _spinningTexture;
    private static Texture2D[] _closedPipeTextures;
    #endregion
    
    #endregion

    public Spinner(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.Spinner, arrayPosition, xx, yy)
    {
        _spinButton = new Button(game, new Rectangle(Position.ToPoint(), new Point(36)));
        _spinButton.RightClicked += Spin;
        if (Pos.Y == 0)
            _registers[(int)Direction.Left] = new Vector2(-13f, 13f);
        ExplodedSpinners.Capacity++;
    }

    protected override void LoadContent()
    {
        _spinningTexture ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerSpin");
        _closedPipeTextures ??= new[]
        {
            Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerClosedLeft"),
            Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerClosedUp"),
            Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerClosedRight"),
            Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/SpinnerClosedDown")
        };
        _popInSfx ??= Game.Content.Load<SoundEffect>("Sfx/PopIn");
        _popOutSfx ??= Game.Content.Load<SoundEffect>("Sfx/PopOut");
        _spinSfx ??= Game.Content.Load<SoundEffect>("Sfx/Spin");
        _explodeSfx ??= Game.Content.Load<SoundEffect>("Sfx/Explode");
        base.LoadContent();
    }

    public void Reload(Block[,] blocks)
    {
        _closedPipes[(int)Direction.Left] = Pos.X == 0 || !HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue);
        _closedPipes[(int)Direction.Up] = Pos.Y != 0 && !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue);
        _closedPipes[(int)Direction.Right] = Pos.X == 7 || !HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue);
        _closedPipes[(int)Direction.Down] = Pos.Y == 4 || !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue);
        
        for (int i = 0; i < 4; i++)
            if (_closedPipes[i])
            {
                _slotButtons[i] = new Button(Game, new Rectangle((Position + _buttonOffsets[i]).ToPoint(),
                    i % 2 == 0 ? new Point(10, 9) : new Point(9, 10))) {Enabled = false};
                _slotButtons[i].LeftClicked += PopOut;
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
                BallOffsetAnimations[i].Start();
        
        _spinSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        if (LevelState.ColorJobs.Count != 0)
            Check();
    }

    public void Pause(bool paused)
    {
        _spinButton.Enabled = !paused;
        if (paused)
        {
            foreach (var button in _slotButtons)
                if (button is not null)
                    button.Enabled = false;
        }
        else
            for (int i = 0; i < 4; i++)
                if (_slotBalls[i] is not null && _slotButtons[i] is not null)
                    _slotButtons[i].Enabled = true;
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

    public async void Check()
    {
        if (LevelState.ColorJobs.Count != 0 && _slotBalls.SequenceEqual(LevelState.ColorJobs.Cast<BallColors?>()))
        {
            await Explode();
            LevelState.ColorJobs.Clear();
            ConditionClear?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (LevelState.ColorJobs.Count != 0 || LevelState.TrafficLights.Count != 0 && _slotBalls[0] != LevelState.TrafficLights[0])
            return;

        if (_slotBalls.Any(a => a != _slotBalls[0]) || _slotBalls[0] is null) return;
        
        await Explode();
        if (LevelState.TrafficLights.Count == 0) return;
        
        LevelState.TrafficLights.RemoveAt(0);
        ConditionClear?.Invoke(this, EventArgs.Empty);
    }

    private async Task Explode(bool fb = false)
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
        while (!_explodeAnimation.IsAtEnd) {
            await Task.Delay(20);
        }
        await Task.Delay(fb ? 20 : 40);
        if (!fb && ExplodedSpinners.Count == ExplodedSpinners.Capacity)
            AllDone?.Invoke(ExplodedSpinners, EventArgs.Empty);
    }

    public async Task<int> FinalBoom()
    {
        _spinButton.Enabled = false;
        foreach (var button in _slotButtons)
            if (button is not null)
                button.Enabled = false;
        var r = _slotBalls.Count(a => a is not null);
        await Explode(true);
        return r;
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
            DrawAnotherTexture(_spinningTexture, _spinTextureOffset, 1, _spinAnimation);

        // Slots
        for (int i = 0; i < 4; i++)
            if (_slotBalls[i] is not null)
                DrawAnotherTexture(LevelResources.SpinnerBall[(int)_slotBalls[i]], BallOffsetAnimations[i].NextFrame(), 2);
            else if (_hasExploded)
                DrawAnotherTexture(LevelResources.SpinnerBallExploded, BallOffsetAnimations[i].NextFrame(), 2);

        // Closed Pipes
        for (int i = 0; i < 4; i++)
            DrawAnotherTexture(_closedPipeTextures[i], ClosedPipeOffsets[i], 1);
        
        // Explode Animation
        if (!_explodeAnimation.IsAtEnd)
            DrawAnotherTexture(_explodeAnimation.NextFrame(), _explodeTextureOffset, 3);
    }

    protected override void UnloadContent()
    {
        _popInSfx = _popOutSfx = _spinSfx = _explodeSfx = null;
        _spinningTexture = null;
        _closedPipeTextures = null;
        Game.Content.UnloadAssets(new []
        {
            "Sfx/PopIn", "Sfx/PopOut", "Sfx/Spin", "Sfx/Explode",
            "SpinnerSpin",
            $"{Configs.GraphicSet}/SpinnerClosedLeft", 
            $"{Configs.GraphicSet}/SpinnerClosedUp", 
            $"{Configs.GraphicSet}/SpinnerClosedRight", 
            $"{Configs.GraphicSet}/SpinnerClosedDown"
        });
        base.UnloadContent();
    }
}