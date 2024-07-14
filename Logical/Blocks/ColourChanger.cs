using System;
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
    private readonly Texture2D _shadow;
    private readonly Texture2D _indicator;
    private readonly Vector2 _indicatorOffset = new(12f);
    private readonly Rectangle _indicatorRectangle;
    private readonly Vector2 _shadowOffset = new(18f);
    #endregion

    public ColourChanger(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy)
    {
        _shadow = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/{ShadowTextureSwitcher(xx)}");
        _ballColor = (BallColors)Argument-1;
        _indicator = Game.Content.Load<Texture2D>("Indicators");
        _indicatorRectangle = new Rectangle(12 * (int)_ballColor, 0, 12, 12);
    }
    
    private static string ShadowTextureSwitcher(byte xx) => xx switch
    {
        0x0B => "ChangerShadowHorizontal",
        0x0C => "ChangerShadowVertical",
        0x0D => "ChangerShadowCross",
        _ => throw new ArgumentException("Invalid Pipe direction")
    };
    
    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            ball.BallColor = _ballColor;
    }


    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_indicator, _indicatorOffset, 1, _indicatorRectangle);
        DrawAnotherTexture(_shadow, _shadowOffset, 1);
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAssets(new []{"Changers", "Indicators"});
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] 
    {
        new SimpleImage(Game, "Changers", Position + _indicatorOffset, 9)
        {DefaultRectangle = _indicatorRectangle}
    };
}