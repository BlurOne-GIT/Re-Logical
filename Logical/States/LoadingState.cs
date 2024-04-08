using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MmgEngine;

namespace Logical.States;

public class LoadingState : GameState
{
    #region Constructors
    public LoadingState(Game game) : base(game) // Make it! 0
    {
        Statics.Lives = 3;
        _message = "MAKE IT!";
        _mode = Mode.Start;
        LevelResources.LoadTextures();
    }
    
    public LoadingState(Game game, string deathReason) : base(game) // You failed! 1
    {
        if (Configs.Lives != 0)
            Configs.Lives--;
        _message = "YOU FAILED!";
        _mode = Mode.Failed;
        _displayMessages.Add(deathReason);
    }
    
    public LoadingState(Game game, int timeLeft, int ballsLeft, int colorJobs, bool superbonus = false) : base(game) // You made it! 2
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

        var points = _bonuses.Sum();

        _displayMessages.Add("POINTS:");
        _bonuses.Add(points);
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
        Components.Add(new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Loading"), new Vector2(0, 28), 0));
        Components.Add(new TextComponent(Game, Statics.LightFont, _message, new Vector2(84, 43), 1, anchor: Alignment.TopCenter));
        var pinkBall = Game.Content.Load<Texture2D>("SpinnerBallPink");
        for (int i = 0; i < Configs.Lives; i++)
            Components.Add(new SimpleImage(Game, pinkBall, new Vector2(72 + 12 * i, 82), 1));
        Components.Add(new TextComponent(Game, Statics.LightFont, $"{Configs.Stage:00} {/*Statics.LevelPassword*/ new Lexer(Game).GetLevelName(Configs.Stage)}", new Vector2(16, 123), 1));
        if (_mode is not Mode.Start)
        {
            // TODO: Implement changingDisplay with displayMessages
            _changingDisplay = new TextComponent(Game, Statics.LightFont, _displayMessages[0], new Vector2(24, 163), 1);
            Components.Add(_changingDisplay);
            if (_mode is Mode.Complete)
            {
                // TODO: Implement bonusMessag with bonuses
                _bonusMessage = new TextComponent(Game, Statics.LightFont, $"= {_bonuses[0]:000000}", new Vector2(232, 163), 1);
                Components.Add(_bonusMessage);
            }
        }
        Components.Add(new TextComponent(Game, Statics.BoldFont/*DEBUG TextureFont*/, $"{Configs.Score:000000}", new Vector2(209, 188), 1));
        FadeIn();
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        if (e.Key is Keys.Escape)
            FadeOut(() => {
                LevelResources.UnloadTextures();
                SwitchState(new MenuState(Game));
                });
        else
            FadeOut(Exit);
    }

    public override void HandleInput(object s, ButtonEventArgs e)
    {
        if (e.Button is "LeftButton" or "RightButton")
            FadeOut(Exit);
    }

    protected override void UnloadContent()
        => Game.Content.UnloadAsset($"{Configs.GraphicSet}/Loading");

    #endregion

    #region Custom Methods
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
    private void Exit()
    {
        switch (_mode)
        {
            case Mode.Start: SwitchState(new LevelState(Game)); break;
            case Mode.Failed:
                if (Statics.Lives == 0)
                {
                    Configs.Score = 0;
                    LevelResources.UnloadTextures();
                    SwitchState(new MenuState(Game));
                }
                else
                    SwitchState(new LevelState(Game));
                break;
            case Mode.Complete: SwitchState(new LevelState(Game)); break;
            default: throw new NotImplementedException();
        }
    }
    #endregion
}