using System;
using System.Linq;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Tiles;

public class TrafficLights : GameTile
{
    private static TrafficLights _primaryInstance;
    private static Texture2D _balls;
    public bool DisableTraffic;

    private static readonly Vector2[] BallOffsets =
    [
        new(14f, 5f),
        new(14f, 14f),
        new(14f, 23f)
    ];
    private readonly Rectangle[] _rectangles = new Rectangle[3];

    public TrafficLights(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "TrafficLights", arrayPosition, xx, yy)
    {
        if (_primaryInstance is not null)
            _primaryInstance.DisableTraffic = true;
        
        _primaryInstance = this;

        LevelState.TrafficLights.Clear();

        if (yy is < 1 or > 4)
            throw new ArgumentException("Invalid TrafficLight position");
        
        LevelState.TrafficLights.AddRange(new []
            {
                BallColors.Pink,   // 1
                BallColors.Yellow, // 1 2
                BallColors.Blue,   // 1 2 3
                BallColors.Green,  //   2 3 4
                BallColors.Pink,   //     3 4
                BallColors.Yellow  //       4
            }.Take((yy - 1)..(yy + 2))
        );
        
        for (int i = 0; i < 3; i++)
            _rectangles[i] = new Rectangle(8 * (int)LevelState.TrafficLights[i], 0, 8, 8);
    }
    
    protected override void LoadContent()
    {
        _balls ??= Game.Content.Load<Texture2D>("SpinnerBalls");
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        for (int i = 3 - (DisableTraffic ? 3 : LevelState.TrafficLights.Count); i < 3; i++)
            DrawAnotherTexture(_balls, BallOffsets[i], 1, _rectangles[i]);
    }

    protected override void Dispose(bool disposing)
    {
        if (_primaryInstance.Equals(this))
            _primaryInstance = null;
        base.Dispose(disposing);
    }
    
    protected override void UnloadContent()
    {
        _balls = null;
        Game.Content.UnloadAsset("SpinnerBalls");
        base.UnloadContent();
    }
}