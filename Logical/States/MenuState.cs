using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class MenuState : GameState
{
    public MenuState():base()
    {
        Statics.ShowCursor = true;
    }

    #region Fields
    private Song choose;
    private SoundEffect clickSfx;
    private SoundEffect beginSfx;
    private SimpleImage Background;
    private Button OwnSetButton;
    private Button PasswordButton;
    private Button AboutButton;
    private Button SettingsButton;
    private Button GraphicSetButton;
    private Button StartButton;
    private Button ScaleButton;
    private Button FullScreenButton;
    private Button BgmVolUpButton;
    private Button BgmVolDownButton;
    private Button SfxVolUpButton;
    private Button SfxVolDownButton;
    private Button StereoSplitUpButton;
    private Button StereoSplitDownButton;
    private Button BackButton;
    private TextComponent BgmVol;
    private TextComponent SfxVol;
    private TextComponent StereoSplit;
    #endregion

    #region Properties

    #endregion

    #region Default Methods
    public override void LoadContent(ContentManager Content)
    {
        choose = Content.Load<Song>("Choose Music");
        clickSfx = Content.Load<SoundEffect>("Menu Button");
        beginSfx = Content.Load<SoundEffect>("1Success"); // DEBUG //
        MediaPlayer.Play(choose);
        MediaPlayer.IsRepeating = true;
        Background = new SimpleImage(Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Titlescreen"), Vector2.Zero, 0);
        OwnSetButton = new Button(new Vector2(108, 87), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\OwnSet"), 3, clickSfx, -1);
        PasswordButton = new Button(new Vector2(108, 109), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Password"), 3, clickSfx, -1);
        AboutButton = new Button(new Vector2(108, 132), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\About"), 3, clickSfx, -1);
        SettingsButton = new Button(new Vector2(108, 155), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Settings"), 3, clickSfx, -1);
        GraphicSetButton = new Button(new Vector2(108, 179), new Point(103, 22), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\GraphicSet"), 3, clickSfx, -1);
        StartButton = new Button(new Vector2(108, 201), new Point(103, 15), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\StartGame"), 3, clickSfx, -1);
        ScaleButton = new Button(new Vector2(108, 87), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Scale"), 3, clickSfx, -1, false);
        FullScreenButton = new Button(new Vector2(108, 109), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Fullscreen"), 3, clickSfx, -1, false);
        Texture2D p = Content.Load<Texture2D>("Plus");
        Texture2D m = Content.Load<Texture2D>("Minus");
        BgmVolUpButton = new Button(new Vector2(108, 132), new Point(10, 10), p, 3, clickSfx, -1, false);
        BgmVolDownButton = new Button(new Vector2(134, 132), new Point(10, 10), m, 3, clickSfx, -1, false);
        BgmVol = new TextComponent(Statics.BoldFont, new Vector2(118, 133), Color.White, $"{Configs.MusicVolume:00}", 3, enabled: false);
        SfxVolUpButton = new Button(new Vector2(108, 155), new Point(10, 10), p, 3, clickSfx, -1, false);
        SfxVolDownButton = new Button(new Vector2(134, 155), new Point(10, 10), m, 3, clickSfx, -1, false);
        SfxVol = new TextComponent(Statics.BoldFont, new Vector2(118, 156), Color.White, $"{Configs.SfxVolume:00}", 3, enabled: false);
        StereoSplitUpButton = new Button(new Vector2(108, 179), new Point(10, 10), p, 3, clickSfx, -1, false);
        StereoSplitDownButton = new Button(new Vector2(142, 179), new Point(10, 10), m, 3, clickSfx, -1, false);
        StereoSplit = new TextComponent(Statics.BoldFont, new Vector2(118, 180), Color.White, $"{Configs.StereoSeparation:000}", 3, enabled: false);
        BackButton = new Button(new Vector2(108, 201), new Point(103, 16), Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Back"), 3, clickSfx, -1, false);
        AddGameObject(Background);
        AddGameObject(OwnSetButton);
        AddGameObject(PasswordButton);
        AddGameObject(AboutButton);
        AddGameObject(SettingsButton);
        AddGameObject(GraphicSetButton);
        AddGameObject(StartButton);
        AddGameObject(ScaleButton);
        AddGameObject(FullScreenButton);
        AddGameObject(BgmVolUpButton);
        AddGameObject(BgmVolDownButton);
        AddGameObject(BgmVol);
        AddGameObject(SfxVolUpButton);
        AddGameObject(SfxVolDownButton);
        AddGameObject(SfxVol);
        AddGameObject(StereoSplitUpButton);
        AddGameObject(StereoSplitDownButton);
        AddGameObject(StereoSplit);
        AddGameObject(BackButton);
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

    public override void UnloadContent(ContentManager Content)
    {
        Content.UnloadAsset("Choose Music");
        Content.UnloadAsset("1Success"); // DEBUG //
        Content.UnloadAsset("Menu Button");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Titlescreen");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\OwnSet");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Password");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\About");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Settings");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\GraphicSet");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\StartGame");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Scale");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Fullscreen");
        Content.UnloadAsset("Plus");
        Content.UnloadAsset("Minus");
        Content.UnloadAsset(@$"{Configs.GraphicSet}\Back");
    }

    public override void Dispose()
    {
        OwnSetButton.LeftClicked -= OwnSet;
        PasswordButton.LeftClicked -= Password;
        AboutButton.LeftClicked -= About;
        SettingsButton.LeftClicked -= Settings;
        GraphicSetButton.LeftClicked -= GraphicSet;
        StartButton.LeftClicked -= StartGame;
        ScaleButton.LeftClicked -= Scale;
        FullScreenButton.LeftClicked -= Fullscreen;
        BgmVolUpButton.LeftClicked -= BgmVolUp;
        BgmVolDownButton.LeftClicked -= BgmVolDown;
        SfxVolUpButton.LeftClicked -= SfxVolUp;
        SfxVolDownButton.LeftClicked -= SfxVolDown;
        StereoSplitUpButton.LeftClicked -= StereoSplitUp;
        StereoSplitDownButton.LeftClicked -= StereoSplitDown;
        BackButton.LeftClicked -= Back;
        base.Dispose();
    }
    #endregion

    #region Methods
    private void ButtonSubscriber()
    {
        OwnSetButton.LeftClicked += OwnSet;
        PasswordButton.LeftClicked += Password;
        AboutButton.LeftClicked += About;
        SettingsButton.LeftClicked += Settings;
        GraphicSetButton.LeftClicked += GraphicSet;
        StartButton.LeftClicked += StartGame;
        ScaleButton.LeftClicked += Scale;
        FullScreenButton.LeftClicked += Fullscreen;
        BgmVolUpButton.LeftClicked += BgmVolUp;
        BgmVolDownButton.LeftClicked += BgmVolDown;
        SfxVolUpButton.LeftClicked += SfxVolUp;
        SfxVolDownButton.LeftClicked += SfxVolDown;
        StereoSplitUpButton.LeftClicked += StereoSplitUp;
        StereoSplitDownButton.LeftClicked += StereoSplitDown;
        BackButton.LeftClicked += Back;
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
        beginSfx.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
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
        string read = Console.ReadLine().ToUpper().TrimEnd();
        if (read.StartsWith("ELO WANTS "))
        {
            read = read.Remove(0, 10);
            try
            {
                Configs.Stage = Convert.ToInt32((string)read.Substring(0, read.Length < 12 ? 1 : 2));
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
        OwnSetButton.IsEnabled = false;
        PasswordButton.IsEnabled = false;
        AboutButton.IsEnabled = false;
        SettingsButton.IsEnabled = false;
        GraphicSetButton.IsEnabled = false;
        StartButton.IsEnabled = false;
        ScaleButton.IsEnabled = true;
        FullScreenButton.IsEnabled = true;
        BgmVolUpButton.IsEnabled = true;
        BgmVolDownButton.IsEnabled = true;
        BgmVol.IsEnabled = true;
        SfxVolUpButton.IsEnabled = true;
        SfxVolDownButton.IsEnabled = true;
        SfxVol.IsEnabled = true;
        StereoSplitUpButton.IsEnabled = true;
        StereoSplitDownButton.IsEnabled = true;
        StereoSplit.IsEnabled = true;
        BackButton.IsEnabled = true;
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
        BgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    private void BgmVolDown(object s, EventArgs e)
    {
        if (Configs.MusicVolume == 0)
            return;

        Configs.MusicVolume--;
        BgmVol.Text = $"{Configs.MusicVolume:00}";
    }
    private void SfxVolUp(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 10)
            return;

        Configs.SfxVolume++;
        SfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    private void SfxVolDown(object s, EventArgs e)
    {
        if (Configs.SfxVolume == 0)
            return;

        Configs.SfxVolume--;
        SfxVol.Text = $"{Configs.SfxVolume:00}";
    }
    private void StereoSplitUp(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 100)
            return;

        Configs.StereoSeparation += 10;
        StereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }
    private void StereoSplitDown(object s, EventArgs e)
    {
        if (Configs.StereoSeparation == 0)
            return;

        Configs.StereoSeparation -= 10;
        StereoSplit.Text = $"{Configs.StereoSeparation:000}";
    }
    private void Back(object s, EventArgs e)
    {
        OwnSetButton.IsEnabled = true;
        PasswordButton.IsEnabled = true;
        AboutButton.IsEnabled = true;
        SettingsButton.IsEnabled = true;
        GraphicSetButton.IsEnabled = true;
        StartButton.IsEnabled = true;
        ScaleButton.IsEnabled = false;
        FullScreenButton.IsEnabled = false;
        BgmVolUpButton.IsEnabled = false;
        BgmVolDownButton.IsEnabled = false;
        BgmVol.IsEnabled = false;
        SfxVolUpButton.IsEnabled = false;
        SfxVolDownButton.IsEnabled = false;
        SfxVol.IsEnabled = false;
        StereoSplitUpButton.IsEnabled = false;
        StereoSplitDownButton.IsEnabled = false;
        StereoSplit.IsEnabled = false;
        BackButton.IsEnabled = false;
    }

    #endregion
}