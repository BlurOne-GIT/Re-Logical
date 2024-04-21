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
        // Todo: have this array in a loadable xml file
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
            glyphRectangles.Add(new Rectangle(i*8, 0, 8, 7));
            fontRectangles.Add(new Rectangle(0, 0, 8, 7));
            kernings.Add(new Vector3(0, 8, 0));
        }
        LightFont = new SpriteFont(fontTexture, glyphRectangles, fontRectangles, characters, 0, 0, kernings, ' ');
        // Todo: have this array in a loadable xml file
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
            glyphRectangles.Add(new Rectangle(i*8, 7, 8, 8));
            fontRectangles.Add(new Rectangle(0, 0, 8, 7));
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
    public static Texture2D TpShadowEmpty; // TODO DISABLED
    public static Texture2D TpShadowHorizontal; // TODO DISABLED
    public static Texture2D TpShadowVertical; // TODO DISABLED
    public static Texture2D TpShadowCross; // TODO DISABLED
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
        /* DISABLED
        TpShadowEmpty = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowEmpty");
        TpShadowHorizontal = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowHorizontal");
        TpShadowVertical = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowVertical");
        TpShadowCross = Statics.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\TpShadowCross");
        */
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