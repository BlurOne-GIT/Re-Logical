using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class VideoPanel : MenuPanel
{
    private readonly Button _scaleUpButton;
    private readonly TextComponent _scaleText;
    private readonly Button _scaleDownButton;
    private readonly Button _windowModeButton;
    private readonly Button _backButton;
    private static Rectangle WindowModeRectangle => new(0, Configs.Fullscreen ? 0 : 16, 103, 16);
    
    public VideoPanel(Game game) : base(game)
    {
        Components.Add(_scaleUpButton = new Button(Game, new Rectangle(158, 111,10, 10)));
        Components.Add(_scaleText = new TextComponent(Game, Statics.BoldFont, $"{Configs.Scale:00}", new Vector2(168, 112), 3));
        Components.Add(_scaleDownButton = new Button(Game, new Rectangle(184, 111, 10, 10)));
        Components.Add(
            _windowModeButton = new Button(Game, new Rectangle(108, 109, 103, 16),
                new SimpleImage(Game, $"{Configs.GraphicSet}/UI/WindowMode", new Vector2(108, 109), 3)
                    { DefaultRectangle = WindowModeRectangle }
            ));
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        
        _scaleUpButton.LeftClicked += ScaleUp;
        _scaleDownButton.LeftClicked += ScaleDown;
        _windowModeButton.LeftClicked += Fullscreen;
        _backButton.LeftClicked += Back;
    }
    
    private void ScaleUp(object s, EventArgs e) => _scaleText.Text = $"{++Configs.Scale:00}";
    
    private void ScaleDown(object s, EventArgs e) => _scaleText.Text = $"{--Configs.Scale:00}";

    private void Fullscreen(object s, EventArgs e)
    {
        Configs.Fullscreen ^= true;
        _windowModeButton.Image!.DefaultRectangle = WindowModeRectangle;
    }
    
    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));
    
    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/WindowMode");
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        _scaleUpButton.LeftClicked -= ScaleUp;
        _scaleDownButton.LeftClicked -= ScaleDown;
        _windowModeButton.LeftClicked -= Fullscreen;
        _backButton.LeftClicked -= Back;

        base.Dispose(disposing);
    }
}