using System;
using System.Collections.Generic;
using System.Linq;
using Logical.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MmgEngine;

namespace Logical.States;

public class LevelState : GameState
{
    #region Fields
    public static int ColorJobsFinished;
    public static int MovesLeft => 5 - Ball.AllBalls.Count;
    public static readonly List<BallColors> TrafficLights = new(3);
    private readonly SimpleImage _oTimeBar;
    private int _oTimeLeft = 145;
    private readonly int _oTime;
    private int _oTimeLoopCounter;
    public static List<BallColors> ColorJobLayout = new(4);
    public static BallColors NextBall { get; private set; }
    private static bool IsTimed => Hourglass.BruceCook is not null;
    private readonly Block[,] _tileset;
    private Ball _mainPipeBall;
    private SoundEffect _successSfx;
    private SoundEffect _failSfx;
    private int _ballsLeft;
    private int _timeLeft;
    private const int BlackTime = 660;
    private const int FadeTime = 280;
    private States _state = States.BlackIn;
    private States State
    {
        get => _state;
        set
        {
            _stateTimer = 0;
            _state = value;
        }
    }

    private int _stateTimer;
    private double _leavingDuration;
    private Action _blackOutAction;
    private SimpleImage _fakeCursor;
    
    private enum States
    {
        BlackIn,
        FadeIn,
        Playing,
        Paused,
        Unpausing,
        Won,
        Leaving,
        FadeOut,
        BlackOut
    }
    #endregion

    public LevelState(Game game) : base(game)
    {
        ColorJobsFinished = 0;
        Ball.BallCreated += AddBall;
        Ball.BallDestroyed += RemoveBall;
        var lexer = new Lexer(game);
        _tileset = lexer.GetLevelBlocks(Configs.Stage, out _oTime, out var timeLeft, out _, out _);
        _oTime++;
        if (IsTimed)
        {
            Hourglass.BruceCook.InitialCycles = timeLeft;
            Hourglass.TimeOut += OnTimeOut;
        }
        foreach (var gameObject in _tileset)
        {
            if (gameObject is IReloadable reloadable)
                reloadable.Reload(_tileset);
            
            if (gameObject is IOverlayable overlayable)
                foreach (var component in overlayable.GetOverlayables())
                    Components.Add(component);
            
            if (gameObject is IFixable fixable && fixable.ShallFix(Configs.FidelityLevel))
                fixable.Fix(Configs.FidelityLevel);
            
            Components.Add(gameObject);
        }

        Components.Add(new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipe", new Vector2(16, 30), 0));
        _oTimeBar = new SimpleImage(Game, "MainPipeTime", new Vector2(304f, 35f), 1);
        Components.Add(_oTimeBar);
        for (int x = 0; x < 8; x++)
            if (_tileset[x, 0].FileValue is 0x01 or 0x16)
                Components.Add(new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipeOpen", new Vector2(25 + 36 * x, 41), 1));

        Spinner.AllDone += Win;
        _oTimeLoopCounter = _oTime;
        ColourHandicap.SteveJobs?.Recharge();

        if (ColorJobLayout.Count != 0 || TrafficLights.Count != 0)
            Spinner.ConditionClear += RecheckConditioned;
    }

    protected override void LoadContent()
    {
        _successSfx = Game.Content.Load<SoundEffect>("Sfx/1/Success"); // DEBUG //
        _failSfx = Game.Content.Load<SoundEffect>("Sfx/1/Fail"); // DEBUG //
        base.LoadContent();
    }

    private void AddBall(object s, EventArgs e) => Components.Add(s as Ball);
    
    private void RemoveBall(object s, EventArgs e)
    {
        if (State is States.Playing && _mainPipeBall.Equals(s))
        {
            _mainPipeBall = new Ball(Game, new Vector2(295, 33), Direction.Left, NextBall, false);
            NextBall = (BallColors)Statics.Brandom.Next(0, 4);
            _oTimeLoopCounter = _oTime;
            _oTimeLeft = 145;
            _oTimeBar.Position = new Vector2(304f, 35f);
        }
        Components.Remove(s as Ball);
    }

    private void RecheckConditioned(object s, EventArgs e)
    {
        foreach (var block in _tileset)
            if (block is Spinner spinner)
                spinner.Check();
    }

    private void Win(object s, EventArgs e)
    {
        State = States.Won;
        _stateTimer = 1;
        InteractionEnabler(false);
        Components.Add(_fakeCursor = new SimpleImage(Game,
            Game.Content.Load<Texture2D>("Cursor"),
            Input.MousePoint.ToVector2() - Statics.CursorTextureOffset,
            10));
        _ballsLeft = Ball.AllBalls.Count - 1;
        foreach (var ball in Ball.AllBalls.ToArray())
            if (ball != _mainPipeBall)
                ball.Dispose();
            else
                ball.Enabled = false;

        Configs.Stage++;

        _timeLeft = IsTimed ? Hourglass.BruceCook.TimeLeftPoints : 100;
    }


    public override void Update(GameTime gameTime)
    {
        switch (State)
        {
            case States.BlackIn:
                if (_stateTimer >= BlackTime)
                    State = States.FadeIn;
                break;
            case States.FadeIn:
            case States.Unpausing:
                Statics.BackdropOpacity = Math.Clamp(1f - _stateTimer / (float) FadeTime, 0f, 1f);
                if (_stateTimer >= FadeTime)
                {
                    if (State is States.FadeIn)
                        _mainPipeBall = new Ball(Game, new Vector2(295, 33), Direction.Left, (BallColors)Statics.Brandom.Next(0, 4), false) { Enabled = false };
                    InteractionEnabler(true);
                    foreach (var ball in Ball.AllBalls)
                        ball.Visible = true;
                    State = States.Playing;
                }
                break;
            case States.Playing:
                if (_mainPipeBall.Position.X is 15 or 297)
                    _mainPipeBall.Bounce();

                if (_oTimeLoopCounter is 0)
                {
                    _oTimeLeft--;
                    _oTimeBar.Position = new Vector2(14f + _oTimeLeft * 2, 35f);
                }
        
                if (_oTimeLeft is 0)
                    Lose("BALLTIMEOUT");

                _oTimeLoopCounter = _oTimeLoopCounter == 0 ? _oTime : _oTimeLoopCounter-1;
                break;
            case States.Paused:
                Statics.BackdropOpacity = Math.Clamp(_stateTimer / (float) FadeTime, 0f, 1f);
                break;
            case States.Won:
                if (_stateTimer >= 9)
                {
                    _ballsLeft += Spinner.ExplodedSpinners.First().FinalBoom();
                    Spinner.ExplodedSpinners.RemoveAt(0);
                    _stateTimer = 0;
                    if (Spinner.ExplodedSpinners.Count is 0)
                        Leave(_successSfx,
                            () => SwitchState(new PreviewState(Game, _timeLeft, _ballsLeft, ColorJobsFinished)));
                }
                break;
            case States.Leaving:
                if (_stateTimer >= _leavingDuration)
                    State = States.FadeOut;
                break;
            case States.FadeOut:
                Statics.BackdropOpacity = Math.Clamp(_stateTimer / (float) FadeTime, 0f, 1f);
                if (_stateTimer >= FadeTime)
                    State = States.BlackOut;
                break;
            case States.BlackOut:
                if (_stateTimer >= BlackTime)
                    _blackOutAction();
                break;
        }

        if (State is not States.Won)
            _stateTimer += gameTime.ElapsedGameTime.Milliseconds;
        else
            ++_stateTimer;
        
        base.Update(gameTime);
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Space:
                switch (State)
                {
                    case States.Playing:
                        State = States.Paused;
                        Components.Add(_fakeCursor = new SimpleImage(Game,
                            Game.Content.Load<Texture2D>("Cursor"),
                            Input.MousePoint.ToVector2() - Statics.CursorTextureOffset,
                            10));
                        InteractionEnabler(false);
                        break;
                    case States.Paused:
                        Components.Remove(_fakeCursor);
                        foreach (var ball in Ball.AllBalls)
                            ball.Visible = false;
                        State = States.Unpausing;
                        break;
                }   
                break;
            case Keys.Escape:
                if (State is States.Playing)
                    Lose("LEVEL ABORTED");
                break;
        }
    }

    protected override void UnloadContent()
    {
        //Game.Content.UnloadAsset("Sfx/1/Success"); // DEBUG //
        //Game.Content.UnloadAsset("Sfx/1/Fail"); // DEBUG //
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/MainPipe");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/MainPipeOpen");
        Game.Content.UnloadAsset("MainPipeTime");
        base.UnloadContent();
    }

    private void InteractionEnabler(bool enable)
    {
        Statics.ShowCursor = enable;
        foreach (var block in _tileset)
            block.Enabled = enable;
        foreach (var ball in Ball.AllBalls)
            ball.Enabled = enable;
    }

    private void Leave(SoundEffect sfx, Action action)
    {
        Components.Remove(_fakeCursor);
        _mainPipeBall.Dispose();
        sfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        _leavingDuration = sfx.Duration.TotalMilliseconds - FadeTime;
        _blackOutAction = action;
        State = States.Leaving;
    }
    
    private void Lose(string reason)
    {
        InteractionEnabler(false);
        Leave(_failSfx, () => SwitchState(new PreviewState(Game, reason)));
        foreach (var ball in Ball.AllBalls.ToArray())
            ball.Dispose();
    }
    
    private void OnTimeOut(object _, EventArgs __) => Lose("TIMEOUT");

    protected override void Dispose(bool disposing)
    {
        ColorJobLayout.Clear();
        TrafficLights.Clear();
        Spinner.ClearList();
        Spinner.AllDone -= Win;
        Spinner.ConditionClear -= RecheckConditioned;
        Ball.BallCreated -= AddBall;
        Ball.BallDestroyed -= RemoveBall;
        Hourglass.TimeOut -= OnTimeOut;
        base.Dispose(disposing);
    }
}