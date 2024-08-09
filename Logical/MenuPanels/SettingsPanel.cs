using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class SettingsPanel : MenuPanel
{
    private readonly ClickableArea _videoButton;
    private readonly ClickableArea _audioButton;
    private readonly ClickableArea _fixesButton;
    private readonly ClickableArea _backButton;
    
    public SettingsPanel(Game game) : base(game)
    {
        Components.Add(_videoButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), false));
        Components.Add(_audioButton = new ClickableArea(Game, new Rectangle(108, 109, 103, 16), false));
        Components.Add(_fixesButton = new ClickableArea(Game, new Rectangle(108, 132, 103, 16), false));
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), false));
        
        _videoButton.LeftButtonDown += Video;
        _audioButton.LeftButtonDown += Audio;
        _fixesButton.LeftButtonDown += Fixes;
        _backButton.LeftButtonDown += Back;
    }

    private void Video(object s, EventArgs e) => SwitchState(new VideoPanel(Game));
    
    private void Audio(object s, EventArgs e) => SwitchState(new AudioPanel(Game));
    
    private void Fixes(object s, EventArgs e) => SwitchState(new FixesPanel(Game));
    
    private void Back(object s, EventArgs e) => SwitchState(new MainPanel(Game));

    protected override void Dispose(bool disposing)
    {
        _videoButton.LeftButtonDown -= Video;
        _audioButton.LeftButtonDown -= Audio;
        _fixesButton.LeftButtonDown -= Fixes;
        _backButton.LeftButtonDown -= Back;
        
        base.Dispose(disposing);
    }
}