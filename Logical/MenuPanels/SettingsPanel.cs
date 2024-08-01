using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.MenuPanels;

public class SettingsPanel : MenuPanel
{
    private readonly Button _scaleButton;
    private readonly Button _fullScreenButton;
    private readonly Button _bgmVolUpButton;
    private readonly Button _bgmVolDownButton;
    private readonly Button _sfxVolUpButton;
    private readonly Button _sfxVolDownButton;
    private readonly Button _stereoSplitUpButton;
    private readonly Button _stereoSplitDownButton;
    private readonly Button _backButton;
    private readonly TextComponent _bgmVol;
    private readonly TextComponent _sfxVol;
    private readonly TextComponent _stereoSplit;
    
    public SettingsPanel(Game game) : base(game)
    {
        Components.Add(_scaleButton = new Button(Game, new Rectangle(108, 87,103, 16)));
        Components.Add(
            _fullScreenButton = new Button(Game, new Rectangle(108, 109, 103, 16),
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/WindowMode", new Vector2(108, 109), 3)
                { DefaultRectangle = new Rectangle(0, 0, 103, Configs.Fullscreen ? 16 : 0) }
            ));
        Components.Add(_bgmVolUpButton = new Button(Game, new Rectangle(108, 132, 10, 10)));
        Components.Add(_bgmVolDownButton = new Button(Game, new Rectangle(134, 132, 10, 10)));
        Components.Add(
            _bgmVol = new TextComponent(Game, Statics.BoldFont, $"{Configs.MusicVolume:00}", new Vector2(118, 133), 3)
            );
        Components.Add(_sfxVolUpButton = new Button(Game, new Rectangle(108, 155, 10, 10)));
        Components.Add(_sfxVolDownButton = new Button(Game, new Rectangle(134, 155, 10, 10)));
        Components.Add(
            _sfxVol = new TextComponent(Game, Statics.BoldFont, $"{Configs.SfxVolume:00}", new Vector2(118, 156), 3)
            );
        Components.Add(_stereoSplitUpButton = new Button(Game, new Rectangle(108, 179, 10, 10)));
        Components.Add(_stereoSplitDownButton = new Button(Game, new Rectangle(142, 179, 10, 10)));
        Components.Add(
            _stereoSplit = new TextComponent(Game, Statics.BoldFont, $"{Configs.StereoSeparation:00}0", new Vector2(118, 180), 3)
            );
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));

        _scaleButton.LeftClicked += Scale;
        _fullScreenButton.LeftClicked += Fullscreen;
        _bgmVolUpButton.LeftClicked += BgmVolUp;
        _bgmVolDownButton.LeftClicked += BgmVolDown;
        _sfxVolUpButton.LeftClicked += SfxVolUp;
        _sfxVolDownButton.LeftClicked += SfxVolDown;
        _stereoSplitUpButton.LeftClicked += StereoSplitUp;
        _stereoSplitDownButton.LeftClicked += StereoSplitDown;
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

    private void BgmVolUp(object s, EventArgs e) => _bgmVol.Text = $"{++Configs.MusicVolume:00}";

    private void BgmVolDown(object s, EventArgs e) => _bgmVol.Text = $"{--Configs.MusicVolume:00}";

    private void SfxVolUp(object s, EventArgs e) => _sfxVol.Text = $"{++Configs.SfxVolume:00}";

    private void SfxVolDown(object s, EventArgs e) => _sfxVol.Text = $"{--Configs.SfxVolume:00}";

    private void StereoSplitUp(object s, EventArgs e) => _stereoSplit.Text = $"{++Configs.StereoSeparation:00}0";

    private void StereoSplitDown(object s, EventArgs e) => _stereoSplit.Text = $"{--Configs.StereoSeparation:00}0";

    private void Back(object s, EventArgs e) => SwitchState(new MainPanel(Game));

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/WindowMode");
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        _scaleButton.LeftClicked -= Scale;
        _fullScreenButton.LeftClicked -= Fullscreen;
        _bgmVolUpButton.LeftClicked -= BgmVolUp;
        _bgmVolDownButton.LeftClicked -= BgmVolDown;
        _sfxVolUpButton.LeftClicked -= SfxVolUp;
        _sfxVolDownButton.LeftClicked -= SfxVolDown;
        _stereoSplitUpButton.LeftClicked -= StereoSplitUp;
        _stereoSplitDownButton.LeftClicked -= StereoSplitDown;
        _backButton.LeftClicked -= Back;
        
        base.Dispose(disposing);
    }
}