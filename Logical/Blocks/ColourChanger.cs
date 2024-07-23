using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class ColourChanger : Pipe, IOverlayable
{
    #region Fields

    private readonly BallColors _ballColor;
    private static Texture2D _shadow;
    private static Texture2D _indicator;
    private readonly Vector2 _indicatorOffset = new(12f);
    private readonly Rectangle _indicatorSource;
    private Vector2 _shadowOffset = new(18f);
    private Rectangle _shadowSource;
    #endregion

    public ColourChanger(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy)
    {
        _ballColor = (BallColors)Argument-1;
        _indicatorSource = new Rectangle(12 * (int)_ballColor, 0, 12, 12);
        _shadowSource = new Rectangle(3 + 13 * Variation, 2 + xx * 12, 10, 10);
    }

    protected override void LoadContent()
    {
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/ChangerShadows");
        _indicator ??= Game.Content.Load<Texture2D>("Indicators");
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            ball.BallColor = _ballColor;
    }


    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_indicator, _indicatorOffset, 1, _indicatorSource);
        DrawAnotherTexture(_shadow, _shadowOffset, 1, _shadowSource);
    }

    public override void Fix(IFixable.FidelityLevel _)
    {
        base.Fix(_);
        _shadowSource = new Rectangle(13 * Variation, FileValue * 12, 12, 12);
        _shadowOffset = new Vector2(15f, 16f);
    }

    protected override void UnloadContent()
    {
        _shadow = _indicator = null;
        Game.Content.UnloadAssets(new []{"ColourChangers", "Indicators"});
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] 
    {
        new SimpleImage(Game, "ColourChangers", Position + _indicatorOffset, 9)
        { DefaultRectangle = _indicatorSource }
    };
}