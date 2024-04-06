using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class TrafficLight : Block
{
    #region Field
    private static TrafficLight _theChosenOne;
    private readonly Texture2D _ball1;
    private readonly Texture2D _ball2;
    private readonly Texture2D _ball3;
    public bool DisableTraffic;
    private readonly Vector2 _b1Pos = new(14f, 5f);
    private readonly Vector2 _b2Pos = new(14f, 14f);
    private readonly Vector2 _b3Pos = new(14f, 23f);
    #endregion

    public TrafficLight(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.TrafficLight, arrayPosition, xx, yy)
    {
        if (_theChosenOne is not null)
            _theChosenOne.DisableTraffic = true;
        
        _theChosenOne = this;

        LevelState.TrafficLights.Clear();

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
        
        _ball1 = LevelResources.SpinnerBall[(int)LevelState.TrafficLights[0]];
        _ball2 = LevelResources.SpinnerBall[(int)LevelState.TrafficLights[1]];
        _ball3 = LevelResources.SpinnerBall[(int)LevelState.TrafficLights[2]];
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        switch (DisableTraffic ? 3 : LevelState.TrafficLights.Count)
        {
            case 3:
                DrawAnotherTexture(_ball1, _b1Pos, 1);
                goto case 2;
            case 2:
                DrawAnotherTexture(_ball2, _b2Pos, 1);
                goto case 1;
            case 1:
                DrawAnotherTexture(_ball3, _b3Pos, 1);
                break;
        }
    }

    public new void Dispose()
    {
        if (_theChosenOne.Equals(this))
            _theChosenOne = null;
        base.Dispose();
    }
}