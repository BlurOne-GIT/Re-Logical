using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.MenuPanels;

public class MainPanel : MenuPanel
{
    private readonly ClickableArea _ownSetButton;
    private readonly ClickableArea _passwordButton;
    private readonly ClickableArea _aboutButton;
    private readonly ClickableArea _settingsButton;
    public readonly ClickableArea GraphicsSetButton;
    public readonly ClickableArea StartButton;
    
    public MainPanel(Game game) : base(game)
    {
        Components.Add(_ownSetButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), false));
        Components.Add(_passwordButton = new ClickableArea(Game, new Rectangle(108, 109, 103, 16), false));
        Components.Add(_aboutButton = new ClickableArea(Game, new Rectangle(108, 132, 103, 16), false));
        Components.Add(_settingsButton = new ClickableArea(Game, new Rectangle(108, 155, 103, 16), false));
        Components.Add(GraphicsSetButton = new ClickableArea(Game, new Rectangle(108, 179, 103, 16), false));
        Components.Add(StartButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), false));
        
        _ownSetButton.LeftButtonDown += OwnSet;
        _passwordButton.LeftButtonDown += Password;
        _aboutButton.LeftButtonDown += About;
        _settingsButton.LeftButtonDown += Settings;
        GraphicsSetButton.ButtonDown += GraphicSet;
        
        GraphicsSetButton.RightButtonDown += PlaySfx;
    }

    private void OwnSet(object s, EventArgs e)
    {
        throw new NotImplementedException("Missing inputs and interchangeable sets.");
    }
    
    private void Password(object s, EventArgs e)
    {
        Console.Write("Insert stage key: ");
        var read = Console.ReadLine()?.ToUpper().TrimEnd();
        if (read != null && read.StartsWith("ELO WANTS "))
        {
            read = read.Remove(0, 10);
            try
            {
                Configs.Stage = Convert.ToByte(read);
            } catch (FormatException)
            {
                throw new NotImplementedException();
            }
            Console.WriteLine("NEW LEVEL: " + Configs.Stage);
            return;
        }

        var search = new Lexer(Game).GetLevelNumber(read);
        if (search is 0)
        {
            Console.WriteLine("WRONG PASSWORD");
            return;
        }

        Configs.Stage = search;
        Console.WriteLine("NEW LEVEL: " + Configs.Stage);
    }
    
    private void About(object s, EventArgs e)
    {
        throw new NotImplementedException("Missing about panel.");
        SwitchState(new AboutPanel(Game));
    }
    
    private void Settings(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));

    private void GraphicSet(object s, MouseButtons e)
    {
        throw new NotImplementedException("Missing graphic sets.");
        if ((e & (MouseButtons.MiddleButton | MouseButtons.XButton1 | MouseButtons.XButton2)) != 0) return;
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/{nameof(MainPanel)}");
        if (e.HasFlag(MouseButtons.LeftButton))
            ++Configs.GraphicSet;
        else
            --Configs.GraphicSet;
        PanelBackground.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/{nameof(MainPanel)}");
    }

    protected override void Dispose(bool disposing)
    {
        _ownSetButton.LeftButtonDown -= OwnSet;
        _passwordButton.LeftButtonDown -= Password;
        _aboutButton.LeftButtonDown -= About;
        _settingsButton.LeftButtonDown -= Settings;
        GraphicsSetButton.ButtonDown -= GraphicSet;
        
        GraphicsSetButton.RightButtonDown -= PlaySfx;
        
        base.Dispose(disposing);
    }
}