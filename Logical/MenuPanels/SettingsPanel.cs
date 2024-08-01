using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class SettingsPanel : MenuPanel
{
    private readonly Button _backButton;
    
    public SettingsPanel(Game game) : base(game)
    {
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        _backButton.LeftClicked += Back;
    }

    private void Back(object s, EventArgs e) => SwitchState(new MainPanel(Game));

    protected override void Dispose(bool disposing)
    {
        _backButton.LeftClicked -= Back;
        
        base.Dispose(disposing);
    }
}