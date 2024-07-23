using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class ColourStopper : Pipe, IOverlayable
{
    #region Fields
    private readonly BallColors _ballColor;
    private static Texture2D _shadow;
    private static Texture2D _balls;
    private static readonly Vector2 BallOffset = new(14f);
    private static readonly Vector2 ShadowOffset = new(10f, 11f);
    private readonly Rectangle _ballSource;
    private Rectangle _shadowSource;
    #endregion

    public ColourStopper(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy)
    {
        _ballColor = (BallColors)Argument-1;
        _ballSource = new Rectangle(8 * (int)_ballColor, 0, 8, 8);
        _shadowSource = new Rectangle(22 * Variation, FileValue * 14, 22, 14);
    }

    protected override void LoadContent()
    {
        _balls = Game.Content.Load<Texture2D>("SpinnerBalls");
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/StopperShadows");
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
        
        DrawAnotherTexture(_balls, BallOffset, 1, _ballSource);
        DrawAnotherTexture(_shadow, ShadowOffset, 1);
    }

    protected override void UnloadContent()
    {
        _balls = null;
        _shadow = null;
        Game.Content.UnloadAssets(new []
        {
            "SpinnerBalls",
            "ColourStoppers",
            $"{Configs.GraphicSet}/StopperShadows"
        });
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {
        new SimpleImage(Game,  "ColourStoppers", Position + new Vector2(7f), 9)
        {DefaultRectangle = new Rectangle(22 * (int)_ballColor, 0, 22, 22)}
    };
}