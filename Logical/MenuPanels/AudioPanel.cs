using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class AudioPanel : MenuPanel
{
    private readonly ClickableArea _bgmVolUpButton;
    private readonly ClickableArea _bgmVolDownButton;
    private readonly ClickableArea _sfxVolUpButton;
    private readonly ClickableArea _sfxVolDownButton;
    private readonly ClickableArea _stereoSplitUpButton;
    private readonly ClickableArea _stereoSplitDownButton;
    private readonly ClickableArea _backButton;
    private readonly TextComponent _bgmVol;
    private readonly TextComponent _sfxVol;
    private readonly TextComponent _stereoSplit;
    
    public AudioPanel(Game game) : base(game)
    {
        Components.Add(_bgmVolUpButton = new ClickableArea(Game, new Rectangle(161, 89, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_bgmVolDownButton = new ClickableArea(Game, new Rectangle(187, 89, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(
            _bgmVol = new TextComponent(Game, Statics.TopazFont, $"{Configs.MusicVolume:00}", new Vector2(171, 90), 3)
            {
                Scale = new Vector2(1f, .5f),
                Color = Statics.TopazColor
            }
        );
        Components.Add(_sfxVolUpButton = new ClickableArea(Game, new Rectangle(161, 111, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_sfxVolDownButton = new ClickableArea(Game, new Rectangle(187, 111, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(
            _sfxVol = new TextComponent(Game, Statics.TopazFont, $"{Configs.SfxVolume:00}", new Vector2(171, 112), 3)
            {
                Scale = new Vector2(1f, .5f),
                Color = Statics.TopazColor
            }
        );
        Components.Add(_stereoSplitUpButton = new ClickableArea(Game, new Rectangle(157, 134, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_stereoSplitDownButton = new ClickableArea(Game, new Rectangle(191, 134, 10, 10), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(
            _stereoSplit = new TextComponent(Game, Statics.TopazFont, $"{Configs.StereoSeparation:00}0", new Vector2(167, 135), 3)
            {
                Scale = new Vector2(1f, .5f),
                Color = Statics.TopazColor
            }
        );
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        
        _bgmVolUpButton.LeftButtonDown += BgmVolUp;
        _bgmVolDownButton.LeftButtonDown += BgmVolDown;
        _sfxVolUpButton.LeftButtonDown += SfxVolUp;
        _sfxVolDownButton.LeftButtonDown += SfxVolDown;
        _stereoSplitUpButton.LeftButtonDown += StereoSplitUp;
        _stereoSplitDownButton.LeftButtonDown += StereoSplitDown;
        _backButton.LeftButtonDown += Back;
    }

    private void BgmVolUp(object s, EventArgs e) => _bgmVol.Text = $"{++Configs.MusicVolume:00}";

    private void BgmVolDown(object s, EventArgs e) => _bgmVol.Text = $"{--Configs.MusicVolume:00}";

    private void SfxVolUp(object s, EventArgs e) => _sfxVol.Text = $"{++Configs.SfxVolume:00}";

    private void SfxVolDown(object s, EventArgs e) => _sfxVol.Text = $"{--Configs.SfxVolume:00}";

    private void StereoSplitUp(object s, EventArgs e) => _stereoSplit.Text = $"{++Configs.StereoSeparation:00}0";

    private void StereoSplitDown(object s, EventArgs e) => _stereoSplit.Text = $"{--Configs.StereoSeparation:00}0";

    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));
    
    protected override void Dispose(bool disposing)
    {
        _bgmVolUpButton.LeftButtonDown -= BgmVolUp;
        _bgmVolDownButton.LeftButtonDown -= BgmVolDown;
        _sfxVolUpButton.LeftButtonDown -= SfxVolUp;
        _sfxVolDownButton.LeftButtonDown -= SfxVolDown;
        _stereoSplitUpButton.LeftButtonDown -= StereoSplitUp;
        _stereoSplitDownButton.LeftButtonDown -= StereoSplitDown;
        _backButton.LeftButtonDown -= Back;
        base.Dispose(disposing);
    }
}