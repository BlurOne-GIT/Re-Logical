using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
    private readonly ClickableArea _infoButton;
    private readonly ClickableArea _settingsButton;
    public  readonly ClickableArea GraphicsSetButton;
    public  readonly ClickableArea StartButton;

    private readonly SimpleImage   _levelsetImage;
    private readonly ClickableArea _standardLevelsetButton;
    private readonly ClickableArea _customLevelsetButton;
    private readonly ClickableArea _cancelLeveldiskButton;
    private readonly ClickableArea _selectLeveldiskButton;
    private readonly TextComponent _leveldiskErrorText;

    private readonly SimpleImage _passwordImage;
    private readonly TextInput   _passwordInput;
    private readonly TextInput.Caret _passwordCaret;
    
    public MainPanel(Game game) : base(game)
    {
        Components.Add(_ownSetButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_passwordButton = new ClickableArea(Game, new Rectangle(108, 109, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_infoButton = new ClickableArea(Game, new Rectangle(108, 132, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_settingsButton = new ClickableArea(Game, new Rectangle(108, 155, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(GraphicsSetButton = new ClickableArea(Game, new Rectangle(108, 179, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(StartButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        
        Components.Add(_levelsetImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/Levelset", new Vector2(7, 87), 4)
                { Enabled = false, Visible = false }
        );
        Components.Add(_standardLevelsetButton =
            new ClickableArea(Game, new Rectangle(187, 87, 70, 18), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
                { Enabled = false }
        );
        Components.Add(_customLevelsetButton =
            new ClickableArea(Game, new Rectangle(257, 87, 54, 18), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
                { Enabled = false }
        );
        Components.Add(_cancelLeveldiskButton =
            new ClickableArea(Game, new Rectangle(203, 87, 54, 18), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
                { Enabled = false }
        );
        Components.Add(_selectLeveldiskButton =
            new ClickableArea(Game, new Rectangle(257, 87, 54, 18), outsideBehaviour: ClickableArea.OutsideBehaviour.None)
                { Enabled = false }
        );
        Components.Add(_leveldiskErrorText =
            new TextComponent(Game, Statics.TopazFont, string.Empty, new Vector2(10, 92), 5)
                { Enabled = false, Visible = false, Color = Statics.TopazColor, Scale = new Vector2(1f, .5f)}
        );
        
        Components.Add(_passwordImage =
            new SimpleImage(Game, $"{Configs.GraphicSet}/UI/Password", new Vector2(85, 109), 4)
                { Enabled = false, Visible = false }
        );
        Components.Add(_passwordInput =
            new TextInput(Game, Statics.DisplayFont, "", new Vector2(95, 114), 5, false, new Regex("^[A-Z 0-9]{0,15}$"), charFunc: char.ToUpper)
                { Enabled = false, Visible = false }
        );
        Components.Add(_passwordCaret =
            new TextInput.Caret(Game, Statics.DisplayFont.Texture, Vector2.Zero, _passwordInput)
                { DefaultSource = new Rectangle(360, 0, 8, 7) }
        );
        
        _ownSetButton.LeftButtonDown += OwnSet;
        _passwordButton.LeftButtonDown += Password;
        _infoButton.LeftButtonDown += Info;
        _settingsButton.LeftButtonDown += Settings;
        GraphicsSetButton.ButtonDown += GraphicSet;

        _standardLevelsetButton.LeftButtonDown += StandardLevelset;
        _customLevelsetButton.LeftButtonDown += CustomLevelset;
        _cancelLeveldiskButton.LeftButtonDown += LevelsetComplete;
        _selectLeveldiskButton.LeftButtonDown += SelectLeveldisk;
        
        GraphicsSetButton.RightButtonDown += PlaySfx;
    }

    #region PanelButtons
    private bool BunkEnabled
    {
        set => _ownSetButton.Enabled = _passwordButton.Enabled = _infoButton.Enabled = _settingsButton.Enabled = GraphicsSetButton.Enabled = StartButton.Enabled = value;
    }
    
    private void OwnSet(object s, EventArgs e)
    {
        _levelsetImage.DefaultSource = new Rectangle(0, 0, 304, 18);
        _levelsetImage.Visible = _standardLevelsetButton.Enabled = _customLevelsetButton.Enabled = true;
        BunkEnabled = false;
    }

    private void Password(object s, EventArgs e)
    {
        // TODO: replicate feel of input opening
        var mouseHelper = Game.Services.GetService<ClickableWindow>();
        mouseHelper.Enabled = Statics.Cursor.Visible /*Statics.ShowCursor*/ = BunkEnabled = false;
        _passwordInput.Text = string.Empty;
        _passwordInput.CaretIndex = 0;
        _passwordImage.Visible = _passwordInput.Visible = _passwordInput.Enabled = true;
        _passwordInput.Escaped += PasswordEscaped;
        _passwordInput.Returned += PasswordReturned;
    }
    
    private void Info(object s, EventArgs e)
    {
        SwitchState(new InfoPanel(Game));
    }
    
    private void Settings(object s, EventArgs e) => SwitchState(new SettingsPanel(Game));

    private void GraphicSet(object s, MouseButtons e)
    {
        throw new NotImplementedException("Missing graphic sets.");
        /*if ((e & (MouseButtons.MiddleButton | MouseButtons.XButton1 | MouseButtons.XButton2)) != 0) return;
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
        _levelsetImage.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/Levelset");
        _leveldiskErrorText.Color = Statics.TopazColor;*/
    }
    #endregion
    
    #region LevelsetMethods
    private void CustomLevelset(object sender, EventArgs e)
    {
        _levelsetImage.DefaultSource = new Rectangle(0, 18, 304, 18);
        _standardLevelsetButton.Enabled = _customLevelsetButton.Enabled = false;
        _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = true;
    }

    private void StandardLevelset(object sender, EventArgs e)
    {
        Statics.LevelSetPath = Statics.StandardSet;
        LevelsetComplete();
    }
    
    // TODO: extra errors (LevelSet constructor exceptions, selected file is default set)
    private async void SelectLeveldisk(object sender, EventArgs e)
    {
        var mouseHelper = Game.Services.GetService<ClickableWindow>();
        mouseHelper.ButtonDown += BeepOnFileSelect;
        Statics.Cursor.Visible /*Statics.ShowCursor*/ = _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = false;
        var result = await Task.Run(() => Dialog.FileOpen("dat"));
        mouseHelper.ButtonDown -= BeepOnFileSelect;
        Statics.Cursor.Visible /*Statics.ShowCursor*/ = _cancelLeveldiskButton.Enabled = _selectLeveldiskButton.Enabled = true;
        
        if (result.IsOk)
            Statics.LevelSetPath = result.Path;
        if (!result.IsError)
        {
            LevelsetComplete();
            return;
        }
        
        _levelsetImage.DefaultSource = new Rectangle(0, 36, 304, 18);
        _leveldiskErrorText.Text = result.ErrorMessage;
        _leveldiskErrorText.Visible = true;
    }
    
    private static void BeepOnFileSelect(object sender, MouseButtons e) => Console.Beep();

    private void LevelsetComplete(object sender = null, EventArgs e = null)
    {
        _levelsetImage.Visible               =
        _standardLevelsetButton.Enabled =
        _customLevelsetButton.Enabled   =
        _cancelLeveldiskButton.Enabled  =
        _selectLeveldiskButton.Enabled  =
        _leveldiskErrorText.Visible     = false;
        
        BunkEnabled = true;
    }
    #endregion

    #region PasswordMethods
    private void PasswordEscaped(object s = null, EventArgs e = null)
    {
        var mouseHelper = Game.Services.GetService<ClickableWindow>();
        _passwordImage.Visible = _passwordInput.Visible = false;
        _passwordInput.Escaped -= PasswordEscaped;
        _passwordInput.Returned -= PasswordReturned;
        mouseHelper.Enabled = Statics.Cursor.Visible /*Statics.ShowCursor*/ = BunkEnabled = true;
    }
    
    private void PasswordReturned(object s, EventArgs e)
    {
        var read = _passwordInput.Text.TrimEnd();
        if (read.Length is 0)
        {
            PasswordEscaped();
            return;
        }
        
        if (read.StartsWith("ELO WANTS"))
        {
            try
            {
                if (read[9] is not ' ')
                {
                    WrongPassword();
                    return;
                }
                read = read.Remove(0, 10);
                if (read.Length > 2)
                    read = read.Remove(2);
                NewLevel(byte.Parse(read));
            } catch (Exception)
            {
                EnterTheEditor();
            }
            return;
        }

        if (read is "THE FINAL CUT")
        {
            EnterTheEditor();
            return;
        }

        var search = Statics.LevelSet.GetLevelNumber(read);
        NewLevel(search);
    }

    private void NewLevel(byte newLevel)
    {
        if (newLevel is 0)
        {
            WrongPassword();
            return;
        }
        
        Configs.ResetGame(newLevel);
        _passwordInput.Text = $"NEW LEVEL: {Configs.Stage}";
        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromSeconds(1.4), () => PasswordEscaped()));
    }

    private void WrongPassword()
    {
        _passwordInput.Text = " WRONG PASSWORD";
        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromSeconds(1.4), () => PasswordEscaped()));
    }
    
    private void EnterTheEditor()
    {
        throw new NotImplementedException("Level editor not implemented.");
    }
    #endregion
    
    protected override void Dispose(bool disposing)
    {
        _ownSetButton.LeftButtonDown -= OwnSet;
        _passwordButton.LeftButtonDown -= Password;
        _infoButton.LeftButtonDown -= Info;
        _settingsButton.LeftButtonDown -= Settings;
        GraphicsSetButton.ButtonDown -= GraphicSet;
        
        GraphicsSetButton.RightButtonDown -= PlaySfx;
        
        _standardLevelsetButton.LeftButtonDown -= StandardLevelset;
        _customLevelsetButton.LeftButtonDown -= CustomLevelset;
        
        base.Dispose(disposing);
    }
}