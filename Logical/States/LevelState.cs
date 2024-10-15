using System;
using System.Collections.Generic;
using System.Linq;
using Logical.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private readonly Block[,] _tileset;
    private readonly Level _level;
    private Ball _mainPipeBall;
    private SoundEffect _successSfx;
    private SoundEffect _failSfx;
    private int _ballsLeft;
    private int _timeLeft;
    private const int BlackTime = 660;
    private const int FadeTime = 300;
    private const int DelayTime = 40;
    private const int PauserTime = 2*FadeTime + DelayTime; 
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
    private readonly TextComponent _pausedText;
    private readonly bool _autoWin = true;
    
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
        _level = Statics.LevelSet.GetLevel(Configs.Stage);
        _tileset = new Block[8,5];
        _oTime = _level.BallTime + 1;
        for (var i = 0; i < _level.Blocks.Length; i++)
        {
            var x = i % 8;
            var y = i / 8;
            _tileset[x, y] = ((FileBlock)_level.Blocks[x, y]).ToGameBlock(game);
            if (_autoWin && _level.Blocks[x, y].FileValue is 0x01)
                _autoWin = false;
        }
        if (_level.IsTimed)
        {
            Hourglass.BruceCook.InitialCycles = _level.Time;
            Hourglass.TimeOut += OnTimeOut;
        }
        foreach (var gameObject in _tileset)
        {
            Components.Add(gameObject);
            
            if (gameObject is IReloadable reloadable)
                reloadable.Reload(_tileset);
            
            if (gameObject is IOverlayable overlayable)
                foreach (var component in overlayable.GetOverlayables())
                    Components.Add(component);
            
            if (gameObject is IFixable fixable && fixable.ShallFix(Configs.FidelityLevel))
                fixable.Fix(Configs.FidelityLevel);
        }

        Components.Add(new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipe", new Vector2(16, 30), 0));
        Components.Add(
            _oTimeBar = new SimpleImage(Game, "MainPipeTime", new Vector2(304f, 35f), 1)
        );
        var intendedPipes = Configs.FidelityLevel >= IFixable.FidelityLevel.Intended;
        for (int x = 0; x < 8; x++)
            if (_level.Blocks[x, 0].FileValue is 0x01 or 0x16)
                Components.Add(
                    new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipeOpen",
                        new Vector2(25 + 36 * x, intendedPipes ? 40 : 41), 1)
                    { DefaultSource = new Rectangle(0, intendedPipes ? 0 : 1, 18, intendedPipes ? 6 : 5) }
                );

        Spinner.AllDone += Win;
        _oTimeLoopCounter = _oTime;
        ColourHandicap.SteveJobs?.Recharge();

        if (ColorJobLayout.Count != 0 || TrafficLights.Count != 0)
            Spinner.ConditionClear += RecheckConditioned;
        
        Game.Window.KeyDown += HandleInput;
        Components.Add(
            _pausedText = new TextComponent(Game, Statics.TextureFont, "PAUSED", new Vector2(113, 119), 10)
            {
                Opacity = 0f,
                Enabled = false, // Somehow writing Enable = Visible = false
                Visible = false // makes the whole state not render, who knows...
            }
        );

        Statics.Cursor.Enabled = true; // TODO: refactor this Cursor Visible/Enabled logic
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
        Statics.Cursor.Enabled = false; /*Components.Add(_fakeCursor = new SimpleImage(Game,
            Game.Content.Load<Texture2D>("Cursor"),
            HoverableArea.MouseVector - Statics.CursorTextureOffset,
            10));*/
        _ballsLeft = Ball.AllBalls.Count - 1;
        foreach (var ball in Ball.AllBalls.ToArray())
            if (ball != _mainPipeBall)
                ball.Dispose();
            else
                ball.Enabled = false;

        //Configs.Stage++;

        _timeLeft = _level.IsTimed ? Hourglass.BruceCook.TimeLeftPoints : 100;
    }


    public override void Update(GameTime gameTime)
    {
        // This piece of code looks absolutely horrible, I may change it up later by making a proper state machine or using SyncRunnables
        switch (State)
        {
            case States.BlackIn:
                if (_stateTimer >= BlackTime)
                    State = States.FadeIn;
                break;
            case States.FadeIn:
                Statics.Backdrop.Opacity = Math.Clamp(1f - _stateTimer / (float)FadeTime, 0f, 1f);
                
                if (_stateTimer < FadeTime)
                    break;
                
                _mainPipeBall = new Ball(Game, new Vector2(295, 33), Direction.Left, (BallColors)Statics.Brandom.Next(0, 4), false) { Enabled = false };
                InteractionEnabler(true);
                foreach (var ball in Ball.AllBalls)
                    ball.Visible = true;
                if (_autoWin)
                    Win(this, EventArgs.Empty);
                else
                    State = States.Playing;
                break;
            case States.Unpausing:
                _pausedText.Opacity = Math.Clamp(1f - _stateTimer / (float)FadeTime, 0f, 1f);
                Statics.Backdrop.Opacity = Math.Clamp(1f - (_stateTimer - FadeTime - DelayTime) / (float)FadeTime, 0f, 1f);

                if (_stateTimer < PauserTime)
                    break;
                
                InteractionEnabler(true);
                foreach (var ball in Ball.AllBalls)
                    ball.Visible = true;
                State = States.Playing;
                _pausedText.Visible = false;
                break;
            case States.Playing:
                if (_mainPipeBall.Position.X is 15 or 297)
                    _mainPipeBall.Bounce();

                if (_oTimeLoopCounter is 0)
                {
                    _oTimeBar.Position = new Vector2(14f + --_oTimeLeft * 2, 35f);
                    _oTimeLoopCounter = _oTime;
                }
                else
                    --_oTimeLoopCounter;
        
                if (_oTimeLeft is 0)
                    Lose("BALLTIMEOUT");
                
                break;
            case States.Paused:
                Statics.Backdrop.Opacity = Math.Clamp(_stateTimer / (float)FadeTime, 0f, 1f);
                _pausedText.Opacity = Math.Clamp((_stateTimer - FadeTime - DelayTime) / (float)FadeTime, 0f, 1f);
                break;
            case States.Won:
                if (_stateTimer < 9)
                    break;

                if (Spinner.ExplodedSpinners.Count is 0)
                {
                    Leave(_successSfx,
                        () => SwitchState(new PreviewState(Game, _timeLeft, _ballsLeft, ColorJobsFinished)));
                    break;
                }
                
                _ballsLeft += Spinner.ExplodedSpinners.First().FinalBoom();
                Spinner.ExplodedSpinners.RemoveAt(0);
                _stateTimer = 0;
                break;
            case States.Leaving:
                if (_stateTimer >= _leavingDuration)
                    State = States.FadeOut;
                break;
            case States.FadeOut:
                Statics.Backdrop.Opacity = Math.Clamp(_stateTimer / (float)FadeTime, 0f, 1f);
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

    private void HandleInput(object s, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Space:
                switch (State)
                {
                    case States.Playing:
                        State = States.Paused;
                        Statics.Cursor.Enabled = false; /*Components.Add(_fakeCursor = new SimpleImage(Game,
                            Game.Content.Load<Texture2D>("Cursor"),
                            HoverableArea.MouseVector - Statics.CursorTextureOffset,
                            10));*/
                        InteractionEnabler(false);
                        _pausedText.Visible = true;
                        break;
                    case States.Paused:
                        Statics.Cursor.Visible = false;
                        Statics.Cursor.Enabled = true; //Components.Remove(_fakeCursor);
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
        if (enable)
            Statics.Cursor.Visible = true; //Statics.ShowCursor = enable;
        foreach (var block in _tileset)
            block.Enabled = enable;
        foreach (var ball in Ball.AllBalls)
            ball.Enabled = enable;
    }

    private void Leave(SoundEffect sfx, Action action)
    {
        Statics.Cursor.Visible = false; //Statics.Cursor.Enabled = false; //Components.Remove(_fakeCursor);
        //_mainPipeBall.Dispose();
        sfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        _leavingDuration = sfx.Duration.TotalMilliseconds - FadeTime;
        _blackOutAction = action;
        State = States.Leaving;
    }
    
    private void Lose(string reason)
    {
        Statics.Cursor.Visible = false;
        InteractionEnabler(false);
        Leave(_failSfx, () => SwitchState(new PreviewState(Game, reason)));
        foreach (var ball in Ball.AllBalls.ToArray())
            ball.Dispose();
    }
    
    private void OnTimeOut(object _, EventArgs __) => Lose("TIMEOUT");

    protected override void Dispose(bool disposing)
    {
        Game.Window.KeyDown -= HandleInput;
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