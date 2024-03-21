using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class LoadingState : GameState
{
    #region Constructors
    public LoadingState() // Make it! 0
    {
        Statics.Lives = 3;
        message = "MAKE IT!";
        mode = 0;
        LevelTextures.LoadTextures();
    }
    public LoadingState(string deathReason) // You failed! 1
    {
        if (Configs.Lives != 0)
            Configs.Lives--;
        message = "YOU FAILED!";
        mode = 1;
        displayMessages.Add(deathReason);
    }
    public LoadingState(int timeLeft, int ballsLeft, int colorJobs, bool SUPERBONUS = false) // You made it! 2
    {
        message = "YOU MADE IT!";
        mode = 2;
        int points = 0;
        displayMessages.Add($"TIME BONUS: 10*{timeLeft}%");
        bonuses.Add(10 * timeLeft);
        if (ballsLeft is 0)
        {
            displayMessages.Add("PERFECT BONUS:");
            bonuses.Add(5000);
        }
        else if (ballsLeft is 1)
        {
            displayMessages.Add("SEMIPERFECT BONUS:");
            bonuses.Add(2000);
        }
        else if (ballsLeft < 10)
        {
            displayMessages.Add($"BALL BONUS: 1000-10*{ballsLeft}");
            bonuses.Add(1000-10*ballsLeft);
        }
        else
        {
            displayMessages.Add("BALL BONUS:");
            bonuses.Add(0);
        }

        if (colorJobs is not 0)
            displayMessages.Add($"COLOR JOBS: 500*{colorJobs}");
        else
            displayMessages.Add("COLOR JOBS:");
        bonuses.Add(500*colorJobs);

        if (SUPERBONUS)
        {
            displayMessages.Add("S U P E R B O N U S");
            bonuses.Add(10000);
        }

        foreach (int i in bonuses)
            points += i;

        displayMessages.Add("POINTS:");
        bonuses.Add(points);
        Configs.Score += points;
    }
    #endregion

    #region Fields
    private int mode;
    private string message;
    private TextComponent changingDisplay;
    private List<string> displayMessages = new List<string>();
    private TextComponent bonusMessage;
    private List<int> bonuses = new List<int>();
    #endregion

    #region Default Methods
    public override void LoadContent(ContentManager Content)
    {
        Statics.ShowCursor = false;
        AddGameObject(new SimpleImage(Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Loading"), new Vector2(0, 28), 0));
        AddGameObject(new TextComponent(Statics.LightFont, new Vector2(84, 43), Color.White, message, 1, TextComponent.Alignment.Center));
        Texture2D pinkBall = Content.Load<Texture2D>("SpinnerBallPink");
        for (int i = 0; i < Configs.Lives; i++)
            AddGameObject(new SimpleImage(pinkBall, new Vector2(72 + 12 * i, 82), 1));
        AddGameObject(new TextComponent(Statics.LightFont, new Vector2(16, 123), Color.White, $"{Configs.Stage:00} {/*Statics.LevelPassword*/ new Lexer().GetLevelName(Configs.Stage)}", 1));
        if (mode is not 0)
        {
            // TODO: Implement changingDisplay with displayMessages
            changingDisplay = new TextComponent(Statics.LightFont, new Vector2(24, 163), Color.White, displayMessages[0], 1);
            AddGameObject(changingDisplay);
            if (mode is 2)
            {
                // TODO: Implement bonusMessag with bonuses
                bonusMessage = new TextComponent(Statics.LightFont, new Vector2(232, 163), Color.White, $"= {bonuses[0]:000000}", 1);
                AddGameObject(bonusMessage);
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
            FadeOut(() => Exit());
    }

    public override void HandleInput(object s, ButtonEventArgs e)
    {
        if (e.Button is "LeftButton" or "RightButton")
        {
            FadeOut(() => Exit());
        }
    }

    public override void UnloadContent(ContentManager Content)
    {
        Content.UnloadAsset((@$"{Configs.GraphicSet}\Loading"));
    }
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
        switch (mode)
        {
            case 0: SwitchState(new LevelState()); break;
            case 1:
                if (Statics.Lives == 0)
                {
                    Configs.Score = 0;
                    LevelTextures.UnloadTextures();
                    SwitchState(new MenuState());
                }
                else
                    SwitchState(new LevelState());
                break;
            case 2: SwitchState(new LevelState()); break;
            default: throw new NotImplementedException();
        }
    }
    #endregion
}