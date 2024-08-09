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
    private Action _action;
    private States _state = States.BlackIn;
    private int _timer;
    private const int BlackTime = 660;
    private const int FadeTime = 280;
    private enum States
    {
        BlackIn,
        FadeIn,
        Standby,
        FadeOut,
        BlackOut
    }
    
    #region Constructors
    public PreviewState(Game game) : base(game) // MAKE IT!
    {
        if (Configs.Lives is 0 || Configs.Stage is 0)
        {
            Configs.Lives = 3;
            Configs.Score = 0;
            Configs.Stage = 1;
        }
        _message = "MAKE IT!";
        _mode = Mode.Start;
    }
    
    public PreviewState(Game game, string deathReason) : base(game) // YOU FAILED!
    {
        if (Configs.Lives != 0)
            Configs.Lives--;
        _message = "YOU FAILED!";
        _mode = Mode.Failed;
        _displayMessages.Add(deathReason);
    }
    
    public PreviewState(Game game, int timeLeft, int ballsLeft, int colorJobs, bool superbonus = false) : base(game) // YOU MADE IT!
    {
        _message = "YOU MADE IT!";
        _mode = Mode.Complete;
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

    #region Fields
    private readonly Mode _mode;
    private readonly string _message;
    private TextComponent _changingDisplay;
    private readonly List<string> _displayMessages = new();
    private TextComponent _bonusMessage;
    private readonly List<int> _bonuses = new();

    private enum Mode
    {
        Start,
        Failed,
        Complete
    }
    #endregion

    #region Default Methods
    protected override void LoadContent()
    {
        Statics.ShowCursor = false;
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
        Components.Add(new TextComponent(Game, Statics.DisplayFont, $"{Configs.Stage:00} {/*Statics.LevelPassword*/ new Lexer(Game).GetLevelName(Configs.Stage)}", new Vector2(16, 123), 1));
        if (_mode is not Mode.Start)
        {
            // TODO: Implement changingDisplay with displayMessages
            _changingDisplay = new TextComponent(Game, Statics.DisplayFont, _displayMessages[0], new Vector2(24, 163), 1);
            Components.Add(_changingDisplay);
            if (_mode is Mode.Complete)
            {
                // TODO: Implement bonusMessage with bonuses
                _bonusMessage = new TextComponent(Game, Statics.DisplayFont, $"= {_bonuses[0]:000000}", new Vector2(232, 163), 1);
                Components.Add(_bonusMessage);
            }
        }
        Components.Add(
            new TextComponent(Game, Statics.TopazFont/*DEBUG TextureFont*/, $"{Configs.Score:000000}", new Vector2(209, 188), 1)
                { Scale = new Vector2(1f, .5f) }
            );
    }

    public override void Initialize()
    {
        Game.Window.KeyDown += HandleInput;
        Game.Services.GetService<MouseHelper>().ButtonDown += HandleInput;
        base.Initialize();
    }

    private void HandleInput(object s, InputKeyEventArgs e)
    {
        if (_state != States.Standby) return;
        
        _action = e.Key is not Keys.Escape ? Exit : () => SwitchState(new MenuState(Game));
        _state = States.FadeOut;
    }

    private void HandleInput(object s, MouseButtons e)
    {
        if ((e & (MouseButtons.LeftButton | MouseButtons.RightButton)) is MouseButtons.None || _state != States.Standby) return;
        _action = Exit;
        _state = States.FadeOut;
    }

    protected override void UnloadContent()
        => Game.Content.UnloadAsset($"{Configs.GraphicSet}/Loading");

    #endregion

    #region Custom Methods
    private void Exit()
    {
        switch (_mode)
        {
            case Mode.Start: SwitchState(new LevelState(Game)); break;
            case Mode.Failed:
                if (Configs.Lives == 0)
                {
                    Configs.Score = 0;
                    SwitchState(new MenuState(Game));
                }
                else
                    SwitchState(new LevelState(Game));
                break;
            case Mode.Complete: SwitchState(new LevelState(Game)); break;
            default: throw new NotImplementedException();
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case States.BlackIn:
                if (_timer >= BlackTime)
                {
                    _state = States.FadeIn;
                    _timer = 0;
                }
                break;
            case States.FadeIn:
                Statics.BackdropOpacity = Math.Clamp(1f - _timer / (float) FadeTime, 0f, 1f);
                if (_timer >= FadeTime)
                {
                    _state = States.Standby;
                    _timer = 0;
                }
                break;
            case States.FadeOut:
                Statics.BackdropOpacity = Math.Clamp(_timer / (float) FadeTime, 0f, 1f);
                if (_timer >= FadeTime)
                {
                    _state = States.BlackOut;
                    _timer = 0;
                }
                break;
            case States.BlackOut:
                if (_timer >= BlackTime)
                    _action();
                break;
        }
        if (_state is not States.Standby)
            _timer += gameTime.ElapsedGameTime.Milliseconds;
        base.Update(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        Game.Window.KeyDown -= HandleInput;
        base.Dispose(disposing);
    }

    #endregion
}