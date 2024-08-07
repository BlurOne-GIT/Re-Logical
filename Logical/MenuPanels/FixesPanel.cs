using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class FixesPanel : MenuPanel
{
    private readonly Button _fidelityLevelButton;
    private readonly SimpleImage _fidelityLevelImage;
    private readonly Button _backButton;
    
    private static Rectangle FidelityLevelRectangle => new(0, (int)Configs.FidelityLevel * 23, 103, 23);
    
    public FixesPanel(Game game) : base(game)
    {
        Components.Add(_fidelityLevelButton = new Button(Game, new Rectangle(108, 87, 103, 16)));
        Components.Add(_fidelityLevelImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/FidelityLevels", new Vector2(108, 87), 3)
            { DefaultRectangle = FidelityLevelRectangle }
        );
        Components.Add(_backButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        
        _fidelityLevelButton.LeftClicked  += FidelityLevel;
        _fidelityLevelButton.RightClicked += FidelityLevel;
        _backButton.LeftClicked += Back;
    }

    private void Back(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));

    private void FidelityLevel(object s, ButtonEventArgs e)
    {
        if (e.Button is "RightButton")
            --Configs.FidelityLevel;
        else
            ++Configs.FidelityLevel;
        
        _fidelityLevelImage.DefaultRectangle = FidelityLevelRectangle;
    }
    
    protected override void Dispose(bool disposing)
    {
        _fidelityLevelButton.LeftClicked  -= FidelityLevel;
        _fidelityLevelButton.RightClicked -= FidelityLevel;
        _backButton.LeftClicked -= Back;

        base.Dispose(disposing);
    }
}