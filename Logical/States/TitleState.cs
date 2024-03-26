using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Logical.States;

public class TitleState : GameState
{
    public TitleState() => Statics.ShowCursor = false;

    #region Fields
    private Song _titel;
    private SimpleImage _background;
    #endregion

    #region Default Methods

    public override void LoadContent(ContentManager content)
    {
        _titel = content.Load<Song>("Titel");
        _background = new SimpleImage(content.Load<Texture2D>("Credit"), Vector2.Zero, 0);
        AddGameObject(_background);
        MediaPlayer.Play(_titel);
        MediaPlayer.MediaStateChanged += EndCaller;
    }

    public override void Update(GameTime gameTime) {}

    public override void UnloadContent(ContentManager content)
    {
        content.UnloadAsset("Titel");
        content.UnloadAsset("Credit");
    }

    public override void HandleInput(object s, InputKeyEventArgs e) => EndScreen();
    public override void HandleInput(object s, ButtonEventArgs e) => EndScreen();
    #endregion

    #region Custom Methods
    private void EndCaller(object s, EventArgs e) => EndScreen();
    private async void EndScreen()
    {
        Input.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        MediaPlayer.MediaStateChanged -= EndCaller;
        for (float i = 1; i > 0; i -= 0.0125f)
        {
            MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2) * i;
            await Task.Delay(100);
        }
        Statics.Opacity = 0;
        MediaPlayer.Stop(); MediaPlayer.Volume = Configs.MusicVolume * 0.1f;
        await Task.Delay(720);
        SwitchState(new MenuState());
    }
    #endregion
}