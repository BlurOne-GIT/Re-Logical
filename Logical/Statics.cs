using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public static class Statics
{
    #region Fields
    public const string StandardSet = "./Content/alf.dat";
    private static SpriteFont _topaz;
    private static SpriteFont _topazPlus;
    private static readonly Color[] TopazColors =
    [
        new Color(0xFF334466U), // packed ABGR for #643 (GS1)
        new Color(0xFF335555U), // packed ABGR for #553 (GS2)
        new Color(0xFF664466U), // packed ABGR for #646 (GS3)
        new Color(0xFF114455U), // packed ABGR for #541 (GS4)
        new Color(0xFF115588U)  // packed ABGR for #851 (GS4R)
    ];
    #endregion

    #region Properties
    private static string _levelSetPath = StandardSet;
    private static readonly SpriteFont[] TextureFonts = new SpriteFont[5]; // TODO: add texture font fidelity levels

    public static string LevelSetPath
    {
        get => _levelSetPath;
        set
        {
            _levelSetPath = value;
            LevelSet = new LevelSet(value);
        }
    }

    public static LevelSet LevelSet { get; private set; } = new(StandardSet);
    public static SpriteFont TextureFont => TextureFonts[Configs.GraphicSet-1];
    public static SpriteFont TopazFont => Configs.FidelityLevel is IFixable.FidelityLevel.Remastered ? _topazPlus : _topaz;
    public static Color TopazColor => TopazColors[Configs.GraphicSet-1];
    public static SpriteFont DisplayFont { get; private set; }
    public static bool ShowCursor { get; set; }
    public static float BackdropOpacity { get; set; }
    public static readonly Random Brandom = new();
    public static readonly Dictionary<Direction, Direction> ReverseDirection = new(4)
    {
        {Direction.Left, Direction.Right},
        {Direction.Up, Direction.Down},
        {Direction.Right, Direction.Left},
        {Direction.Down, Direction.Up}
    };

    public static readonly Vector2 CursorTextureOffset = new(7f, 7f);
    #endregion

    #region Methods
    // TODO: after exporting the complete font sprite, turn into an actual font to use along a local spritefont description
    public static void LoadFonts(ContentManager content)
    {
        var fontTexture = content.Load<Texture2D>("Fonts/DisplayFont");
        var kernings = new List<Vector3>();
        var glyphRectangles = new List<Rectangle>();
        var fontRectangles = new List<Rectangle>();
        var characters = content.Load<List<char>>("Fonts/DisplayFontCharset");
        for (int i = 0; i < characters.Count; i++)
        {
            glyphRectangles.Add(new Rectangle(i*8, 0, 8, 7));
            fontRectangles.Add(new Rectangle(0, 0, 8, 7));
            kernings.Add(new Vector3(0, 8, 0));
        }
        DisplayFont = new SpriteFont(fontTexture, glyphRectangles, fontRectangles, characters, 0, 0, kernings, ' ');
        
        _topaz = content.Load<SpriteFont>("Fonts/Topaz");
        for (int i = 0; i < _topaz.Glyphs.Length; i++)
            ++_topaz.Glyphs[i].Cropping.X;
        _topazPlus = content.Load<SpriteFont>("Fonts/TopazPlus");
        for (int i = 0; i < _topazPlus.Glyphs.Length; i++)
            ++_topazPlus.Glyphs[i].Cropping.X;
        
        characters = content.Load<List<char>>("Fonts/TextureFontCharset");
        kernings = [];
        glyphRectangles = [];
        fontRectangles = [];
        for (int i = 0; i < characters.Count; ++i)
        {
            glyphRectangles.Add(new Rectangle(i*16, 0, 16, 16));
            fontRectangles.Add(new Rectangle(0, 0, 16, 16));
            kernings.Add(new Vector3(0, 16, 0));
        }
        // TODO: add all texture fonts
        for (int gs = 0; gs < 1/*5*/; ++gs)
        {
            fontTexture = content.Load<Texture2D>($"{gs+1}/UI/Font");
            TextureFonts[gs] = new SpriteFont(fontTexture, glyphRectangles, fontRectangles, characters, 0, 0, kernings, ' ');
        }
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