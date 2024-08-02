using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class SettingsPanel : MenuPanel
{
    private readonly Button _videoButton;
    private readonly Button _audioButton;
    private readonly Button _fixesButton;
    private readonly Button _backButton;
    
    public SettingsPanel(Game game) : base(game)
    {
        Components.Add(_videoButton = new Button(Game, new Rectangle(108, 87, 103, 16)));
        Components.Add(_audioButton = new Button(Game, new Rectangle(108, 109, 103, 16)));
        Components.Add(_fixesButton = new Button(Game, new Rectangle(108, 132, 103, 16)));
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        
        _videoButton.LeftClicked += Video;
        _audioButton.LeftClicked += Audio;
        _fixesButton.LeftClicked += Fixes;
        _backButton.LeftClicked += Back;
    }

    private void Video(object s, EventArgs e) => SwitchState(new VideoPanel(Game));
    
    private void Audio(object s, EventArgs e) => SwitchState(new AudioPanel(Game));
    
    private void Fixes(object s, EventArgs e) => SwitchState(new FixesPanel(Game));
    
    private void Back(object s, EventArgs e) => SwitchState(new MainPanel(Game));

    protected override void Dispose(bool disposing)
    {
        _videoButton.LeftClicked -= Video;
        _audioButton.LeftClicked -= Audio;
        _fixesButton.LeftClicked -= Fixes;
        _backButton.LeftClicked -= Back;
        
        base.Dispose(disposing);
    }
}