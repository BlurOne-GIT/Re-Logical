using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Logical;

public class LevelState : GameState
{
    #region Fields
    private bool finished;
    public static int ColorJobsFinished;
    public static int MovesLeft { get => 5 - Ball.allBalls.Count; }
    public static List<BallColors> TrafficLights = new List<BallColors>(3);
    private SimpleImage oTimeBar = new SimpleImage(LevelTextures.MainPipeBar, new Vector2(304f, 35f), 1);
    private int oTimeLeft = 145;
    private readonly int oTime;
    private int oTimeLoopCounter;
    public static List<BallColors> ColorJobs = new List<BallColors>(4);
    public static BallColors NextBall { get; private set; }
    public static int TimeLeft;
    public static TimeSpan TimeSpanLeft;
    private TimeSpan initialTimeSpan;
    private static bool isTimed;
    private Block[,] Tileset;
    private Ball MainPipeBall;
    private bool paused;
    private SoundEffect successSfx;
    private SoundEffect failSfx;
    #endregion

    public LevelState():base()
    {
        ColorJobsFinished = 0;
        Ball.BallCreated += AddBall;
        Ball.BallDestroyed += RemoveBall;
        Lexer lexer = new Lexer();
        Tileset = lexer.GetLevelBlocks(Configs.Stage, out oTime, out TimeLeft, out isTimed, out string a);
        oTime++;
        initialTimeSpan = new TimeSpan(0, 1, 30) * (TimeLeft + 1);
        TimeSpanLeft = initialTimeSpan;
        foreach (Component gameObject in Tileset)
        {
            if (gameObject is IReloadable)
                (gameObject as IReloadable).Reload(Tileset); 
            if (gameObject is IOverlayable)
                foreach (Component overlayable in (gameObject as IOverlayable).GetOverlayables())
                    AddGameObject(overlayable);
            AddGameObject(gameObject);
        }

        AddGameObject(new SimpleImage(LevelTextures.MainPipe, new Vector2(16, 30), 0));
        AddGameObject(oTimeBar);
        for (int i = 0; i < 8; i++)
            if (Tileset[i, 0].FileValue is 0x01 or 0x16)
                AddGameObject(new SimpleImage(LevelTextures.MainPipeOpen, new Vector2(26 + 36 * i, 41), 1));

        Spinner.AllDone += Win;
        Statics.ShowCursor = true;
        oTimeLoopCounter = oTime;
        ColorJob.SteveJobs?.Reload();

        if (ColorJobs.Count != 0 || TrafficLights.Count != 0)
            Spinner.ConditionClear += RecheckConditioned;

    }

    public override void LoadContent(ContentManager Content)
    {
        MainPipeBall = new Ball(new Vector2(295, 33), Direction.Left, (BallColors)Statics.Brandom.Next(0, 4), false);
        successSfx = Content.Load<SoundEffect>("1Success"); // DEBUG //
        failSfx = Content.Load<SoundEffect>("1Fail"); // DEBUG //
        FadeIn();
    }

    private async void FadeIn()
    {
        await Task.Delay(660);
        for (float i = 0; i < 1; i += 0.05f)
        {
            Statics.Opacity = i;
            await Task.Delay(14);
        }
        Statics.Opacity = 1;
    }

    private async void FadeOut(Action action)
    {
        for (float i = 1; i > 0; i -= 0.05f)
        {
            Statics.Opacity = i;
            await Task.Delay(14);
        }
        Statics.Opacity = 0;
        await Task.Delay(660);
        action();
    }

    private void AddBall(object s, EventArgs e) => AddGameObject(s as Component);
    private void RemoveBall(object s, EventArgs e)
    {
        if (!finished && MainPipeBall.Equals(s))
        {
            MainPipeBall = new Ball(new Vector2(295, 33), Direction.Left, NextBall, false);
            NextBall = (BallColors)Statics.Brandom.Next(0, 4);
            oTimeLoopCounter = oTime;
            oTimeLeft = 145;
            oTimeBar.ChangePosition(new Vector2(304f, 35f));
        }
        RemoveGameObject(s as Component);
    }

    private void RecheckConditioned(object s, EventArgs e)
    {
        foreach (Block block in Tileset)
            if (block is Spinner)
                (block as Spinner).Check();
    }

    private async void Win(object s, EventArgs e)
    {
        finished = true;
        int ballsLeft = Ball.allBalls.Count - 1;

        foreach (Spinner spinner in s as List<Spinner>)
            ballsLeft += await spinner.FinalBoom();

        Statics.ShowCursor = false;
        foreach (Ball ball in Ball.allBalls.ToArray())
            ball.Dispose();

        Configs.Stage++;

        int timeLeft = 100;
        if (isTimed)
            timeLeft = (int)(TimeSpanLeft.Ticks * 100 / initialTimeSpan.Ticks); 

        successSfx.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(successSfx.Duration - new TimeSpan(0,0,0,0,280));
        FadeOut(() => SwitchState(new LoadingState(timeLeft, ballsLeft, ColorJobsFinished)));
    }


    public override void Update(GameTime gameTime)
    {

        if (paused || finished)
            return;

        if (MainPipeBall.Position.X is 15 or 297)
            MainPipeBall.Bounce();

        if (oTimeLoopCounter is 0)
        {
            oTimeLeft--;
            oTimeBar.ChangePosition(new Vector2(14f + oTimeLeft * 2, 35f));
        }
        
        if (oTimeLeft is 0)
            Lose("BALLTIMEOUT");

        if (TimeLeft is -1)
            Lose("TIMEOUT");

        oTimeLoopCounter = oTimeLoopCounter == 0 ? oTime : oTimeLoopCounter-1;
                
        base.Update(gameTime);
    }

    public override void HandleInput(object s, ButtonEventArgs e)
    {
        
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Space: Pause(); break;
            case Keys.Escape: Lose("LEVEL ABORTED"); break;
        }
    }

    public override void UnloadContent(ContentManager Content)
    {
        Content.UnloadAsset("1Success"); // DEBUG //
        Content.UnloadAsset("1Fail"); // DEBUG //
    }

    private void Pause()
    {
        paused ^= true;
        Statics.ShowCursor = !paused;
        foreach (Block block in Tileset)
            if (block is Spinner)
                (block as Spinner).Pause(paused);

        //do the transition thingy
    }

    private async void Lose(string reason)
    {
        finished = true;
        Statics.ShowCursor = false;
        foreach (Ball ball in Ball.allBalls.ToArray())
            ball.Dispose();
        failSfx.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(failSfx.Duration - new TimeSpan(0,0,0,0,280));
        FadeOut(() => SwitchState(new LoadingState(reason)));
    }

    public override void Dispose()
    {
        ColorJobs.Clear();
        TrafficLights.Clear();
        Spinner.ClearList();
        Spinner.AllDone -= Win;
        Spinner.ConditionClear -= RecheckConditioned;
        Ball.BallCreated -= AddBall;
        Ball.BallDestroyed -= RemoveBall;
        base.Dispose();
    }
}