using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MmgEngine;

namespace Logical.States;

public class TitleState : GameState
{
    public TitleState(Game game) : base(game) => Statics.ShowCursor = false;

    #region Fields

    private bool _isEnding;
    private Song _titel;
    private SimpleImage _background;
    private const int MusicTransitionTime = 8000;
    private const int SilenceTime = 8720;
    private int _transitionCounter;
    #endregion

    #region Default Methods

    protected override void LoadContent()
    {
        _titel = Game.Content.Load<Song>("Titel");
        _background = new SimpleImage(Game, "Credit", new Vector2(0f, 28f), 0);
        Components.Add(_background);
        Configs.MusicVolume = Configs.MusicVolume;
        MediaPlayer.Play(_titel);
        MediaPlayer.MediaStateChanged += EndCaller;
    }

    public override void Update(GameTime gameTime)
    {
        if (!_isEnding) return;
        
        if (_transitionCounter < MusicTransitionTime)
            MediaPlayer.Volume = MathHelper.LerpPrecise(MathF.Pow(Configs.MusicVolume * 0.1f, 2), 0f, _transitionCounter / (float) MusicTransitionTime);
        else
        {
            Statics.BackdropOpacity = 1;
            MediaPlayer.Stop();
            MediaPlayer.Volume = Configs.MusicVolume * 0.1f;
        }
        
        _transitionCounter += gameTime.ElapsedGameTime.Milliseconds;
        
        if (_transitionCounter >= SilenceTime)
            SwitchState(new MenuState(Game));
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset("Titel");
        Game.Content.UnloadAsset("Credit");
    }

    public override void HandleInput(object s, InputKeyEventArgs e) => EndCaller(s, e);
    public override void HandleInput(object s, ButtonEventArgs e) => EndCaller(s, e);
    #endregion

    #region Custom Methods
    private void EndCaller(object s, object e)
    {
        Game.Window.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        MediaPlayer.MediaStateChanged -= EndCaller;
        if (Configs.MusicVolume is 0)
            _transitionCounter = MusicTransitionTime;
        _isEnding = true;
    }
    #endregion
}