using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class VideoPanel : MenuPanel
{
    private readonly Button _scaleButton;
    private readonly Button _fullScreenButton;
    private readonly Button _backButton;
    
    public VideoPanel(Game game) : base(game)
    {
        Components.Add(_scaleButton = new Button(Game, new Rectangle(108, 87,103, 16)));
        Components.Add(
            _fullScreenButton = new Button(Game, new Rectangle(108, 109, 103, 16),
                new SimpleImage(Game, $"{Configs.GraphicSet}/UI/WindowMode", new Vector2(108, 109), 3)
                    { DefaultRectangle = new Rectangle(0, 0, 103, Configs.Fullscreen ? 16 : 0) }
            ));
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        
        _scaleButton.LeftClicked += Scale;
        _fullScreenButton.LeftClicked += Fullscreen;
        _backButton.LeftClicked += Back;
    }
    
    private void Scale(object s, EventArgs e)
    {
        if (Configs.Scale != Configs.MaxScale)
            Configs.Scale++;
        else
            Configs.Scale = 1;
    }
    
    private void Fullscreen(object s, EventArgs e)
    {
        Configs.Fullscreen ^= true;
        _fullScreenButton.Image.DefaultRectangle = new Rectangle(0, 0, 103, Configs.Fullscreen ? 16 : 0);
    }
    
    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));
    
    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/WindowMode");
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        _scaleButton.LeftClicked -= Scale;
        _fullScreenButton.LeftClicked -= Fullscreen;
        _backButton.LeftClicked -= Back;

        base.Dispose(disposing);
    }
}