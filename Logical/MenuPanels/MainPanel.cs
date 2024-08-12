using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;
using NativeFileDialogSharp;

namespace Logical.MenuPanels;

public class MainPanel : MenuPanel
{
    private readonly ClickableArea _ownSetButton;
    private readonly ClickableArea _passwordButton;
    private readonly ClickableArea _aboutButton;
    private readonly ClickableArea _settingsButton;
    public readonly ClickableArea GraphicsSetButton;
    public readonly ClickableArea StartButton;

    private readonly SimpleImage _levelset;
    private readonly ClickableArea _standardLevelsetButton;
    private readonly ClickableArea _customLevelsetButton;
    private readonly ClickableArea _cancelLeveldiskButton;
    private readonly ClickableArea _selectLeveldiskButton;
    private readonly TextComponent _leveldiskErrorText;
    
    public MainPanel(Game game) : base(game)
    {
        Components.Add(_ownSetButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), false));
        Components.Add(_passwordButton = new ClickableArea(Game, new Rectangle(108, 109, 103, 16), false));
        Components.Add(_aboutButton = new ClickableArea(Game, new Rectangle(108, 132, 103, 16), false));
        Components.Add(_settingsButton = new ClickableArea(Game, new Rectangle(108, 155, 103, 16), false));
        Components.Add(GraphicsSetButton = new ClickableArea(Game, new Rectangle(108, 179, 103, 16), false));
        Components.Add(StartButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), false));
        Components.Add(_levelset =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/Levelset", new Vector2(7, 87), 4)
                { Enabled = false, Visible = false }
        );
        Components.Add(_standardLevelsetButton =
            new ClickableArea(Game, new Rectangle(187, 87, 70, 18), false)
                { Enabled = false }
        );
        Components.Add(_customLevelsetButton =
            new ClickableArea(Game, new Rectangle(257, 87, 54, 18), false)
                { Enabled = false }
        );
        Components.Add(_cancelLeveldiskButton =
            new ClickableArea(Game, new Rectangle(203, 87, 54, 18), false)
                { Enabled = false }
        );
        Components.Add(_selectLeveldiskButton =
            new ClickableArea(Game, new Rectangle(257, 87, 54, 18), false)
                { Enabled = false }
        );
        Components.Add(_leveldiskErrorText =
            new TextComponent(Game, Statics.TopazFont, string.Empty, new Vector2(10, 92), 5)
                { Enabled = false, Visible = false, Color = Statics.TopazColor, Scale = new Vector2(1f, .5f)}
        );
        
        _ownSetButton.LeftButtonDown += OwnSet;
        _passwordButton.LeftButtonDown += Password;
        _aboutButton.LeftButtonDown += About;
        _settingsButton.LeftButtonDown += Settings;
        GraphicsSetButton.ButtonDown += GraphicSet;

        _standardLevelsetButton.LeftButtonDown += StandardLevelset;
        _customLevelsetButton.LeftButtonDown += CustomLevelset;
        _cancelLeveldiskButton.LeftButtonDown += LevelsetComplete;
        _selectLeveldiskButton.LeftButtonDown += SelectLeveldisk;
        
        GraphicsSetButton.RightButtonDown += PlaySfx;
    }

    #region PanelButtons
    private void OwnSet(object s, EventArgs e)
    {
        _levelset.DefaultSource = new Rectangle(0, 0, 304, 18);
        _levelset.Visible = _standardLevelsetButton.Enabled = _customLevelsetButton.Enabled = true;
        _ownSetButton.Enabled = _passwordButton.Enabled = _aboutButton.Enabled = _settingsButton.Enabled = GraphicsSetButton.Enabled = StartButton.Enabled = false;
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
        Game.Content.UnloadAssets(new []
        {
            $"{Configs.GraphicSet}/UI/{nameof(MainPanel)}",
            $"{Configs.GraphicSet}/UI/Levelset"
        });
        if (e.HasFlag(MouseButtons.LeftButton))
            ++Configs.GraphicSet;
        else
            --Configs.GraphicSet;
        PanelBackground.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/{nameof(MainPanel)}");
        _levelset.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/Levelset");
        _leveldiskErrorText.Color = Statics.TopazColor;
    }
    #endregion
    
    #region LevelsetMethods
    private void CustomLevelset(object sender, EventArgs e)
    {
        _levelset.DefaultSource = new Rectangle(0, 18, 304, 18);
        _standardLevelsetButton.Enabled = _customLevelsetButton.Enabled = false;
        _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = true;
    }

    private void StandardLevelset(object sender, EventArgs e)
    {
        Statics.Set = Statics.StandardSet;
        LevelsetComplete();
    }
    
    private async void SelectLeveldisk(object sender, EventArgs e)
    {
        var mouseHelper = Game.Services.GetService<MouseHelper>();
        mouseHelper.ButtonDown += BeepOnFileSelect;
        Statics.ShowCursor = _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = false;
        var result = await Task.Run(() => Dialog.FileOpen("dat"));
        mouseHelper.ButtonDown -= BeepOnFileSelect;
        Statics.ShowCursor = _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = true;
        
        if (result.IsOk)
            Statics.Set = result.Path;
        if (!result.IsError)
        {
            LevelsetComplete();
            return;
        }
        
        _levelset.DefaultSource = new Rectangle(0, 36, 304, 18);
        _leveldiskErrorText.Text = result.ErrorMessage;
        _leveldiskErrorText.Visible = true;
    }
    
    private void BeepOnFileSelect(object sender, MouseButtons e) => Console.Beep();

    private void LevelsetComplete(object sender = null, EventArgs e = null)
    {
        _levelset.Visible               =
        _standardLevelsetButton.Enabled =
        _customLevelsetButton.Enabled   =
        _cancelLeveldiskButton.Enabled  =
        _selectLeveldiskButton.Enabled  =
        _leveldiskErrorText.Visible     = false;
        
        _ownSetButton.Enabled     =
        _passwordButton.Enabled   =
        _aboutButton.Enabled      =
        _settingsButton.Enabled   =
        GraphicsSetButton.Enabled =
        StartButton.Enabled       = true;
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        _ownSetButton.LeftButtonDown -= OwnSet;
        _passwordButton.LeftButtonDown -= Password;
        _aboutButton.LeftButtonDown -= About;
        _settingsButton.LeftButtonDown -= Settings;
        GraphicsSetButton.ButtonDown -= GraphicSet;
        
        GraphicsSetButton.RightButtonDown -= PlaySfx;
        
        _standardLevelsetButton.LeftButtonDown -= StandardLevelset;
        _customLevelsetButton.LeftButtonDown -= CustomLevelset;
        
        base.Dispose(disposing);
    }
}