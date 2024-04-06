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
    public readonly Button BackButton;
    private readonly TextComponent _bgmVol;
    private readonly TextComponent _sfxVol;
    private readonly TextComponent _stereoSplit;
    
    public SettingsPanel(Game game) : base(game)
    {
        _scaleButton = new Button(Game, new Rectangle(108, 87,103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Scale"), new Vector2(108, 87), 3), enabled: false);
        _fullScreenButton = new Button(Game, new Rectangle(108, 109,103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Fullscreen"), new Vector2(108, 109), 3), enabled: false);
        var plus = Game.Content.Load<Texture2D>("Plus");
        var minus = Game.Content.Load<Texture2D>("Minus");
        _bgmVolUpButton = new Button(Game, new Rectangle(108, 132, 10, 10), new SimpleImage(Game, plus, new Vector2(108, 132), 3), enabled: false);
        _bgmVolDownButton = new Button(Game, new Rectangle(134, 132, 10, 10), new SimpleImage(Game, minus, new Vector2(134, 132), 3), enabled: false);
        _bgmVol = new TextComponent(Game, Statics.BoldFont, $"{Configs.MusicVolume:00}", new Vector2(118, 133), 3);
        _sfxVolUpButton = new Button(Game, new Rectangle(108, 155, 10, 10), new SimpleImage(Game, plus, new Vector2(108, 155), 3), enabled: false);
        _sfxVolDownButton = new Button(Game, new Rectangle(134, 155, 10, 10), new SimpleImage(Game, minus, new Vector2(134, 155), 3), enabled: false);
        _sfxVol = new TextComponent(Game, Statics.BoldFont, $"{Configs.SfxVolume:00}", new Vector2(118, 156), 3);
        _stereoSplitUpButton = new Button(Game, new Rectangle(108, 179, 10, 10), new SimpleImage(Game, plus, new Vector2(108, 179), 3), enabled: false);
        _stereoSplitDownButton = new Button(Game, new Rectangle(142, 179, 10, 10), new SimpleImage(Game, minus, new Vector2(142, 179), 3), enabled: false);
        _stereoSplit = new TextComponent(Game, Statics.BoldFont, $"{Configs.StereoSeparation:000}", new Vector2(118, 180), 3);
        BackButton = new Button(Game, new Rectangle(108, 201, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Back"), new Vector2(108, 201), 3), enabled: false);
        
        _scaleButton.LeftClicked += PlaySfx;
        _fullScreenButton.LeftClicked += PlaySfx;
        _bgmVolUpButton.LeftClicked += PlaySfx;
        _bgmVolDownButton.LeftClicked += PlaySfx;
        _sfxVolUpButton.LeftClicked += PlaySfx;
        _sfxVolDownButton.LeftClicked += PlaySfx;
        _stereoSplitUpButton.LeftClicked += PlaySfx;
        _stereoSplitDownButton.LeftClicked += PlaySfx;
        BackButton.LeftClicked += PlaySfx;
        
        _scaleButton.LeftClicked += Scale;
        _fullScreenButton.LeftClicked += Fullscreen;
        _bgmVolUpButton.LeftClicked += BgmVolUp;
        _bgmVolDownButton.LeftClicked += BgmVolDown;
        _sfxVolUpButton.LeftClicked += SfxVolUp;
        _sfxVolDownButton.LeftClicked += SfxVolDown;
        _stereoSplitUpButton.LeftClicked += StereoSplitUp;
        _stereoSplitDownButton.LeftClicked += StereoSplitDown;
    }

    protected override void OnEnableChanged(object s, EventArgs e) =>
        _scaleButton.Enabled =
            _fullScreenButton.Enabled =
                _bgmVolUpButton.Enabled =
                    _bgmVolDownButton.Enabled =
                        _sfxVolUpButton.Enabled =
                            _sfxVolDownButton.Enabled =
                                _stereoSplitUpButton.Enabled =
                                    _stereoSplitDownButton.Enabled =
                                        BackButton.Enabled =
                                            _bgmVol.Enabled =
                                                _sfxVol.Enabled =
                                                    _stereoSplit.Enabled =
                                                        Enabled;

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _scaleButton.Draw(gameTime);
        _fullScreenButton.Draw(gameTime);
        _bgmVolUpButton.Draw(gameTime);
        _bgmVolDownButton.Draw(gameTime);
        _sfxVolUpButton.Draw(gameTime);
        _sfxVolDownButton.Draw(gameTime);
        _stereoSplitUpButton.Draw(gameTime);
        _stereoSplitDownButton.Draw(gameTime);
        BackButton.Draw(gameTime);
        _bgmVol.Draw(gameTime);
        _sfxVol.Draw(gameTime);
        _stereoSplit.Draw(gameTime);
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _scaleButton.Update(gameTime);
        _fullScreenButton.Update(gameTime);
        _bgmVolUpButton.Update(gameTime);
        _bgmVolDownButton.Update(gameTime);
        _sfxVolUpButton.Update(gameTime);
        _sfxVolDownButton.Update(gameTime);
        _stereoSplitUpButton.Update(gameTime);
        _stereoSplitDownButton.Update(gameTime);
        BackButton.Update(gameTime);
        _bgmVol.Update(gameTime);
        _sfxVol.Update(gameTime);
        _stereoSplit.Update(gameTime);
    }

    private void Scale(object s, EventArgs e)
    {
        if (Configs.Scale != Configs.MaxScale)
            Configs.Scale++;
        else
            Configs.Scale = 1;
    }
    
    private void Fullscreen(object s, EventArgs e) => Configs.Fullscreen ^= true;
    
    private void BgmVolUp(object s, EventArgs e)
    {
        if (Configs.MusicVolume == 10)
            return;

        Configs.MusicVolume++;
        _bgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    
    private void BgmVolDown(object s, EventArgs e)
    {
        if (Configs.MusicVolume == 0)
            return;

        Configs.MusicVolume--;
        _bgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    
    private void SfxVolUp(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 10)
            return;

        Configs.SfxVolume++;
        _sfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    
    private void SfxVolDown(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 0)
            return;

        Configs.SfxVolume--;
        _sfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    
    private void StereoSplitUp(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 100)
            return;

        Configs.StereoSeparation += 10;
        _stereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }
    
    private void StereoSplitDown(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 0)
            return;

        Configs.StereoSeparation -= 10;
        _stereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }

    public new void Dispose()
    {
        _scaleButton.LeftClicked -= PlaySfx;
        _fullScreenButton.LeftClicked -= PlaySfx;
        _bgmVolUpButton.LeftClicked -= PlaySfx;
        _bgmVolDownButton.LeftClicked -= PlaySfx;
        _sfxVolUpButton.LeftClicked -= PlaySfx;
        _sfxVolDownButton.LeftClicked -= PlaySfx;
        _stereoSplitUpButton.LeftClicked -= PlaySfx;
        _stereoSplitDownButton.LeftClicked -= PlaySfx;
        BackButton.LeftClicked -= PlaySfx;
        
        _scaleButton.LeftClicked -= Scale;
        _fullScreenButton.LeftClicked -= Fullscreen;
        _bgmVolUpButton.LeftClicked -= BgmVolUp;
        _bgmVolDownButton.LeftClicked -= BgmVolDown;
        _sfxVolUpButton.LeftClicked -= SfxVolUp;
        _sfxVolDownButton.LeftClicked -= SfxVolDown;
        _stereoSplitUpButton.LeftClicked -= StereoSplitUp;
        _stereoSplitDownButton.LeftClicked -= StereoSplitDown;
        
        _scaleButton.Dispose();
        _fullScreenButton.Dispose();
        _bgmVolUpButton.Dispose();
        _bgmVolDownButton.Dispose();
        _sfxVolUpButton.Dispose();
        _sfxVolDownButton.Dispose();
        _stereoSplitUpButton.Dispose();
        _stereoSplitDownButton.Dispose();
        BackButton.Dispose();
        _bgmVol.Dispose();
        _sfxVol.Dispose();
        _stereoSplit.Dispose();
        base.Dispose();
    }
}