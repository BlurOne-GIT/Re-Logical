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
#if DEBUG
    private readonly string _versionString;
    private readonly string _commitString;
    #endif
#endregion

#region Default Methods
    public LogicalGame()
    {
        #if DEBUG
        var fullInfoVersion =
            ((AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute),
                false)[0]).InformationalVersion;

        _versionString = "v" + fullInfoVersion.Split('+')[0];
        _commitString = fullInfoVersion.Split('+')[1][..7];
        #endif
        
        IsMouseVisible = false;
        Window.AllowAltF4 = true;
        Window.AllowUserResizing = false;
        Window.IsBorderless = false;
        TargetElapsedTime = TimeSpan.FromTicks(200000L);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Configs.Initialize(Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width, Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
        ReloadScale(null, EventArgs.Empty);
        if (Configs.Fullscreen)
            Graphics.ToggleFullScreen();
        
        Window.ScreenDeviceNameChanged += OnWindowOnScreenDeviceNameChanged;
        Configs.ResolutionChanged += ReloadScale;
        Configs.FullscreenChanged += Fullscreen;
        Configs.MusicVolumeChanged += UpdateVolume;
        GameStateManager.Switched += OnGameStateSwitch;
        GameStateManager.GameState = new TitleState(this);
        base.Initialize();
    }

    private void OnWindowOnScreenDeviceNameChanged(object s, EventArgs e)
        => Configs.ScreenSize = new Vector2(
            Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
            Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height
            );

    protected override void LoadContent()
    {
        base.LoadContent();

        // TODO: use this.Content to load your game content here
        MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2);
        MediaPlayer.IsMuted = _m(Configs.MusicVolume);
        
        _cursorTexture = Content.Load<Texture2D>("Cursor");
        _backdropTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
        _backdropTexture.SetData([Color.Black]);
        
        Components.Add(
            Statics.Cursor = new Cursor(this) { Enabled = false, Visible = false, DrawOrder = 10, UpdateOrder = 0}
        );
        Components.Add(
            Statics.Backdrop = new SimpleImage(this, _backdropTexture, Vector2.Zero, 10)
                { Enabled = false, Opacity = 0f, Scale = new Vector2(Configs.NativeWidth, Configs.NativeHeight) }
        );
        Statics.LoadFonts(Content);
        #if DEBUG
        Components.Add(
            new TextComponent(this, Statics.TopazFont, _versionString, new Vector2(0, 256), 1, Alignment.BottomLeft)
                { Scale = new Vector2(1f, .5f) }
        );
        Components.Add(
            new TextComponent(this, Statics.TopazFont, _commitString, new Vector2(320, 256), 1, Alignment.BottomRight)
                { Scale = new Vector2(1f, .5f) }
        );
        #endif
    }

    private static void OnGameStateSwitch(object s, SwitchingGameStateEventArgs<GameState> e)
    {
        if (e.OldGameState is not null or TitleState)
            Configs.SaveFile();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        SpriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: ViewportMatrix);
        base.Draw(gameTime);
        /*if (Statics.ShowCursor)
            SpriteBatch.Draw(
                _cursorTexture,
                HoverableArea.MouseVector,
                null,
                Color.White,
                0,
                Statics.CursorTextureOffset,
                1f,
                SpriteEffects.None,
                1f
            );
        /*if (Statics.BackdropOpacity > 0)
            SpriteBatch.Draw(
                _backdropTexture,
                Vector2.Zero,
                null,
                Color.White * Statics.BackdropOpacity,
                0f,
                Vector2.Zero,
                _backdropSize,
                SpriteEffects.None,
                0f
                );*/
        SpriteBatch.End();
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        Configs.CloseFile();
        Configs.ResolutionChanged -= ReloadScale;
        Configs.FullscreenChanged -= Fullscreen;
        Configs.MusicVolumeChanged -= UpdateVolume;
        base.OnExiting(sender, args);
    }
#endregion

#region Custom Methods
    private void ReloadScale(object s, EventArgs e)
    {
        if (Configs.Fullscreen)
        {
            Graphics.PreferredBackBufferWidth = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        }
        else
        {
            Graphics.PreferredBackBufferWidth = Configs.NativeWidth * Configs.Scale;
            Graphics.PreferredBackBufferHeight = Configs.NativeHeight * Configs.Scale;
        }
        EngineStatics.Scale = new Vector2(Configs.Scale);
        EngineStatics.Offset = Configs.ScreenOffset;
        Graphics.ApplyChanges();
    }

    private void Fullscreen(object s, EventArgs e) => Graphics.ToggleFullScreen();

    private void UpdateVolume(object s, EventArgs e) {MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2); MediaPlayer.IsMuted = _m(Configs.MusicVolume);}
    
    private readonly Func<int, bool> _m = x => x == 0;
#endregion
}
