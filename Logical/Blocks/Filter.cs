using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Filter : Pipe, IOverlayable
{
    #region Fields
    private readonly BallColors _ballColor;
    private readonly Texture2D _shadow;
    private static Texture2D _balls;
    private static readonly Vector2 BallPos = new(14f);
    private static readonly Vector2 ShadowPos = new(10f, 11f);
    private readonly Rectangle _rectangle;
    #endregion

    public Filter(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy, false)
    {
        _shadow = xx switch
        {
            0x05 => LevelResources.FilterShadowHorizontal,
            0x06 => LevelResources.FilterShadowVertical,
            0x07 => LevelResources.FilterShadowCross,
            _ => throw new ArgumentException("Invalid Pipe direction")
        };
        _ballColor = (BallColors)Argument-1;
        _rectangle = new Rectangle(8 * (int)_ballColor, 0, 8, 8);
    }

    protected override void LoadContent()
    {
        _balls = Game.Content.Load<Texture2D>("SpinnerBalls");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position && ball.BallColor != _ballColor))
            ball.Bounce();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_balls, BallPos, 1, _rectangle);
        DrawAnotherTexture(_shadow, ShadowPos, 1);
    }

    protected override void UnloadContent()
    {
        _balls = null;
        Game.Content.UnloadAsset("SpinnerBalls");
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {new SimpleImage(Game, LevelResources.Filter[Argument-1], Position + new Vector2(7f), 9)};
}