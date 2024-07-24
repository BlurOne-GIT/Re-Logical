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
    private DefaultPanel _defaultPanel;
    private SettingsPanel _settingsPanel;
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
        
        _background = new SimpleImage(Game, Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Titlescreen"), Vector2.Zero, 0);
        Components.Add(_background);
        
        _defaultPanel = new DefaultPanel(Game);
        Components.Add(_defaultPanel);
        
        _settingsPanel = new SettingsPanel(Game);
        _settingsPanel.Enabled = false;
        _settingsPanel.Visible = false;
        Components.Add(_settingsPanel);
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
                    ButtonSubscriber();
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
        _defaultPanel.SettingsButton.LeftClicked -= Settings;
        _defaultPanel.StartButton.LeftClicked -= StartGame;
        _settingsPanel.BackButton.LeftClicked -= Back;
        _defaultPanel.Dispose();
        _settingsPanel.Dispose();
        base.Dispose(disposing);
    }
    #endregion

    #region Methods
    private void ButtonSubscriber()
    {
        _defaultPanel.SettingsButton.LeftClicked += Settings;
        _defaultPanel.StartButton.LeftClicked += StartGame;
        _settingsPanel.BackButton.LeftClicked += Back;
    }
    
    private void Settings(object s, EventArgs e)
    {
        _defaultPanel.Enabled = false;
        _defaultPanel.Visible = false;
        _settingsPanel.Enabled = true;
        _settingsPanel.Visible = true;
    }
    
    private void StartGame(object s, EventArgs e)
    {
        if (Configs.Stage is 0)
            Configs.Stage = 1;
        MediaPlayer.Stop();
        _state = States.FadeOut;
        _defaultPanel.Enabled = false;
    }
    
    private void Back(object s, EventArgs e)
    {
        _settingsPanel.Enabled = false;
        _settingsPanel.Visible = false;
        _defaultPanel.Enabled = true;
        _defaultPanel.Visible = true;
    }
    #endregion
}