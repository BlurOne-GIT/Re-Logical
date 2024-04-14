using System;
using System.Linq;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class TrafficLight : Block
{
    #region Field
    private static TrafficLight _theChosenOne;
    private static Texture2D _balls;
    public bool DisableTraffic;

    private static readonly Vector2[] BallOffsets =
    {
        new(14f, 5f),
        new(14f, 14f),
        new(14f, 23f)
    };
    private readonly Rectangle[] _rectangles = new Rectangle[3];
    #endregion

    public TrafficLight(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.TrafficLight, arrayPosition, xx, yy)
    {
        if (_theChosenOne is not null)
            _theChosenOne.DisableTraffic = true;
        
        _theChosenOne = this;

        LevelState.TrafficLights.Clear();

        if (yy is < 1 or > 4)
            throw new ArgumentException("Invalid TrafficLight position");
        
        LevelState.TrafficLights.AddRange(new [] { BallColors.Pink, BallColors.Yellow, BallColors.Blue, BallColors.Green, BallColors.Pink, BallColors.Yellow }.Take(
            (yy - 1)..(yy + 2)));
        /*
        LevelState.TrafficLights.AddRange(yy switch
        {
            1 => new[]
            {
                BallColors.Pink,
                BallColors.Yellow,
                BallColors.Blue
            },
            2 => new[]
            {
                BallColors.Yellow,
                BallColors.Blue,
                BallColors.Green
            },
            3 => new[]
            {
                BallColors.Blue,
                BallColors.Green,
                BallColors.Pink
            },
            4 => new[]
            {
                BallColors.Green,
                BallColors.Pink,
                BallColors.Yellow
            },
            _ => throw new ArgumentException("Invalid TrafficLight colors")
        });
        */
        
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
        
        for (int i = 3 - LevelState.TrafficLights.Count; i < 3; i++)
            DrawAnotherTexture(_balls, BallOffsets[i], 1, _rectangles[i]);
    }

    protected override void Dispose(bool disposing)
    {
        if (_theChosenOne.Equals(this))
            _theChosenOne = null;
        base.Dispose(disposing);
    }
    
    protected override void UnloadContent()
    {
        _balls = null;
        Game.Content.UnloadAsset("SpinnerBalls");
        base.UnloadContent();
    }
}