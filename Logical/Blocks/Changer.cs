using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Changer : Pipe, IOverlayable
{
    #region Fields

    private readonly BallColors _ballColor;
    private readonly Texture2D _shadow;
    private readonly Texture2D _indicator;
    private readonly Vector2 _ciPos = new(12f);
    private readonly Vector2 _shadowPos = new(18f);
    #endregion

    public Changer(Game game, Point arrayPosition, byte xx, byte yy):base(game, arrayPosition, xx, yy, false)
    {
        _shadow = xx switch
        {
            0x0B => LevelResources.ChangerShadowHorizontal,
            0x0C => LevelResources.ChangerShadowVertical,
            0x0D => LevelResources.ChangerShadowCross,
            _ => throw new ArgumentException("Invalid Pipe direction")
        };
        _ballColor = (BallColors)Argument-1;
        _indicator = LevelResources.Indicator[Argument-1];
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            ball.BallColor = _ballColor;
    }


    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_indicator, _ciPos, 1);
        DrawAnotherTexture(_shadow, _shadowPos, 1);
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset("Changers");
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {new SimpleImage(
        Game, Game.Content.Load<Texture2D>("Changers"), Position + _ciPos, 9)
        {DefaultRectangle = new Rectangle(12 * (int)_ballColor, 0, 12, 12)}
    };
}