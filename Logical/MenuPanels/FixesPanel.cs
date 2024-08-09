using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class FixesPanel : MenuPanel
{
    private readonly ClickableArea _fidelityLevelButton;
    private readonly SimpleImage _fidelityLevelImage;
    private readonly ClickableArea _backButton;
    
    private static Rectangle FidelityLevelRectangle => new(0, (int)Configs.FidelityLevel * 23, 103, 23);
    
    public FixesPanel(Game game) : base(game)
    {
        Components.Add(_fidelityLevelButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), false));
        Components.Add(_fidelityLevelImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/FidelityLevels", new Vector2(108, 87), 3)
            { DefaultSource = FidelityLevelRectangle }
        );
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), false));
        
        _fidelityLevelButton.ButtonDown += FidelityLevel;
        _backButton.LeftButtonDown += Back;

        _fidelityLevelButton.RightButtonDown += PlaySfx;
    }

    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));

    private void FidelityLevel(object s, MouseButtons e)
    {
        if (e.HasFlag(MouseButtons.LeftButton))
            ++Configs.FidelityLevel;
        else if (e.HasFlag(MouseButtons.RightButton))
            --Configs.FidelityLevel;
        else
            return;
        
        _fidelityLevelImage.DefaultSource = FidelityLevelRectangle;
    }
    
    protected override void Dispose(bool disposing)
    {
        _fidelityLevelButton.ButtonDown  -= FidelityLevel;
        _backButton.LeftButtonDown -= Back;
        
        _fidelityLevelButton.RightButtonDown -= PlaySfx;

        base.Dispose(disposing);
    }
}