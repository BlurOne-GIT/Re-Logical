using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class ColourHandicap : Block, IFixable
{
    #region Field
    public static ColourHandicap SteveJobs;
    public bool DisableJobs;
    private static Texture2D _balls;
    private static readonly Vector2[] BallOffsets =
    [
        new Vector2( 5f, 15f), // Left
        new Vector2(14f,  5f), // Up
        new Vector2(23f, 15f), // Right
        new Vector2(14f, 23f)  // Down
    ];
    private static readonly Vector2[] FixedBallOffsets =
    [
        new Vector2( 5f, 14f), // Left
        new Vector2(14f,  5f), // Up
        new Vector2(23f, 14f), // Right
        new Vector2(14f, 23f)  // Down
    ];

    private Vector2[] _ballOffsets = BallOffsets;
    
    private readonly Rectangle[] _rectangles = new Rectangle[4];
    #endregion

    public ColourHandicap(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "ColourHandicap", arrayPosition, xx, yy)
    {
        if (SteveJobs is not null)
            SteveJobs.DisableJobs = true;
        
        SteveJobs = this;
        
        DefaultSource = new Rectangle(0, 0, 36, 36);
    }

    protected override void LoadContent()
    {
        _balls ??= Game.Content.Load<Texture2D>("SpinnerBalls");
        base.LoadContent();
    }

    public void Recharge()
    {
        for (int i = 0; i < 4; i++)
        {
            var random = Statics.Brandom.Next(0, 4);
            LevelState.ColorJobLayout.Add((BallColors)random);
            _rectangles[i] = new Rectangle(8 * random, 0, 8, 8);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        if (LevelState.ColorJobLayout.Count == 0 || DisableJobs)
            return;

        for (int i = 0; i < 4; i++)
            DrawAnotherTexture(_balls, _ballOffsets[i], 1, _rectangles[i]);
    }

    protected override void Dispose(bool disposing)
    {
        if (SteveJobs.Equals(this))
            SteveJobs = null;
        base.Dispose(disposing);
    }

    protected override void UnloadContent()
    {
        _balls = null;
        Game.Content.UnloadAsset("SpinnerBalls");
        base.UnloadContent();
    }

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Refined;

    public void Fix(IFixable.FidelityLevel fidelity)
    {
        var rectangle = DefaultSource!.Value;
        if (Configs.GraphicSet >= 4)
            rectangle.Y = 36;

        if (fidelity is IFixable.FidelityLevel.Remastered)
        {
            rectangle.X = 36;
            _ballOffsets = FixedBallOffsets;
        }

        DefaultSource = rectangle;
    }
}