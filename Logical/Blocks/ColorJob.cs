using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class ColorJob : Block, IFixable
{
    #region Field
    public static ColorJob SteveJobs;
    public bool DisableJobs;
    private static Texture2D _balls;
    private static readonly Vector2[] BallOffsets =
    {
        new( 5f, 15f), // Left
        new(14f,  5f), // Up
        new(23f, 15f), // Right
        new(14f, 23f)  // Down
    };
    private static readonly Vector2[] FixedBallOffsets =
    {
        new( 5f, 14f), // Left
        new(14f,  5f), // Up
        new(23f, 14f), // Right
        new(14f, 23f)  // Down
    };

    // Uncomment when MonoGame moves to .Net 7+
    //private ref Vector2[] ballOffsets = ref BallOffsets;
    // In the meanwhile, we're stuck with copying the array values
    private Vector2[] _ballOffsets = BallOffsets;
    
    private readonly Rectangle[] _rectangles = new Rectangle[4];
    #endregion

    public ColorJob(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "ColorJob", arrayPosition, xx, yy)
    {
        if (SteveJobs is not null)
            SteveJobs.DisableJobs = true;
        
        SteveJobs = this;
        
        DefaultRectangle = new Rectangle(0, 0, 36, 36);
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

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Fixed;

    public void Fix(IFixable.FidelityLevel fidelity)
    {
        var rectangle = DefaultRectangle!.Value;
        if (Configs.GraphicSet >= 4)
            rectangle.Y = 36;

        if (fidelity is IFixable.FidelityLevel.Remastered)
        {
            rectangle.X = 36;
            _ballOffsets = FixedBallOffsets;
        }

        DefaultRectangle = rectangle;
    }
}