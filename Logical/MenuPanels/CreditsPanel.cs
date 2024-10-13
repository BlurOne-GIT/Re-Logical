using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class CreditsPanel : MenuPanel
{
    private readonly ClickableArea _originalButton;
    private readonly ClickableArea _backButton;
    
    public CreditsPanel(Game game) : base(game)
    {
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        _backButton.LeftButtonDown += Back;
    }

    private void Back(object s, EventArgs e) => SwitchState(new InfoPanel(Game));
    
    protected override void Dispose(bool disposing)
    {
        _backButton.LeftButtonDown -= Back;

        base.Dispose(disposing);
    }
}