using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.MenuPanels;

public class MainPanel : MenuPanel
{
    private readonly Button _ownSetButton;
    private readonly Button _passwordButton;
    private readonly Button _aboutButton;
    private readonly Button _settingsButton;
    public readonly Button GraphicsSetButton;
    public readonly Button StartButton;
    
    public MainPanel(Game game) : base(game)
    {
        Components.Add(_ownSetButton = new Button(Game, new Rectangle(108, 87, 103, 16)));
        Components.Add(_passwordButton = new Button(Game, new Rectangle(108, 109, 103, 16)));
        Components.Add(_aboutButton = new Button(Game, new Rectangle(108, 132, 103, 16)));
        Components.Add(_settingsButton = new Button(Game, new Rectangle(108, 155, 103, 16)));
        Components.Add(GraphicsSetButton = new Button(Game, new Rectangle(108, 179, 103, 16)));
        Components.Add(StartButton = new Button(Game, new Rectangle(108, 201, 103, 16)));
        
        _ownSetButton.LeftClicked += OwnSet;
        _passwordButton.LeftClicked += Password;
        _aboutButton.LeftClicked += About;
        _settingsButton.LeftClicked += Settings;
        GraphicsSetButton.LeftClicked += GraphicSet;
        GraphicsSetButton.RightClicked += GraphicSet;
        
        GraphicsSetButton.RightClicked += PlaySfx;
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

    private void GraphicSet(object s, ButtonEventArgs e)
    {
        throw new NotImplementedException("Missing graphic sets.");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/{nameof(MainPanel)}");
        if (e.Button is "RightButton")
            --Configs.GraphicSet;
        else
            ++Configs.GraphicSet;
        PanelBackground.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/{nameof(MainPanel)}");
    }

    protected override void Dispose(bool disposing)
    {
        _ownSetButton.LeftClicked -= OwnSet;
        _passwordButton.LeftClicked -= Password;
        _aboutButton.LeftClicked -= About;
        _settingsButton.LeftClicked -= Settings;
        GraphicsSetButton.LeftClicked -= GraphicSet;
        
        GraphicsSetButton.RightClicked -= PlaySfx;
        
        base.Dispose(disposing);
    }
}