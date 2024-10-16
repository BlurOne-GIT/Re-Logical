﻿using System;
using Logical.States;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        TargetElapsedTime = TimeSpan.FromTicks(200000L);
    }

    protected override void Initialize()
    {
        Configs.Initialize(Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width, Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
        ReloadScale(null, EventArgs.Empty);
        if (Configs.Fullscreen)
            Fullscreen(this, EventArgs.Empty);
        
        Window.ScreenDeviceNameChanged += OnWindowOnScreenDeviceNameChanged;
        Configs.ResolutionChanged += ReloadScale;
        Configs.FullscreenChanged += Fullscreen;
        Configs.MusicVolumeChanged += UpdateVolume;
        GameStateManager.Switched += OnGameStateSwitch;
        GameStateManager.GameState = new TitleState(this);
        base.Initialize();
    }

    private void OnWindowOnScreenDeviceNameChanged(object s, EventArgs e) =>
        Configs.ScreenSize = new Vector2(
            Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
            Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height
        );

    protected override void LoadContent()
    {
        base.LoadContent();

        MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2);
        MediaPlayer.IsMuted = Configs.MusicVolume is 0;
        
        _cursorTexture = Content.Load<Texture2D>("Cursor");
        _backdropTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
        _backdropTexture.SetData([Color.Black]);
        
        Components.Add(
            Statics.Cursor = new Cursor(this) { Enabled = false, Visible = false, DrawOrder = 10, UpdateOrder = 0}
        );
        Components.Add(
            Statics.Backdrop = new SimpleImage(this, _backdropTexture,
                    (Configs.NativeSize - Configs.ScreenSize / Configs.MaxScale) / 2, 10)
                { Enabled = false, Opacity = 0f, Scale = Configs.ScreenSize / Configs.MaxScale }
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

        SpriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: ViewportMatrix);
        base.Draw(gameTime);
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

    private void Fullscreen(object s, EventArgs e)
    {
        Graphics.ToggleFullScreen();
        Window.IsBorderless = Configs.Fullscreen;
    }

    private static void UpdateVolume(object s, EventArgs e)
    {
        MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2);
        MediaPlayer.IsMuted = Configs.MusicVolume is 0;
    }
#endregion
}
