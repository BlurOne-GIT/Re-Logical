using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private bool _finished;
    public static int ColorJobsFinished;
    public static int MovesLeft => 5 - Ball.AllBalls.Count;
    public static readonly List<BallColors> TrafficLights = new(3);
    private readonly SimpleImage _oTimeBar;
    private int _oTimeLeft = 145;
    private readonly int _oTime;
    private int _oTimeLoopCounter;
    public static List<BallColors> ColorJobLayout = new(4);
    public static BallColors NextBall { get; private set; }
    public static int TimeLeft;
    public static TimeSpan TimeSpanLeft;
    private readonly TimeSpan _initialTimeSpan;
    private static bool _isTimed;
    private readonly Block[,] _tileset;
    private Ball _mainPipeBall;
    private bool _paused;
    private SoundEffect _successSfx;
    private SoundEffect _failSfx;
    #endregion

    public LevelState(Game game) : base(game)
    {
        ColorJobsFinished = 0;
        Ball.BallCreated += AddBall;
        Ball.BallDestroyed += RemoveBall;
        var lexer = new Lexer(game);
        _tileset = lexer.GetLevelBlocks(Configs.Stage, out _oTime, out TimeLeft, out _isTimed, out _);
        _oTime++;
        _initialTimeSpan = new TimeSpan(0, 1, 30) * (TimeLeft + 1);
        TimeSpanLeft = _initialTimeSpan;
        foreach (var gameObject in _tileset)
        {
            if (gameObject is IReloadable reloadable)
                reloadable.Reload(_tileset); 
            if (gameObject is IOverlayable overlayable)
                foreach (var component in overlayable.GetOverlayables())
                    Components.Add(component);
            Components.Add(gameObject);
        }

        Components.Add(new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/MainPipe"), new Vector2(16, 30), 0));
        _oTimeBar = new SimpleImage(Game, Game.Content.Load<Texture2D>("MainPipeTime"), new Vector2(304f, 35f), 1);
        Components.Add(_oTimeBar);
        for (int i = 0; i < 8; i++)
            if (_tileset[i, 0].FileValue is 0x01 or 0x16)
                Components.Add(new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/MainPipeOpen"), new Vector2(26 + 36 * i, 41), 1));

        Spinner.AllDone += Win;
        Statics.ShowCursor = true;
        _oTimeLoopCounter = _oTime;
        ColorJob.SteveJobs?.Recharge();

        if (ColorJobLayout.Count != 0 || TrafficLights.Count != 0)
            Spinner.ConditionClear += RecheckConditioned;

    }

    protected override void LoadContent()
    {
        _successSfx = Game.Content.Load<SoundEffect>("Sfx/1/Success"); // DEBUG //
        _failSfx = Game.Content.Load<SoundEffect>("Sfx/1/Fail"); // DEBUG //
        _mainPipeBall = new Ball(Game, new Vector2(295, 33), Direction.Left, (BallColors)Statics.Brandom.Next(0, 4), false);
        FadeIn();
        base.LoadContent();
    }

    private async void FadeIn()
    {
        await Task.Delay(660);
        for (float i = 1; i > 0; i -= 0.05f)
        {
            Statics.BackdropOpacity = i;
            await Task.Delay(14);
        }
        Statics.BackdropOpacity = 0;
    }

    private async void FadeOut(Action action)
    {
        for (float i = 0; i < 1; i += 0.05f)
        {
            Statics.BackdropOpacity = i;
            await Task.Delay(14);
        }
        Statics.BackdropOpacity = 1;
        await Task.Delay(660);
        action();
    }

    private void AddBall(object s, EventArgs e) => Components.Add(s as Ball);
    
    private void RemoveBall(object s, EventArgs e)
    {
        if (!_finished && _mainPipeBall.Equals(s))
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

    private async void Win(object s, EventArgs e)
    {
        _finished = true;
        var ballsLeft = Ball.AllBalls.Count - 1;

        foreach (var spinner in (List<Spinner>)s)
            ballsLeft += await spinner.FinalBoom();

        Statics.ShowCursor = false;
        foreach (var ball in Ball.AllBalls.ToArray())
            ball.Dispose();

        Configs.Stage++;

        var timeLeft = _isTimed ? (int)(TimeSpanLeft.Ticks * 100 / _initialTimeSpan.Ticks) : 100;

        _successSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(_successSfx.Duration - new TimeSpan(0,0,0,0,280));
        FadeOut(() => SwitchState(new LoadingState(Game, timeLeft, ballsLeft, ColorJobsFinished)));
    }


    public override void Update(GameTime gameTime)
    {

        if (_paused || _finished)
            return;

        if (_mainPipeBall.Position.X is 15 or 297)
            _mainPipeBall.Bounce();

        if (_oTimeLoopCounter is 0)
        {
            _oTimeLeft--;
            _oTimeBar.Position = new Vector2(14f + _oTimeLeft * 2, 35f);
        }
        
        if (_oTimeLeft is 0)
            Lose("BALLTIMEOUT");

        if (TimeLeft is -1)
            Lose("TIMEOUT");

        _oTimeLoopCounter = _oTimeLoopCounter == 0 ? _oTime : _oTimeLoopCounter-1;
                
        base.Update(gameTime);
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Space: Pause(); break;
            case Keys.Escape: Lose("LEVEL ABORTED"); break;
        }
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset("Sfx/1/Success"); // DEBUG //
        Game.Content.UnloadAsset("Sfx/1/Fail"); // DEBUG //
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/MainPipe");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/MainPipeOpen");
        Game.Content.UnloadAsset("MainPipeTime");
        base.UnloadContent();
    }

    private void Pause()
    {
        _paused ^= true;
        Statics.ShowCursor = !_paused;
        foreach (var block in _tileset)
            if (block is Spinner spinner)
                spinner.Pause(_paused);

        //do the transition thingy
    }

    private async void Lose(string reason)
    {
        _finished = true;
        Statics.ShowCursor = false;
        foreach (var ball in Ball.AllBalls.ToArray())
            ball.Dispose();
        _failSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(_failSfx.Duration - new TimeSpan(0,0,0,0,280));
        FadeOut(() => SwitchState(new LoadingState(Game, reason)));
    }

    protected override void Dispose(bool disposing)
    {
        ColorJobLayout.Clear();
        TrafficLights.Clear();
        Spinner.ClearList();
        Spinner.AllDone -= Win;
        Spinner.ConditionClear -= RecheckConditioned;
        Ball.BallCreated -= AddBall;
        Ball.BallDestroyed -= RemoveBall;
        base.Dispose(disposing);
    }
}