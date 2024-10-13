using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class CreditsPanel : MenuPanel
{
    public readonly ClickableArea OriginalButton;
    private readonly ClickableArea _backButton;
    
    public CreditsPanel(Game game) : base(game)
    {
        Components.Add(
            new TextComponent(Game, Statics.DisplayFont, "DEVELOPER:\n BLURONE!\n\n\nMADE WITH:\n MONOGAME", new Vector2(120, 91), 3)
        );
        Components.Add(OriginalButton = new ClickableArea(Game, new Rectangle(108, 179, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
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