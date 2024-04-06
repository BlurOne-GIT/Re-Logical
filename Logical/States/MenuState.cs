using System;
using System.Threading.Tasks;
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
    #endregion

    #region Default Methods
    public override void LoadContent()
    {
        _choose = Game.Content.Load<Song>("Choose Music");
        _beginSfx = Game.Content.Load<SoundEffect>("1Success"); // DEBUG //
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
        
        FadeIn();
    }

    /*public override void Update(GameTime gameTime)
    {
        
    }*/

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        if (e.Key == Keys.Escape) Game.Exit();
    }

    public override void UnloadContent()
    {
        Game.Content.UnloadAsset("Choose Music");
        Game.Content.UnloadAsset("1Success"); // DEBUG //
        Game.Content.UnloadAsset("Menu Button");
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
    
    private async void FadeIn()
    {
        await Task.Delay(660);
        for (float i = 0; i < 1; i += 0.05f)
        {
            Statics.Opacity = i;
            await Task.Delay(14);
        }
        Statics.Opacity = 1;
        ButtonSubscriber();
    }
    
    private async void FadeOut()
    {
        MediaPlayer.Stop();
        for (float i = 1; i > 0; i -= 0.05f)
        {
            Statics.Opacity = i;
            await Task.Delay(14);
        }
        Statics.Opacity = 0;
        _beginSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(660);
        SwitchState(new LoadingState(Game));
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
        FadeOut();
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