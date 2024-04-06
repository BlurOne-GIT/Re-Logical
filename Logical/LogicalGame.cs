using System;
using Logical.States;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MmgEngine;

namespace Logical;

public class LogicalGame : EngineGame
{
#region Instances
    private Texture2D _cursorTexture;
    private Texture2D _backdropTexture;
    private Vector2 _backdropSize;
    #if DEBUG
    private readonly string _versionString = ((AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0]).InformationalVersion;
    private TextComponent _version;
    #endif
#endregion

#region Default Methods
    public LogicalGame()
    {
        Statics.Initialize(Content);
        IsMouseVisible = false;
        Window.AllowAltF4 = true;
        Window.AllowUserResizing = false;
        Window.IsBorderless = false;
        Configs.Initialize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        ReloadScale(null, EventArgs.Empty);
        if (Configs.Fullscreen)
            Graphics.ToggleFullScreen();
        
        Configs.ResolutionChanged += ReloadScale;
        Configs.FullscreenChanged += Fullscreen;
        Configs.MusicVolumeChanged += UpdateVolume;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        // TODO: use this.Content to load your game content here
        MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2);
        MediaPlayer.IsMuted = _m(Configs.MusicVolume);
        
        _cursorTexture = Content.Load<Texture2D>("Cursor");
        _backdropTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
        _backdropTexture.SetData(new[] { Color.Black });
        _backdropSize = new Vector2(Configs.NativeWidth, Configs.NativeHeight);
        Statics.LoadFonts();
        #if DEBUG
        _version = new TextComponent(this, Statics.LightFont, _versionString.ToUpper().Replace('.', '_'), new Vector2(0, 248), 1);
        #endif
        
        SwitchGameState(new TitleState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        Input.UpdateMouseInput(Mouse.GetState());
        CurrentGameState.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(new Vector3(EngineStatics.Scale, 0f))/*ViewportMatrix*/);
        CurrentGameState.Draw(gameTime);
        if (Statics.ShowCursor)
            SpriteBatch.Draw(
                _cursorTexture,
                Input.MousePoint.ToVector2(),
                null,
                Color.White,
                0,
                new Vector2(7, 7),
                1f,
                SpriteEffects.None,
                1f
            );
        if (Statics.BackdropOpacity > 0)
            SpriteBatch.Draw(
                _backdropTexture,
                Vector2.Zero,
                null,
                Color.White * Statics.BackdropOpacity,
                0f,
                Vector2.Zero,
                _backdropSize,
                SpriteEffects.None,
                1f
                );
        #if DEBUG
        _version.Draw(gameTime);
        #endif
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        Configs.CloseFile();
        Configs.ResolutionChanged -= ReloadScale;
        Configs.FullscreenChanged -= Fullscreen;
        Configs.MusicVolumeChanged -= UpdateVolume;
        CurrentGameState.OnStateSwitched -= OnStateSwitched;
        Activated -= Statics.Focus;
        Deactivated -= Statics.UnFocus;
        base.OnExiting(sender, args);
    }
#endregion

#region Custom Methods

    private void OnStateSwitched(object s, GameState e) => SwitchGameState(e);

    private void ReloadScale(object s, EventArgs e)
    {
        if (Configs.Fullscreen)
        {
            Graphics.PreferredBackBufferWidth = Configs.Width;
            Graphics.PreferredBackBufferHeight = Configs.Height;
        }
        else
        {
            Graphics.PreferredBackBufferWidth = Configs.NativeWidth * Configs.Scale;
            Graphics.PreferredBackBufferHeight = Configs.NativeHeight * Configs.Scale;
        }
        Graphics.ApplyChanges();
    }

    private void Fullscreen(object s, EventArgs e) => Graphics.ToggleFullScreen();

    private void UpdateVolume(object s, EventArgs e) {MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2); MediaPlayer.IsMuted = _m(Configs.MusicVolume);}
    
    private readonly Func<int, bool> _m = x => x == 0;
#endregion
}
