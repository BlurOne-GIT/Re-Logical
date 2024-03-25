using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Logical.States;

public class MenuState : GameState
{
    public MenuState() => Statics.ShowCursor = true;

    #region Fields
    private Song _choose;
    private SoundEffect _clickSfx;
    private SoundEffect _beginSfx;
    private SimpleImage _background;
    private Button _ownSetButton;
    private Button _passwordButton;
    private Button _aboutButton;
    private Button _settingsButton;
    private Button _graphicSetButton;
    private Button _startButton;
    private Button _scaleButton;
    private Button _fullScreenButton;
    private Button _bgmVolUpButton;
    private Button _bgmVolDownButton;
    private Button _sfxVolUpButton;
    private Button _sfxVolDownButton;
    private Button _stereoSplitUpButton;
    private Button _stereoSplitDownButton;
    private Button _backButton;
    private TextComponent _bgmVol;
    private TextComponent _sfxVol;
    private TextComponent _stereoSplit;
    #endregion

    #region Default Methods
    public override void LoadContent(ContentManager content)
    {
        _choose = content.Load<Song>("Choose Music");
        _clickSfx = content.Load<SoundEffect>("Menu Button");
        _beginSfx = content.Load<SoundEffect>("1Success"); // DEBUG //
        MediaPlayer.Play(_choose);
        MediaPlayer.IsRepeating = true;
        _background = new SimpleImage(content.Load<Texture2D>($"{Configs.GraphicSet}/Titlescreen"), Vector2.Zero, 0);
        _ownSetButton = new Button(new Vector2(108, 87), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/OwnSet"), 3, _clickSfx, -1);
        _passwordButton = new Button(new Vector2(108, 109), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/Password"), 3, _clickSfx, -1);
        _aboutButton = new Button(new Vector2(108, 132), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/About"), 3, _clickSfx, -1);
        _settingsButton = new Button(new Vector2(108, 155), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/Settings"), 3, _clickSfx, -1);
        _graphicSetButton = new Button(new Vector2(108, 179), new Point(103, 22), content.Load<Texture2D>($"{Configs.GraphicSet}/GraphicSet"), 3, _clickSfx, -1);
        _startButton = new Button(new Vector2(108, 201), new Point(103, 15), content.Load<Texture2D>($"{Configs.GraphicSet}/StartGame"), 3, _clickSfx, -1);
        _scaleButton = new Button(new Vector2(108, 87), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/Scale"), 3, _clickSfx, -1, false);
        _fullScreenButton = new Button(new Vector2(108, 109), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/Fullscreen"), 3, _clickSfx, -1, false);
        var p = content.Load<Texture2D>("Plus");
        var m = content.Load<Texture2D>("Minus");
        _bgmVolUpButton = new Button(new Vector2(108, 132), new Point(10, 10), p, 3, _clickSfx, -1, false);
        _bgmVolDownButton = new Button(new Vector2(134, 132), new Point(10, 10), m, 3, _clickSfx, -1, false);
        _bgmVol = new TextComponent(Statics.BoldFont, new Vector2(118, 133), Color.White, $"{Configs.MusicVolume:00}", 3, enabled: false);
        _sfxVolUpButton = new Button(new Vector2(108, 155), new Point(10, 10), p, 3, _clickSfx, -1, false);
        _sfxVolDownButton = new Button(new Vector2(134, 155), new Point(10, 10), m, 3, _clickSfx, -1, false);
        _sfxVol = new TextComponent(Statics.BoldFont, new Vector2(118, 156), Color.White, $"{Configs.SfxVolume:00}", 3, enabled: false);
        _stereoSplitUpButton = new Button(new Vector2(108, 179), new Point(10, 10), p, 3, _clickSfx, -1, false);
        _stereoSplitDownButton = new Button(new Vector2(142, 179), new Point(10, 10), m, 3, _clickSfx, -1, false);
        _stereoSplit = new TextComponent(Statics.BoldFont, new Vector2(118, 180), Color.White, $"{Configs.StereoSeparation:000}", 3, enabled: false);
        _backButton = new Button(new Vector2(108, 201), new Point(103, 16), content.Load<Texture2D>($"{Configs.GraphicSet}/Back"), 3, _clickSfx, -1, false);
        AddGameObject(_background);
        AddGameObject(_ownSetButton);
        AddGameObject(_passwordButton);
        AddGameObject(_aboutButton);
        AddGameObject(_settingsButton);
        AddGameObject(_graphicSetButton);
        AddGameObject(_startButton);
        AddGameObject(_scaleButton);
        AddGameObject(_fullScreenButton);
        AddGameObject(_bgmVolUpButton);
        AddGameObject(_bgmVolDownButton);
        AddGameObject(_bgmVol);
        AddGameObject(_sfxVolUpButton);
        AddGameObject(_sfxVolDownButton);
        AddGameObject(_sfxVol);
        AddGameObject(_stereoSplitUpButton);
        AddGameObject(_stereoSplitDownButton);
        AddGameObject(_stereoSplit);
        AddGameObject(_backButton);
        FadeIn();
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void HandleInput(object s, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Escape: NotifyEvent(GameEvents.Exit); break;
        }
    }
    public override void HandleInput(object s, ButtonEventArgs e) {}

    public override void UnloadContent(ContentManager content)
    {
        content.UnloadAsset("Choose Music");
        content.UnloadAsset("1Success"); // DEBUG //
        content.UnloadAsset("Menu Button");
        content.UnloadAsset($"{Configs.GraphicSet}/Titlescreen");
        content.UnloadAsset($"{Configs.GraphicSet}/OwnSet");
        content.UnloadAsset($"{Configs.GraphicSet}/Password");
        content.UnloadAsset($"{Configs.GraphicSet}/About");
        content.UnloadAsset($"{Configs.GraphicSet}/Settings");
        content.UnloadAsset($"{Configs.GraphicSet}/GraphicSet");
        content.UnloadAsset($"{Configs.GraphicSet}/StartGame");
        content.UnloadAsset($"{Configs.GraphicSet}/Scale");
        content.UnloadAsset($"{Configs.GraphicSet}/Fullscreen");
        content.UnloadAsset("Plus");
        content.UnloadAsset("Minus");
        content.UnloadAsset($"{Configs.GraphicSet}/Back");
    }

    public override void Dispose()
    {
        _ownSetButton.LeftClicked -= OwnSet;
        _passwordButton.LeftClicked -= Password;
        _aboutButton.LeftClicked -= About;
        _settingsButton.LeftClicked -= Settings;
        _graphicSetButton.LeftClicked -= GraphicSet;
        _startButton.LeftClicked -= StartGame;
        _scaleButton.LeftClicked -= Scale;
        _fullScreenButton.LeftClicked -= Fullscreen;
        _bgmVolUpButton.LeftClicked -= BgmVolUp;
        _bgmVolDownButton.LeftClicked -= BgmVolDown;
        _sfxVolUpButton.LeftClicked -= SfxVolUp;
        _sfxVolDownButton.LeftClicked -= SfxVolDown;
        _stereoSplitUpButton.LeftClicked -= StereoSplitUp;
        _stereoSplitDownButton.LeftClicked -= StereoSplitDown;
        _backButton.LeftClicked -= Back;
        base.Dispose();
    }
    #endregion

    #region Methods
    private void ButtonSubscriber()
    {
        _ownSetButton.LeftClicked += OwnSet;
        _passwordButton.LeftClicked += Password;
        _aboutButton.LeftClicked += About;
        _settingsButton.LeftClicked += Settings;
        _graphicSetButton.LeftClicked += GraphicSet;
        _startButton.LeftClicked += StartGame;
        _scaleButton.LeftClicked += Scale;
        _fullScreenButton.LeftClicked += Fullscreen;
        _bgmVolUpButton.LeftClicked += BgmVolUp;
        _bgmVolDownButton.LeftClicked += BgmVolDown;
        _sfxVolUpButton.LeftClicked += SfxVolUp;
        _sfxVolDownButton.LeftClicked += SfxVolDown;
        _stereoSplitUpButton.LeftClicked += StereoSplitUp;
        _stereoSplitDownButton.LeftClicked += StereoSplitDown;
        _backButton.LeftClicked += Back;
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
        _beginSfx.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
        await Task.Delay(660);
        SwitchState(new LoadingState());
    }

    private void OwnSet(object s, EventArgs e)
    {
        throw new NotImplementedException();
    }
    private void Password(object s, EventArgs e)
    {
        Console.Write("Insert stage key: ");
        string read = Console.ReadLine()?.ToUpper().TrimEnd();
        if (read != null && read.StartsWith("ELO WANTS "))
        {
            read = read.Remove(0, 10);
            try
            {
                Configs.Stage = Convert.ToInt32(read[..(read.Length < 12 ? 1 : 2)]);
            } catch (FormatException)
            {
                throw new NotImplementedException();
            }
            Console.WriteLine("NEW LEVEL: " + Configs.Stage);
            return;
        }

        int search = new Lexer().GetLevelNumber(read);
        if (search is 0)
        {
            Console.WriteLine("WRONG PASSWORD");
            return;
        }

        Configs.Stage = search;
        Console.WriteLine("NEW LEVEL: " + Configs.Stage);
    }
    private void About(object s, EventArgs e)
    {
        throw new NotImplementedException();
    }
    private void Settings(object s, EventArgs e)
    {
        _ownSetButton.IsEnabled = false;
        _passwordButton.IsEnabled = false;
        _aboutButton.IsEnabled = false;
        _settingsButton.IsEnabled = false;
        _graphicSetButton.IsEnabled = false;
        _startButton.IsEnabled = false;
        _scaleButton.IsEnabled = true;
        _fullScreenButton.IsEnabled = true;
        _bgmVolUpButton.IsEnabled = true;
        _bgmVolDownButton.IsEnabled = true;
        _bgmVol.IsEnabled = true;
        _sfxVolUpButton.IsEnabled = true;
        _sfxVolDownButton.IsEnabled = true;
        _sfxVol.IsEnabled = true;
        _stereoSplitUpButton.IsEnabled = true;
        _stereoSplitDownButton.IsEnabled = true;
        _stereoSplit.IsEnabled = true;
        _backButton.IsEnabled = true;
    }
    private void GraphicSet(object s, EventArgs e)
    {
        throw new NotImplementedException();
    }
    private void StartGame(object s, EventArgs e)
    {
        FadeOut();
    }

    private void Scale(object s, EventArgs e)
    {
        if (Configs.Scale != Configs.MaxScale)
            Configs.Scale++;
        else
            Configs.Scale = 1;
    }
    private void Fullscreen(object s, EventArgs e) => Configs.Fullscreen ^= true;
    private void BgmVolUp(object s, EventArgs e)
    {
        if (Configs.MusicVolume == 10)
            return;

        Configs.MusicVolume++;
        _bgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    private void BgmVolDown(object s, EventArgs e)
    {
        if (Configs.MusicVolume == 0)
            return;

        Configs.MusicVolume--;
        _bgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    private void SfxVolUp(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 10)
            return;

        Configs.SfxVolume++;
        _sfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    private void SfxVolDown(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 0)
            return;

        Configs.SfxVolume--;
        _sfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    private void StereoSplitUp(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 100)
            return;

        Configs.StereoSeparation += 10;
        _stereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }
    private void StereoSplitDown(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 0)
            return;

        Configs.StereoSeparation -= 10;
        _stereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }
    private void Back(object s, EventArgs e)
    {
        _ownSetButton.IsEnabled = true;
        _passwordButton.IsEnabled = true;
        _aboutButton.IsEnabled = true;
        _settingsButton.IsEnabled = true;
        _graphicSetButton.IsEnabled = true;
        _startButton.IsEnabled = true;
        _scaleButton.IsEnabled = false;
        _fullScreenButton.IsEnabled = false;
        _bgmVolUpButton.IsEnabled = false;
        _bgmVolDownButton.IsEnabled = false;
        _bgmVol.IsEnabled = false;
        _sfxVolUpButton.IsEnabled = false;
        _sfxVolDownButton.IsEnabled = false;
        _sfxVol.IsEnabled = false;
        _stereoSplitUpButton.IsEnabled = false;
        _stereoSplitDownButton.IsEnabled = false;
        _stereoSplit.IsEnabled = false;
        _backButton.IsEnabled = false;
    }

    #endregion
}