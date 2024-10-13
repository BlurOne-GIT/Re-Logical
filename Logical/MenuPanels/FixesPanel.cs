using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.MenuPanels;

public class FixesPanel : MenuPanel
{
    private readonly ClickableArea _fidelityLevelButton;
    private readonly SimpleImage _fidelityLevelImage;
    private readonly ClickableArea _fourthPaletteButton;
    private readonly SimpleImage _nffImage;
    private readonly ClickableArea _backButton;
    
    private static Rectangle FidelityLevelRectangle => new(0, (int)Configs.FidelityLevel * 23, 103, 23);
    private static Rectangle NffRectangle => new(Configs.GraphicSet4Remastered ? 0 : 11, 0, 11, 23);
    
    public FixesPanel(Game game) : base(game)
    {
        Components.Add(_fidelityLevelButton =
            new ClickableArea(Game, new Rectangle(108, 87, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
        );
        Components.Add(_fidelityLevelImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/FidelityLevels", new Vector2(108, 87), 3)
                { DefaultSource = FidelityLevelRectangle }
        );
        Components.Add(_fourthPaletteButton =
            new ClickableArea(Game, new Rectangle(108, 109, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
        );
        Components.Add(_nffImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/nff", new Vector2(193, 109), 3)
                { DefaultSource = NffRectangle }
        );
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        
        _fidelityLevelButton.ButtonDown += FidelityLevel;
        _fourthPaletteButton.LeftButtonDown += FourthPaletteFix;
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

    private void FourthPaletteFix(object s, EventArgs e)
    {
        var b = Configs.GraphicSet >= 4;
        if (b)
        {
            Game.Content.UnloadAssets([
                $"{Configs.GraphicSet}/UI/{nameof(FixesPanel)}",
                $"{Configs.GraphicSet}/UI/FidelityLevels",
                $"{Configs.GraphicSet}/UI/nff"
            ]);
        }
        
        Configs.GraphicSet4Remastered ^= true;
        _nffImage.DefaultSource = NffRectangle;
        
        if (!b) return;
        
        PanelBackground.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/{nameof(FixesPanel)}");
        _fidelityLevelImage.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/FidelityLevels");
        _nffImage.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/nff");
    }
    
    protected override void Dispose(bool disposing)
    {
        _fidelityLevelButton.ButtonDown  -= FidelityLevel;
        _fourthPaletteButton.LeftButtonDown -= FourthPaletteFix;
        _backButton.LeftButtonDown -= Back;
        
        _fidelityLevelButton.RightButtonDown -= PlaySfx;

        base.Dispose(disposing);
    }
}