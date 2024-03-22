using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Octokit;

namespace Logical;

public class Game1 : Game
{
#region Instances
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D CursorTexture;
    private TextComponent Version;
    private GameState _currentGameState;
    private readonly string version = (System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)[0] as System.Reflection.AssemblyInformationalVersionAttribute).InformationalVersion;
#endregion

#region Default Methods
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Statics.Initialize(Content);
        Window.AllowAltF4 = true;
        Window.AllowUserResizing = false;
        Window.IsBorderless = false;
        Configs.Initialize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        /*
        #if !DEBUG
        if (Configs.AutoUpdate)
            AutoUpdater();
        #endif*/
    }

    /*
    private async void AutoUpdater()
    {
        GitHubClient gc = new GitHubClient(new Octokit.ProductHeaderValue("BlurOne-GIT"));
        gc.Credentials = new Credentials("X");
        Release latestRelease = await gc.Repository.Release.GetLatest("BlurOne-GIT", "Input-is-not-real");
        if (latestRelease.Name == version)
            return;

        Version.Text = "NEW VERSION FOUND";
        string v = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "Unknown";
        foreach (var asset in latestRelease.Assets)
        {
            if (asset.Name.Contains(v))
            {
                Version.Text = "DOWNLOADING";
                FileStream fs = File.Create(@".\update.zip");
                var response = await gc.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream");
                fs.Write(response.Body.ToArray());
                Version.Text = "RESTARTING";
                fs.Close();
                Process.Start(v == "windows" ? @".\Updater.exe" : @".\Updater");
                this.Exit();
            }
        }
        Version.Text = $"{version.ToUpper().Replace('.', '_')} COULD NOT UPDATE";
    }
    */

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
        ReloadScale(null, EventArgs.Empty);
        if (Configs.Fullscreen)
            _graphics.ToggleFullScreen();
        Window.KeyDown += UpdateInputs;
        Window.KeyUp += UpdateInputs;
        
        Configs.ResolutionChanged += ReloadScale;
        Configs.FullscreenChanged += Fullscreen;
        Configs.MusicVolumeChanged += UpdateVolume;
        Activated += Statics.Focus;
        Deactivated += Statics.UnFocus;
        Statics.Focus(null, null);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        MediaPlayer.Volume = MathF.Pow(Configs.MusicVolume * 0.1f, 2);
        MediaPlayer.IsMuted = _m(Configs.MusicVolume);
        
        CursorTexture = Content.Load<Texture2D>("Cursor");
        Statics.LoadFonts();
        Version = new TextComponent(Statics.LightFont, new Vector2(0, 248), Color.White, version.ToUpper().Replace('.', '_'), 1);

        SwitchGameState(new TitleState());
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here

        Input.UpdateInputs(Mouse.GetState());
        Statics.MousePoint = Mouse.GetState().Position;
        _currentGameState.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _currentGameState.Render(_spriteBatch);
        if (Statics.ShowCursor)
        {
            _spriteBatch.Draw(
                CursorTexture,
                new Vector2((int)(Statics.MousePoint.X / Configs.Scale) * Configs.Scale, (int)(Statics.MousePoint.Y / Configs.Scale) * Configs.Scale),
                null,
                Color.White * Statics.Opacity,
                0,
                new Vector2(7, 7),
                Configs.Scale,
                SpriteEffects.None,
                1f
            );
        }
        Version.Render(_spriteBatch);
        _spriteBatch.End();

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
        Window.KeyDown -= UpdateInputs;
        Window.KeyUp -= UpdateInputs;
        Configs.ResolutionChanged -= ReloadScale;
        Configs.FullscreenChanged -= Fullscreen;
        Configs.MusicVolumeChanged -= UpdateVolume;
        _currentGameState.OnStateSwitched -= OnStateSwitched;
        _currentGameState.OnEventNotification -= OnEventNotification;
        Activated -= Statics.Focus;
        Deactivated -= Statics.UnFocus;
    }
#endregion

#region Custom Methods
    private void SwitchGameState(GameState newGameState)
    {
        if (_currentGameState is not null)
        {
            _currentGameState.OnStateSwitched -= OnStateSwitched;
            _currentGameState.OnEventNotification -= OnEventNotification;
            _currentGameState.UnloadContent(Content);
            _currentGameState.Dispose();
        }

        _currentGameState = newGameState;

        _currentGameState.LoadContent(Content);

        _currentGameState.OnStateSwitched += OnStateSwitched;
        _currentGameState.OnEventNotification += OnEventNotification;
    }

    private void OnStateSwitched(object s, GameState e) => SwitchGameState(e);

    private void OnEventNotification(object s, GameEvents e)
    {
        switch (e)
        {
            case GameEvents.Exit: Exit(); break;
        }
    }

    private void ReloadScale(object s, EventArgs e)
    {
        if (Configs.Fullscreen)
        {
            _graphics.PreferredBackBufferWidth = Configs.Width; _graphics.PreferredBackBufferHeight = Configs.Height;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = Configs.NativeWidth * Configs.Scale; _graphics.PreferredBackBufferHeight = Configs.NativeHeight * Configs.Scale;
        }
        _graphics.ApplyChanges();
    }

    private void UpdateInputs(object s, InputKeyEventArgs e) => Input.UpdateInputs(Keyboard.GetState());

    private void Fullscreen(object s, EventArgs e) => _graphics.ToggleFullScreen();

    private void UpdateVolume(object s, EventArgs e) {MediaPlayer.Volume = MathF.Pow((float)Configs.MusicVolume * 0.1f, 2); MediaPlayer.IsMuted = _m(Configs.MusicVolume);}
    
    private readonly Func<int, bool> _m = x => x == 0;
#endregion
}
