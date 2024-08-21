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
    public MenuState(Game game) : base(game)
    {
        Statics.ShowCursor = true;
        _menuManager = new GameStateManager<MenuPanel>(Components);
        _menuManager.Switched += MenuManagerOnSwitched;
        Game.Window.KeyDown += HandleInput;
    }

    #region Fields
    private Song _choose;
    private SoundEffect _beginSfx;
    private SimpleImage _background;
    private readonly GameStateManager<MenuPanel> _menuManager;
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
        
        _menuManager.GameState = new MainPanel(Game) { Enabled = false };
    }
    
    private void MenuManagerOnSwitched(object sender, SwitchingGameStateEventArgs<MenuPanel> e)
    {
        if (e.OldGameState is MainPanel oldMainPanel)
        {
            oldMainPanel.StartButton.LeftButtonDown -= StartGame;
            oldMainPanel.GraphicsSetButton.LeftButtonDown -= GraphicSet;
            oldMainPanel.GraphicsSetButton.RightButtonDown -= GraphicSet;
        }
        else if (e.NewGameState is MainPanel newMainPanel)
        {
            newMainPanel.StartButton.LeftButtonDown += StartGame;
            newMainPanel.GraphicsSetButton.LeftButtonDown += GraphicSet;
            newMainPanel.GraphicsSetButton.RightButtonDown += GraphicSet;
        }
    }

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
                    _menuManager.GameState!.Enabled = true;
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

    private void HandleInput(object s, InputKeyEventArgs e)
    {
        if (_menuManager.GameState is MainPanel && Statics.ShowCursor && e.Key is Keys.Escape)
            Game.Exit();
    }

    protected override void UnloadContent()
    {
        //Game.Content.UnloadAsset("Sfx/1/Success"); // DEBUG //
        Game.Content.UnloadAssets([
            "Choose Music",
            "Sfx/Button",
            $"{Configs.GraphicSet}/UI/MainMenu"
        ]);
    }

    protected override void Dispose(bool disposing)
    {
        Game.Window.KeyDown -= HandleInput;
        _menuManager.GameState = null;
        _menuManager.Switched -= MenuManagerOnSwitched;
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
        _menuManager.GameState!.Enabled = false;
    }

    private void GraphicSet(object s, EventArgs e)
    {
        Game.Content.UnloadAsset($"{Configs.GraphicSet}/UI/MainMenu");
        _background.Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/UI/MainMenu");
    }
    #endregion
}