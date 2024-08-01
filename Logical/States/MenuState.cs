using System;
using Logical.MenuPanels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MmgEngine;

namespace Logical.States;

public class MenuState : GameState
{
    public MenuState(Game game) : base(game) => Statics.ShowCursor = true;

    #region Fields
    private Song _choose;
    private SoundEffect _beginSfx;
    private SimpleImage _background;
    private MenuPanel _menuPanel;
    private States _state = States.BlackIn;
    private int _timer;
    private const int BlackTime = 660;
    private const int FadeTime = 280;
    private enum States
    {
        BlackIn,
        FadeIn,
        Standby,
        FadeOut,
        BlackOut
    }
    #endregion

    #region Default Methods
    protected override void LoadContent()
    {
        _choose = Game.Content.Load<Song>("Choose Music");
        _beginSfx = Game.Content.Load<SoundEffect>("Sfx/1/Success"); // DEBUG //
        MediaPlayer.Play(_choose);
        MediaPlayer.IsRepeating = true;
        
        _background = new SimpleImage(Game, $"{Configs.GraphicSet}/UI/MainMenu", new Vector2(39, 28), 0);
        Components.Add(_background);
        
        SwitchMenuPanel(new MainPanel(Game) { Enabled = false });
    }
    
    private void SwitchMenuPanel(MenuPanel newGameState)
    {
        if (_menuPanel is not null)
        {
            Components.Remove(_menuPanel);
            _menuPanel.OnStateSwitched -= OnMenuPanelSwitched;
            if (_menuPanel is MainPanel mainPanel)
            {
                mainPanel.StartButton.LeftClicked -= StartGame;
                mainPanel.GraphicsSetButton.LeftClicked -= GraphicSet;
                mainPanel.GraphicsSetButton.RightClicked -= GraphicSet;
            }
            _menuPanel.Dispose();
        }

        _menuPanel = newGameState;

        Components.Add(_menuPanel);

        _menuPanel.OnStateSwitched += OnMenuPanelSwitched;
        
        if (_menuPanel is not MainPanel mainPanel2) return; // Having to add a 2 to this just makes no sense.
        mainPanel2.StartButton.LeftClicked += StartGame;
        mainPanel2.GraphicsSetButton.LeftClicked += GraphicSet;
        mainPanel2.GraphicsSetButton.RightClicked += GraphicSet;
    }

    private void OnMenuPanelSwitched(object s, GameState e) => SwitchMenuPanel(e as MenuPanel);

    public override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case States.BlackIn:
                if (_timer >= BlackTime)
                {
                    _state = States.FadeIn;
                    _timer = 0;
                }
                break;
            case States.FadeIn:
                Statics.BackdropOpacity = Math.Clamp(1f - _timer / (float) FadeTime, 0f, 1f);
                if (_timer >= FadeTime)
                {
                    _state = States.Standby;
                    _timer = 0;
                    _menuPanel.Enabled = true;
                }
                break;
            case States.FadeOut:
                Statics.BackdropOpacity = Math.Clamp(_timer / (float) FadeTime, 0f, 1f);
                if (_timer >= FadeTime)
                {
                    _state = States.BlackOut;
                    _timer = 0;
                    _beginSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                }
                break;
            case States.BlackOut:
                if (_timer >= BlackTime)
                    SwitchState(new PreviewState(Game));
                break;
        }
        if (_state is not States.Standby)
            _timer += gameTime.ElapsedGameTime.Milliseconds;
        base.Update(gameTime);
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        if (e.Key == Keys.Escape) Game.Exit();
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset("Choose Music");
        //Game.Content.UnloadAsset("Sfx/1/Success"); // DEBUG //
        Game.Content.UnloadAsset("Sfx/Button");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Titlescreen");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/OwnSet");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Password");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/About");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Settings");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/GraphicSet");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/StartGame");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Scale");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Fullscreen");
        Game.Content.UnloadAsset("Plus");
        Game.Content.UnloadAsset("Minus");
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/Back");
    }

    protected override void Dispose(bool disposing)
    {
        if (_menuPanel is MainPanel mainPanel)
        {
            mainPanel.StartButton.LeftClicked -= StartGame;
            mainPanel.GraphicsSetButton.LeftClicked -= GraphicSet;
            mainPanel.GraphicsSetButton.RightClicked -= GraphicSet;
        }
        _menuPanel.OnStateSwitched -= OnMenuPanelSwitched;
        base.Dispose(disposing);
    }
    #endregion

    #region Methods
    private void StartGame(object s, EventArgs e)
    {
        if (Configs.Stage is 0)
            Configs.Stage = 1;
        MediaPlayer.Stop();
        _state = States.FadeOut;
        _menuPanel.Enabled = false;
    }

    private void GraphicSet(object s, EventArgs e)
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/MainMenu");
        _background.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/MainMenu");
    }
    #endregion
}