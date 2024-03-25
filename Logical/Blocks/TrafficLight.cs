using System;
using System.Collections.Generic;
using System.Linq;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class TrafficLight : Block
{
    #region Field
    private static TrafficLight theChosenOne;
    private readonly Texture2D _ball1;
    private readonly Texture2D _ball2;
    private readonly Texture2D _ball3;
    public bool DisableTraffic;
    private readonly Vector2 b1Pos = new Vector2(14f, 5f);
    private readonly Vector2 b2Pos = new Vector2(14f, 14f);
    private readonly Vector2 b3Pos = new Vector2(14f, 23f);
    #endregion

    public TrafficLight(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        _texture = LevelTextures.TrafficLight;
        if (theChosenOne is not null)
            theChosenOne.DisableTraffic = true;
        
        theChosenOne = this;

        LevelState.TrafficLights.Clear();

        switch(yy)
        {
            case 1:
                LevelState.TrafficLights.Add(BallColors.Pink);
                LevelState.TrafficLights.Add(BallColors.Yellow);
                LevelState.TrafficLights.Add(BallColors.Blue);
                break;
            case 2:
                LevelState.TrafficLights.Add(BallColors.Yellow);
                LevelState.TrafficLights.Add(BallColors.Blue);
                LevelState.TrafficLights.Add(BallColors.Green);
                break;
            case 3:
                LevelState.TrafficLights.Add(BallColors.Blue);
                LevelState.TrafficLights.Add(BallColors.Green);
                LevelState.TrafficLights.Add(BallColors.Pink);
                break;
            case 4:
                LevelState.TrafficLights.Add(BallColors.Green);
                LevelState.TrafficLights.Add(BallColors.Pink);
                LevelState.TrafficLights.Add(BallColors.Yellow);
                break;
            default:
                throw new Exception("Unhandled");
        }

        _ball1 = LevelTextures.SpinnerBall[(int)LevelState.TrafficLights[0]];
        _ball2 = LevelTextures.SpinnerBall[(int)LevelState.TrafficLights[1]];
        _ball3 = LevelTextures.SpinnerBall[(int)LevelState.TrafficLights[2]];
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        if (LevelState.TrafficLights.Count == 3 || DisableTraffic)
        {
            _spriteBatch.Draw(
                _ball1,
                (_position + b1Pos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (LevelState.TrafficLights.Count > 1 || DisableTraffic)
        {
            _spriteBatch.Draw(
                _ball2,
                (_position + b2Pos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (LevelState.TrafficLights.Count > 0 || DisableTraffic)
        {
            _spriteBatch.Draw(
                _ball3,
                (_position + b3Pos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
    }

    public override void Dispose()
    {
        if (theChosenOne.Equals(this))
            theChosenOne = null;
        base.Dispose();
    }
}