using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Logical.States;

public class LoadingState : GameState
{
    #region Constructors
    public LoadingState() // Make it! 0
    {
        Statics.Lives = 3;
        _message = "MAKE IT!";
        _mode = Mode.Start;
        LevelTextures.LoadTextures();
    }
    public LoadingState(string deathReason) // You failed! 1
    {
        if (Configs.Lives != 0)
            Configs.Lives--;
        _message = "YOU FAILED!";
        _mode = Mode.Failed;
        _displayMessages.Add(deathReason);
    }
    public LoadingState(int timeLeft, int ballsLeft, int colorJobs, bool superbonus = false) // You made it! 2
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

        int points = _bonuses.Sum();

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
    public override void LoadContent(ContentManager content)
    {
        Statics.ShowCursor = false;
        AddGameObject(new SimpleImage(content.Load<Texture2D>($"{Configs.GraphicSet}/Loading"), new Vector2(0, 28), 0));
        AddGameObject(new TextComponent(Statics.LightFont, new Vector2(84, 43), Color.White, _message, 1, TextComponent.Alignment.Center));
        var pinkBall = content.Load<Texture2D>("SpinnerBallPink");
        for (int i = 0; i < Configs.Lives; i++)
            AddGameObject(new SimpleImage(pinkBall, new Vector2(72 + 12 * i, 82), 1));
        AddGameObject(new TextComponent(Statics.LightFont, new Vector2(16, 123), Color.White, $"{Configs.Stage:00} {/*Statics.LevelPassword*/ new Lexer().GetLevelName(Configs.Stage)}", 1));
        if (_mode is not Mode.Start)
        {
            // TODO: Implement changingDisplay with displayMessages
            _changingDisplay = new TextComponent(Statics.LightFont, new Vector2(24, 163), Color.White, _displayMessages[0], 1);
            AddGameObject(_changingDisplay);
            if (_mode is Mode.Complete)
            {
                // TODO: Implement bonusMessag with bonuses
                _bonusMessage = new TextComponent(Statics.LightFont, new Vector2(232, 163), Color.White, $"= {_bonuses[0]:000000}", 1);
                AddGameObject(_bonusMessage);
            }
        }
        AddGameObject(new TextComponent(Statics.BoldFont/*DEBUG TextureFont*/, new Vector2(209, 188), Color.White, $"{Configs.Score:000000}", 1));
        FadeIn();
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        if (e.Key is Keys.Escape)
            FadeOut(() => {
                LevelTextures.UnloadTextures();
                SwitchState(new MenuState());
                });
        else
            FadeOut(Exit);
    }

    public override void HandleInput(object s, ButtonEventArgs e)
    {
        if (e.Button is "LeftButton" or "RightButton")
            FadeOut(Exit);
    }

    public override void UnloadContent(ContentManager content)
        => content.UnloadAsset($"{Configs.GraphicSet}/Loading");

    #endregion

    #region Custom Methods
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
    private void Exit()
    {
        switch (_mode)
        {
            case Mode.Start: SwitchState(new LevelState()); break;
            case Mode.Failed:
                if (Statics.Lives == 0)
                {
                    Configs.Score = 0;
                    LevelTextures.UnloadTextures();
                    SwitchState(new MenuState());
                }
                else
                    SwitchState(new LevelState());
                break;
            case Mode.Complete: SwitchState(new LevelState()); break;
            default: throw new NotImplementedException();
        }
    }
    #endregion
}