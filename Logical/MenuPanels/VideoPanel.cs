using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class VideoPanel : MenuPanel
{
    private readonly ClickableArea _windowModeButton;
    private readonly SimpleImage _windowModeImage; 
    private static Rectangle WindowModeRectangle => new(0, Configs.Fullscreen ? 0 : 16, 103, 16);
    
    private readonly ClickableArea _scaleUpButton;
    private readonly TextComponent _scaleText;
    private readonly ClickableArea _scaleDownButton;
    
    private readonly ClickableArea _backButton;
    
    public VideoPanel(Game game) : base(game)
    {
        Components.Add(_windowModeButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), false));
        Components.Add(_windowModeImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/WindowMode", new Vector2(108, 87), 3)
            { DefaultSource = WindowModeRectangle }
        );
        Components.Add(_scaleUpButton = new ClickableArea(Game, new Rectangle(158, 111,10, 10), false));
        Components.Add(_scaleText =
            new TextComponent(Game, Statics.TopazFont, $"{Configs.Scale:00}", new Vector2(168, 112), 3)
            {
                Scale = new Vector2(1f, .5f),
                Color = Statics.TopazColor
            }
        );
        Components.Add(_scaleDownButton = new ClickableArea(Game, new Rectangle(184, 111, 10, 10), false));
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), false));
        
        _scaleUpButton.LeftButtonDown += ScaleUp;
        _scaleDownButton.LeftButtonDown += ScaleDown;
        _windowModeButton.LeftButtonDown += Fullscreen;
        _backButton.LeftButtonDown += Back;
    }

    private void ScaleUp(object s, EventArgs e) => _scaleText.Text = $"{++Configs.Scale:00}";
    
    private void ScaleDown(object s, EventArgs e) => _scaleText.Text = $"{--Configs.Scale:00}";

    private void Fullscreen(object s, EventArgs e)
    {
        Configs.Fullscreen ^= true;
        _windowModeImage.DefaultSource = WindowModeRectangle;
    }
    
    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));
    
    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/WindowMode");
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        _scaleUpButton.LeftButtonDown -= ScaleUp;
        _scaleDownButton.LeftButtonDown -= ScaleDown;
        _windowModeButton.LeftButtonDown -= Fullscreen;
        _backButton.LeftButtonDown -= Back;

        base.Dispose(disposing);
    }
}