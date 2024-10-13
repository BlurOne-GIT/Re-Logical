using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.States;

public class MessageState : GameState
{
    // TODO: verify these values, borrowed from pause animation
    private const int BlackTime = 660;
    private const int FadeTime = 300;
    //private const int DelayTime = 40;
    //private const int PauserTime = 2*FadeTime + DelayTime; 
    
    public MessageState(Game game, string message) : base(game) =>
        Components.Add(
            new TextComponent(game, Statics.TextureFont, message, new Vector2(161, 111), 1, Alignment.TopCenter)
        );
    
    public override void Initialize()
    {
        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(BlackTime), 
            () => Components.Add(new LoopedAction(Game, 
                (_, time) => Statics.Backdrop.Opacity = (float)Math.Clamp(1.0 - time.TotalMilliseconds / FadeTime, 0f, 1f),
                TimeSpan.FromMilliseconds(FadeTime),//PauserTime,
                () => Game.Services.GetService<ClickableWindow>().ButtonDown += OnButtonDown))
        ));
        base.Initialize();
    }

    private void OnButtonDown(object sender, MouseButtons e)
    {
        if ((e & (MouseButtons.LeftButton | MouseButtons.RightButton)) is MouseButtons.None)
            return;
        
        Game.Services.GetService<ClickableWindow>().ButtonDown -= OnButtonDown;
        Components.Add(new LoopedAction(Game,
            (_, time) => Statics.Backdrop.Opacity = (float)Math.Clamp(time.TotalMilliseconds / FadeTime, 0f, 1f),
            TimeSpan.FromMilliseconds(FadeTime),//PauserTime,
            () => Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(BlackTime),
                () => SwitchState(new MenuState(Game))
            ))
        ));
    }
}