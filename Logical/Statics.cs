using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public static class Statics
{
    #region Fields
    public const string StandardSet = "./alf.dat";
    #endregion

    #region Properties
    public static string Set { get; set; } = StandardSet;
    public static SpriteFont TextureFont { get; private set; }
    public static SpriteFont BoldFont { get; private set; }
    public static SpriteFont LightFont { get; private set; }
    public static bool ShowCursor { get; set; }
    public static float BackdropOpacity { get; set; }
    public static ContentManager Content { get; private set;}
    public static Random Brandom = new();
    public static readonly Dictionary<Direction, Direction> ReverseDirection = new(4)
    {
        {Direction.Left, Direction.Right},
        {Direction.Up, Direction.Down},
        {Direction.Right, Direction.Left},
        {Direction.Down, Direction.Up}
    };
    #endregion

    #region Methods
    public static void Initialize(ContentManager content)
    {
        Content = content;
    }
    
    public static void LoadFonts()
    {
        var fontTexture = Content.Load<Texture2D>("Fonts");
        var kernings = new List<Vector3>();
        var glyphRectangles = new List<Rectangle>();
        var fontRectangles = new List<Rectangle>();
        var characters = new List<char>{
            ' ',
            '!',
            '%',
            '*',
            '-',
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            ':',
            '=',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '_'
        };
        for (int i = 0; i < characters.Count; i++)
        {
            glyphRectangles.Add(new Rectangle(new Point(i*8, 0), new Point(8, 7)));
            fontRectangles.Add(new Rectangle(new Point(0, 0), new Point(8, 7)));
            kernings.Add(new Vector3(0, 8, 0));
        }
        LightFont = new SpriteFont(fontTexture, glyphRectangles, fontRectangles, characters, 0, 0, kernings, ' ');
        characters = new List<char>{
            ' ',
            '/',
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '_'
        };
        glyphRectangles = new List<Rectangle>();
        fontRectangles = new List<Rectangle>();
        kernings = new List<Vector3>();
        for (int i = 0; i < characters.Count; i++)
        {
            var p = new Point(i * 8, 7);
            glyphRectangles.Add(new Rectangle(p, new Point(8, 8)));
            fontRectangles.Add(new Rectangle(new Point(0, 0), new Point(8, 7)));
            kernings.Add(new Vector3(0, 8, 0));
        }
        BoldFont = new SpriteFont(fontTexture, glyphRectangles, fontRectangles, characters, 0, 0, kernings, ' ');
    }
    #endregion
}

public enum BallColors
{
    Pink,
    Yellow,
    Blue,
    Green
}

public enum Direction
{
    Left,
    Up,
    Right,
    Down
}

public static class LevelResources
{
    #region Textures
    public static Texture2D[] Ball = new Texture2D[4]; // DONE
    public static Texture2D MainPipe; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D MainPipeOpen; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D MainPipeBar; // Done
    public static Texture2D EmptyBlock; // 0✔ 1X 2X 3X 4X
    public static Texture2D EmptyBlockAlt; // 0✔ 1X 2X 3X 4X
    public static Texture2D Spinner; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D[] SpinnerSpin = new Texture2D[3]; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D[] SpinnerExplode = new Texture2D[7];  // 0✔ 1✔ 2X 3X 4X
    public static Texture2D SpinnerClosedLeft; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D SpinnerClosedUp; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D SpinnerClosedRight; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D SpinnerClosedDown; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D[] SpinnerBall = new Texture2D[4]; // DONE
    public static Texture2D SpinnerBallExploded; // DONE
    public static Texture2D PipeHorizontal; // 0✔ 1X 2X 3X 4X
    public static Texture2D PipeHorizontalAlt; // 0X 1✔ 2X 3X 4X
    public static Texture2D PipeVertical; // 0✔ 1X 2X 3X 4X
    public static Texture2D PipeVerticalAlt; // 0X 1✔ 2X 3X 4X
    public static Texture2D PipeCross; // 0✔ 1X 2X 3X 4X
    public static Texture2D[] Filter = new Texture2D[4]; // DONE
    public static Texture2D FilterShadowHorizontal; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D FilterShadowVertical; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D FilterShadowCross; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D TpHorizontal; // DONE
    public static Texture2D TpVertical; // DONE
    public static Texture2D TpCross; // DONE
    public static Texture2D TpShadowEmpty; // TODO DISABLED
    public static Texture2D TpShadowHorizontal; // TODO DISABLED
    public static Texture2D TpShadowVertical; // TODO DISABLED
    public static Texture2D TpShadowCross; // TODO DISABLED
    public static Texture2D PipeClosedLeft; // 0✔ 1X 2X 3X 4X
    public static Texture2D PipeClosedUp; // 0✔ 1X 2X 3X 4X
    public static Texture2D PipeClosedRight; // 0✔ 1X 2X 3X 4X
    public static Texture2D PipeClosedDown; // 0✔ 1X 2X 3X 4X
    public static Texture2D[] Changer = new Texture2D[4]; // DONE
    public static Texture2D[] Indicator = new Texture2D[4]; // DONE
    public static Texture2D ChangerShadowHorizontal; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D ChangerShadowVertical; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D ChangerShadowCross; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D Holder; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D HolderShadowEmpty; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D HolderShadowHorizontal; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D HolderShadowVertical; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D HolderShadowCross; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D[] Bumper = new Texture2D[4]; // 0✔ 1✔ 2X 3X 4X
    public static Texture2D Moves; // DONE
    public static Texture2D MovesBlue; // DONE
    public static Texture2D Sandclock; // DONE
    public static Texture2D[] UsedSand = new Texture2D[10]; // TODO DISABLED
    public static Texture2D[] SandLeft = new Texture2D[10]; // TODO DISABLED
    public static Texture2D FallingSand; // PLACEHOLDER TODO DISABLED
    public static Texture2D ColorJob; // DONE
    public static Texture2D TrafficLight; // DONE
    #endregion

    // Don't worry, I'll get rid of this after I'm done with the conversion
    public static void LoadTextures()
    {
        Ball[0] = Statics.Content.Load<Texture2D>("BallPink");
        Ball[1] = Statics.Content.Load<Texture2D>("BallYellow");
        Ball[2] = Statics.Content.Load<Texture2D>("BallBlue");
        Ball[3] = Statics.Content.Load<Texture2D>("BallGreen");
        MainPipe = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\MainPipe");
        MainPipeOpen = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\MainPipeOpen");
        MainPipeBar = Statics.Content.Load<Texture2D>("MainPipeBar");
        EmptyBlock = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\EmptyBlock");
        EmptyBlockAlt = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\EmptyBlockAlt");
        Spinner = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Spinner");
        /*
        SpinnerSpin[0] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerSpin0");
        SpinnerSpin[1] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerSpin1");
        SpinnerSpin[2] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerSpin2");
        */
        SpinnerExplode[0] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerExplode2");
        SpinnerExplode[1] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerExplode0");
        SpinnerExplode[2] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerExplode1");
        SpinnerExplode[3] = SpinnerExplode[2];
        SpinnerExplode[4] = SpinnerExplode[1];
        SpinnerExplode[5] = SpinnerExplode[0];
        SpinnerExplode[6] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerExplode3");
        SpinnerClosedLeft = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerClosedLeft");
        SpinnerClosedRight = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerClosedRight");
        SpinnerClosedDown = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerClosedDown");
        SpinnerClosedUp = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\SpinnerClosedUp");
        SpinnerBall[0] = Statics.Content.Load<Texture2D>("SpinnerBallPink");
        SpinnerBall[1] = Statics.Content.Load<Texture2D>("SpinnerBallYellow");
        SpinnerBall[2] = Statics.Content.Load<Texture2D>("SpinnerBallBlue");
        SpinnerBall[3] = Statics.Content.Load<Texture2D>("SpinnerBallGreen");
        SpinnerBallExploded = Statics.Content.Load<Texture2D>("SpinnerBallExploded");
        PipeHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeHorizontal");
        PipeVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeVertical");
        PipeHorizontalAlt = Configs.GraphicSet is 1 ? PipeHorizontal : Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeHorizontalAlt");
        PipeVerticalAlt = Configs.GraphicSet is 1 or 3 ? PipeVertical : Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeVerticalAlt");
        PipeCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeCross");
        Filter[0] = Statics.Content.Load<Texture2D>("FilterPink");
        Filter[1] = Statics.Content.Load<Texture2D>("FilterYellow");
        Filter[2] = Statics.Content.Load<Texture2D>("FilterBlue");
        Filter[3] = Statics.Content.Load<Texture2D>("FilterGreen");
        FilterShadowHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\FilterShadowHorizontal");
        FilterShadowVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\FilterShadowVertical");
        FilterShadowCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\FilterShadowCross");
        TpHorizontal = Statics.Content.Load<Texture2D>("TpHorizontal");
        TpVertical = Statics.Content.Load<Texture2D>("TpVertical");
        TpCross = Statics.Content.Load<Texture2D>("TpCross");
        /* DISABLED
        TpShadowEmpty = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowEmpty");
        TpShadowHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowHorizontal");
        TpShadowVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowVertical");
        TpShadowCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowCross");
        */
        Changer[0] = Statics.Content.Load<Texture2D>("ChangerPink");
        Changer[1] = Statics.Content.Load<Texture2D>("ChangerYellow");
        Changer[2] = Statics.Content.Load<Texture2D>("ChangerBlue");
        Changer[3] = Statics.Content.Load<Texture2D>("ChangerGreen");
        Indicator[0] = Statics.Content.Load<Texture2D>("IndicatorPink");
        Indicator[1] = Statics.Content.Load<Texture2D>("IndicatorYellow");
        Indicator[2] = Statics.Content.Load<Texture2D>("IndicatorBlue");
        Indicator[3] = Statics.Content.Load<Texture2D>("IndicatorGreen");
        ChangerShadowHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\ChangerShadowHorizontal");
        ChangerShadowVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\ChangerShadowVertical");
        ChangerShadowCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\ChangerShadowCross");
        Holder = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Holder");
        HolderShadowEmpty = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\HolderShadowEmpty");
        HolderShadowHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\HolderShadowHorizontal");
        HolderShadowVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\HolderShadowVertical");
        HolderShadowCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\HolderShadowCross");
        Bumper[0] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\BumperLeft");
        Bumper[1] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\BumperUp");
        Bumper[2] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\BumperRight");
        Bumper[3] = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\BumperDown");
        Moves = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Moves");
        MovesBlue = Statics.Content.Load<Texture2D>("MovesBlue");
        Sandclock = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\Sandclock");
        /* DISABLED
        for (int i = 0; i < 10; i++)
        {
            UsedSand[i] = Statics.Content.Load<Texture2D>($"UsedSand{i}");
            SandLeft[i] = Statics.Content.Load<Texture2D>($"SandLeft{i}");
        }
        FallingSand = Statics.Content.Load<Texture2D>("FallingSand");
        */
        ColorJob = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\ColorJob");
        TrafficLight = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TrafficLight");
        PipeClosedLeft = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeClosedLeft");
        PipeClosedUp = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeClosedUp");
        PipeClosedRight = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeClosedRight");
        PipeClosedDown = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\PipeClosedDown");
    }

    public static void UnloadTextures()
    {
        Statics.Content.UnloadAsset("PopIn");
        Statics.Content.UnloadAsset("PopOut");
        Statics.Content.UnloadAsset("Spin");
        Statics.Content.UnloadAsset("Bounce");
        Statics.Content.UnloadAsset("ColorChange");
        Statics.Content.UnloadAsset("Tp");
        Statics.Content.UnloadAsset("Explode");
        Statics.Content.UnloadAsset("BallPink");
        Statics.Content.UnloadAsset("BallYellow");
        Statics.Content.UnloadAsset("BallBlue");
        Statics.Content.UnloadAsset("BallGreen");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\MainPipe");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\MainPipeOpen");
        Statics.Content.UnloadAsset("MainPipeBar");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\EmptyBlock");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\EmptyBlockAlt");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\Spinner");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerSpin0");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerSpin1");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerSpin2");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerExplode0");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerExplode1");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerExplode2");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerExplode3");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerClosedLeft");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerClosedRight");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerClosedDown");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\SpinnerClosedUp");
        Statics.Content.UnloadAsset("SpinnerBallPink");
        Statics.Content.UnloadAsset("SpinnerBallYellow");
        Statics.Content.UnloadAsset("SpinnerBallBlue");
        Statics.Content.UnloadAsset("SpinnerBallGreen");
        Statics.Content.UnloadAsset("SpinnerBallExploded");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\PipeHorizontal");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\PipeVertical");
        if (Configs.GraphicSet is not 1)
            Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\PipeHorizontalAlt");
        if (Configs.GraphicSet is not 1 and 3)
            Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\PipeVerticalAlt");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\PipeCross");
        Statics.Content.UnloadAsset("FilterPink");
        Statics.Content.UnloadAsset("FilterYellow");
        Statics.Content.UnloadAsset("FilterBlue");
        Statics.Content.UnloadAsset("FilterGreen");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\FilterShadowHorizontal");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\FilterShadowVertical");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\FilterShadowCross");
        Statics.Content.UnloadAsset("TpHorizontal");
        Statics.Content.UnloadAsset("TpVertical");
        Statics.Content.UnloadAsset("TpCross");
        /* DISABLED
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\TpShadowEmpty");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\TpShadowHorizontal");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\TpShadowVertical");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\TpShadowCross");
        */
        Statics.Content.UnloadAsset("ChangerPink");
        Statics.Content.UnloadAsset("ChangerYellow");
        Statics.Content.UnloadAsset("ChangerBlue");
        Statics.Content.UnloadAsset("ChangerGreen");
        Statics.Content.UnloadAsset("IndicatorPink");
        Statics.Content.UnloadAsset("IndicatorYellow");
        Statics.Content.UnloadAsset("IndicatorBlue");
        Statics.Content.UnloadAsset("IndicatorGreen");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\ChangerShadowHorizontal");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\ChangerShadowVertical");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\ChangerShadowCross");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\Holder");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\HolderShadowEmpty");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\HolderShadowHorizontal");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\HolderShadowVertical");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\HolderShadowCross");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\BumperLeft");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\BumperRight");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\BumperDown");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\BumperUp");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\Moves");
        Statics.Content.UnloadAsset("MovesBlue");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\Sandclock");
        /* DISABLED
        for (int i = 0; i < 10; i++)
        {
            Statics.Content.UnloadAsset($"UsedSand{i}");
            Statics.Content.UnloadAsset($"SandLeft{i}");
        }
        Statics.Content.UnloadAsset("FallingSand");
        */
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\ColorJob");
        Statics.Content.UnloadAsset(@$"{Configs.GraphicSet}\TrafficLight");
    }
}