using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MmgEngine;

namespace Logical.States;

public class PreviewState : GameState
{
    private const int BlackTime = 660;
    private const int FadeTime = 280;
    private const int TextFadeTime = 140;
    private const int DelayTime = 100;
    private const int TextTime = 3040;
    private const int BlankTime = 1280;
    private const int TotalTextTime = 2*TextFadeTime + TextTime + DelayTime;
    
    #region Fields
    private readonly Mode _mode;
    private readonly string _message;
    private TextComponent _changingDisplay;
    private readonly List<string> _displayMessages = [];
    private TextComponent _bonusMessage;
    private readonly List<int> _bonuses = [];
    private int _bonusItr;
    private LoopedAction _displayLoop;
    static private bool _showEditor;

    private enum Mode
    {
        Start,
        Failed,
        Complete
    }
    #endregion
    
    #region Constructors
    // Common constructor
    private PreviewState(Game game, Mode mode, string message) : base(game)
    {
        _message = message;
        _mode = mode;
    }
    
    // MAKE IT!
    public PreviewState(Game game) : this(game, Mode.Start, "MAKE IT!")
    {
        if (Configs.Lives is 0 || Configs.Stage is 0)
            Configs.ResetGame();
    }
    
    // YOU FAILED!
    public PreviewState(Game game, string deathReason) : this(game, Mode.Failed, "YOU FAILED!")
    {
        if (Configs.Lives != 0)
            Configs.Lives--;
        _displayMessages.Add(deathReason);
    }
    
    // YOU MADE IT!
    public PreviewState(Game game, int timeLeft, int ballsLeft, int colorJobs, bool superbonus = false)
        : this(game, Mode.Complete, (_showEditor = !Statics.LevelSet.CheckValidStage(++Configs.Stage)) ? "THE EDITOR!" : "YOU MADE IT!")
    {
        _displayMessages.Add($"TIME BONUS: 10*{timeLeft}%");
        _bonuses.Add(10 * timeLeft);
        switch (ballsLeft)
        {
            case 0:
                _displayMessages.Add("PERFECT BONUS:");
                _bonuses.Add(5000);
                break;
            case 1:
                _displayMessages.Add("SEMIPERFECT BONUS:");
                _bonuses.Add(2000);
                break;
            case < 10:
                _displayMessages.Add($"BALL BONUS: 1000-10*{ballsLeft}");
                _bonuses.Add(1000-10*ballsLeft);
                break;
            default:
                _displayMessages.Add("BALL BONUS:");
                _bonuses.Add(0);
                break;
        }

        _displayMessages.Add("COLOR JOBS:" + (colorJobs is not 0 ? $" 500*{colorJobs}" : ""));
        _bonuses.Add(500*colorJobs);

        if (superbonus)
        {
            _displayMessages.Add("S U P E R B O N U S");
            _bonuses.Add(10000);
        }

        var points = Convert.ToUInt32(_bonuses.Sum());

        _displayMessages.Add("POINTS:");
        _bonuses.Add((int)points);
        Configs.Score += points;
    }
    #endregion
    
    #region Default Methods
    protected override void LoadContent()
    {
        if (_showEditor && !Statics.LevelSet.CheckValidStage(Configs.Stage - 1))
        {
            // TODO: call GuruState
            Configs.ResetGame();
            Components.Add(new FrameDelayedAction(Game, 1, () => SwitchState(new MenuState(Game))));
            return;
        }
        
        Statics.Cursor.Visible = false; //Statics.ShowCursor = false;
        Components.Add(new SimpleImage(
            Game,
            $"{Configs.GraphicSet}/UI/Preview",
            new Vector2(0, 28),
            0
            ));
        Components.Add(new TextComponent(Game, Statics.DisplayFont, _message, new Vector2(84, 43), 1, anchor: Alignment.TopCenter));
        var pinkBall = Game.Content.Load<Texture2D>("SpinnerBalls");
        for (int i = 0; i < Configs.Lives; i++)
            Components.Add(new SimpleImage(Game, pinkBall, new Vector2(72 + 12 * i, 82), 1) { DefaultSource = new Rectangle(0, 0, 8, 8) });
        Components.Add(new TextComponent(Game, Statics.DisplayFont, _showEditor ? " THE FINAL CUT" : $"{Configs.Stage:00} {Statics.LevelSet.GetLevelName(Configs.Stage)}", new Vector2(16, 123), 1));
        if (_mode is Mode.Complete) //if (_mode is not Mode.Start)
        {
            Components.Add(
                _changingDisplay = new TextComponent(Game, Statics.DisplayFont, _displayMessages[0], new Vector2(24, 163), 1)
                    { Opacity = 0f, Enabled = false }
            );
            //if (_mode is Mode.Complete)
            //{
            Components.Add(
                _bonusMessage = new TextComponent(Game, Statics.DisplayFont, $"= {_bonuses[0]:000000}", new Vector2(232, 163), 1)
                    { Opacity = 0f, Enabled = false }
            );
            //}
        }
        Components.Add( // Score
            new TextComponent(Game, Statics.TextureFont, $"{Configs.Score:000000}", new Vector2(208, 188), 1)
                { Enabled = false }
        );
        
        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(BlackTime),
            () => Components.Add(new LoopedAction(Game,
                (_, time) => Statics.Backdrop.Opacity = (float)Math.Clamp(1f - time.TotalMilliseconds / (float) FadeTime, 0f, 1f),
                TimeSpan.FromMilliseconds(FadeTime),
                () =>
                {
                    Game.Window.KeyDown += HandleInput;
                    Game.Services.GetService<ClickableWindow>().ButtonDown += HandleInput;
                    if (_mode is Mode.Complete)
                        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(BlankTime),
                            () => Components.Add(
                                _displayLoop = new LoopedAction(Game,
                                    (_, time) => _changingDisplay.Opacity = _bonusMessage.Opacity = (float)Math.Min(
                                        Math.Clamp(time.TotalMilliseconds / TextFadeTime, 0f, 1f),
                                        Math.Clamp(
                                            1f - (time.TotalMilliseconds - TextTime - TextFadeTime) / TextFadeTime,
                                            0f, 1f
                                        )
                                    ),
                                    TimeSpan.FromMilliseconds(TotalTextTime),
                                    () =>
                                    {
                                        if (++_bonusItr >= _bonuses.Count)
                                            _bonusItr = 0;
                                        _changingDisplay.Text = _displayMessages[_bonusItr];
                                        _bonusMessage.Text = $"= {_bonuses[_bonusItr]:000000}";
                                        Components.Add(_displayLoop);
                                    }
                                )
                            )
                        ));
                }
            )) 
        ));
    }

    private void HandleInput(object s, InputKeyEventArgs e) =>
        PrepareExit(e.Key is not Keys.Escape || Configs.Lives is 0 ? Exit : () => SwitchState(new MenuState(Game)));
    

    private void HandleInput(object s, MouseButtons e)
    {
        if ((e & (MouseButtons.LeftButton | MouseButtons.RightButton)) is not MouseButtons.None)
            PrepareExit(Exit);
    }

    protected override void UnloadContent()
        => Game.Content.UnloadAsset($"{Configs.GraphicSet}/Loading");

    #endregion

    #region Custom Methods
    private void PrepareExit(Action action)
    {
        Game.Window.KeyDown -= HandleInput;
        Game.Services.GetService<ClickableWindow>().ButtonDown -= HandleInput;
        Components.Add(new LoopedAction(Game,
            (_, time) => Statics.Backdrop.Opacity = (float)Math.Clamp(time.TotalMilliseconds / (float) FadeTime, 0f, 1f),
            TimeSpan.FromMilliseconds(FadeTime),
            () => Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(BlackTime), action))
        ));
    } 
    
    private void Exit()
    {
        switch (_mode)
        {
            case Mode.Start: SwitchState(new LevelState(Game)); break;
            case Mode.Failed:
                if (Configs.Lives is 0)
                {
                    Configs.ResetGame();
                    SwitchState(new MessageState(Game, "GAME OVER!"));
                }
                else
                    SwitchState(new LevelState(Game));
                break;
            case Mode.Complete: 
                if (_showEditor)
                    SwitchState(new MessageState(Game, "CONGRATULATIONS\n  YOU MADE IT  "));
                else
                    SwitchState(new LevelState(Game));
                break;
            default: throw new NotImplementedException();
        }
    }

    #endregion
}