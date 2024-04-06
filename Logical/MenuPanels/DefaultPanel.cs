using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.MenuPanels;

public class DefaultPanel : MenuPanel
{
    private readonly Button _ownSetButton;
    private readonly Button _passwordButton;
    private readonly Button _aboutButton;
    public readonly Button SettingsButton;
    private readonly Button _graphicsSetButton;
    public readonly Button StartButton;
    
    public DefaultPanel(Game game) : base(game)
    {
        _ownSetButton = new Button(Game, new Rectangle(108, 87, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/OwnSet"), new Vector2(108, 87), 3));
        _passwordButton = new Button(Game, new Rectangle(108, 109, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Password"), new Vector2(108, 109), 3));
        _aboutButton = new Button(Game, new Rectangle(108, 132, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/About"), new Vector2(108, 132), 3));
        SettingsButton = new Button(Game, new Rectangle(108, 155, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Settings"), new Vector2(108, 155), 3));
        _graphicsSetButton = new Button(Game, new Rectangle(108, 179, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/GraphicSet"), new Vector2(108, 179), 3));
        StartButton = new Button(Game, new Rectangle(108, 201, 103, 16), new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/StartGame"), new Vector2(108, 201), 3));
        
        _ownSetButton.LeftClicked += PlaySfx;
        _passwordButton.LeftClicked += PlaySfx;
        _aboutButton.LeftClicked += PlaySfx;
        SettingsButton.LeftClicked += PlaySfx;
        _graphicsSetButton.LeftClicked += PlaySfx;
        StartButton.LeftClicked += PlaySfx;
        
        _ownSetButton.LeftClicked += OwnSet;
        _passwordButton.LeftClicked += Password;
        _aboutButton.LeftClicked += About;
        _graphicsSetButton.LeftClicked += GraphicSet;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _ownSetButton.Draw(gameTime);
        _passwordButton.Draw(gameTime);
        _aboutButton.Draw(gameTime);
        SettingsButton.Draw(gameTime);
        _graphicsSetButton.Draw(gameTime);
        StartButton.Draw(gameTime);
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _ownSetButton.Update(gameTime);
        _passwordButton.Update(gameTime);
        _aboutButton.Update(gameTime);
        SettingsButton.Update(gameTime);
        _graphicsSetButton.Update(gameTime);
        StartButton.Update(gameTime);
    }
    
    private void OwnSet(object s, EventArgs e)
    {
        throw new NotImplementedException();
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
                Configs.Stage = Convert.ToInt32(read[..(read.Length < 12 ? 1 : 2)]);
            } catch (FormatException)
            {
                throw new NotImplementedException();
            }
            Console.WriteLine("NEW LEVEL: " + Configs.Stage);
            return;
        }

        int search = new Lexer(Game).GetLevelNumber(read);
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
        throw new NotImplementedException();
    }
    
    private void GraphicSet(object s, EventArgs e)
    {
        throw new NotImplementedException();
    }
    
    protected override void OnEnableChanged(object s, EventArgs e)
        => _ownSetButton.Enabled = _passwordButton.Enabled = _aboutButton.Enabled = SettingsButton.Enabled = _graphicsSetButton.Enabled = StartButton.Enabled = Enabled;

    protected override void Dispose(bool disposing)
    {
        _ownSetButton.LeftClicked -= PlaySfx;
        _passwordButton.LeftClicked -= PlaySfx;
        _aboutButton.LeftClicked -= PlaySfx;
        SettingsButton.LeftClicked -= PlaySfx;
        _graphicsSetButton.LeftClicked -= PlaySfx;
        StartButton.LeftClicked -= PlaySfx;
        
        _ownSetButton.LeftClicked -= OwnSet;
        _passwordButton.LeftClicked -= Password;
        _aboutButton.LeftClicked -= About;
        _graphicsSetButton.LeftClicked -= GraphicSet;
        
        _ownSetButton.Dispose();
        _passwordButton.Dispose();
        _aboutButton.Dispose();
        SettingsButton.Dispose();
        _graphicsSetButton.Dispose();
        StartButton.Dispose();
        base.Dispose(disposing);
    }
}