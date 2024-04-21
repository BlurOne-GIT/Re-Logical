using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Tp : Pipe, IReloadable, IOverlayable
{
    #region Field
    public static Tp FirstHorizontalTp;
    public static Tp SecondHorizontalTp;
    public static Tp FirstVerticalTp;
    public static Tp SecondVerticalTp;
    private readonly SimpleImage _overlay;
    //private Texture2D _shadow;
    private bool[] _closedPipes = new bool[4];
    //private readonly Vector2 _shadowOffset = new(10, 11);
    private static readonly Vector2[] ClosedPipeOffsets =
    {
        new(0, 10),  // Left
        new(10, 0),  // Up
        new(26, 10), // Right
        new(10, 26)  // Down
    };
    private static Texture2D[] _closedPipeTextures;
    #endregion

    public Tp(Game game, Point arrayPosition, byte xx, byte yy):base(game, arrayPosition, xx, yy, false)
    {
        switch (xx)
        {
            case 0x08:
                _overlay = new SimpleImage(game, Game.Content.Load<Texture2D>("TpHorizontal"), Position + new Vector2(7, 7), 9);
                if (FirstHorizontalTp is null)
                    FirstHorizontalTp = this;
                else if (SecondHorizontalTp is null)
                    SecondHorizontalTp = this;
                break;
            case 0x09:
                _overlay = new SimpleImage(game, Game.Content.Load<Texture2D>("TpVertical"), Position + new Vector2(7, 7), 9);
                if (FirstVerticalTp is null)
                    FirstVerticalTp = this;
                else if (SecondVerticalTp is null)
                    SecondVerticalTp = this;
                break;
            case 0x0A:
                _overlay = new SimpleImage(game, Game.Content.Load<Texture2D>("TpCross"), Position + new Vector2(7, 7), 9);
                if (FirstHorizontalTp is null)
                    FirstHorizontalTp = this;
                else if (SecondHorizontalTp is null)
                    SecondHorizontalTp = this;
                if (FirstVerticalTp is null)
                    FirstVerticalTp = this;
                else if (SecondVerticalTp is null)
                    SecondVerticalTp = this;
                break;
            default: throw new ArgumentException("Invalid Tp direction");
        }
    }

    protected override void LoadContent()
    {
        if (_closedPipeTextures is null)
        {
            _closedPipeTextures = new Texture2D[4];
            for (var i = 0; i < 4; i++)
                _closedPipeTextures[i] = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosed{(Direction)i}");
        }
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            if (ball.MovementDirection is Direction.Left or Direction.Right)
                if (FirstHorizontalTp.Equals(this))
                    ball.Position = DetectionPoint + SecondHorizontalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstHorizontalTp.Position;
            else
                if (FirstVerticalTp.Equals(this))
                    ball.Position = DetectionPoint + SecondVerticalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstVerticalTp.Position;
    }

    public void Reload(Block[,] blocks)
    {
        _closedPipes = new[]
        {
            Pos.X == 0 || (!HorizontalAttachables.Contains(blocks[Pos.X - 1, Pos.Y].FileValue) &&
                           FileValue is 0x08 or 0x0A), // Left
            Pos.Y == 0 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y - 1].FileValue) &&
                           FileValue is 0x09 or 0x0A), // Up
            Pos.X == 7 || (!HorizontalAttachables.Contains(blocks[Pos.X + 1, Pos.Y].FileValue) &&
                           FileValue is 0x08 or 0x0A), // Right
            Pos.Y == 4 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y + 1].FileValue) &&
                           FileValue is 0x09 or 0x0A)  // Down
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (FirstHorizontalTp == this)
            FirstHorizontalTp = null;
        else if (SecondHorizontalTp == this)
            SecondHorizontalTp = null;

        if (FirstVerticalTp == this)
            FirstVerticalTp = null;
        else if (SecondVerticalTp == this)
            SecondHorizontalTp = null;
        base.Dispose(disposing);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        for (int i = 0; i < 4; i++)
            if (_closedPipes[i])
                DrawAnotherTexture(_closedPipeTextures[i], ClosedPipeOffsets[i], 1);
        
        /* DISABLED
        _spriteBatch.Draw(
            _shadow,
            (Position + shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.2f
        );*/
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset(FileValue switch
        {
            0x08 => "TpHorizontal",
            0x09 => "TpVertical",
            0x0A => "TpCross",
            _ => throw new ArgumentException("Invalid Tp direction")
        });
        _closedPipeTextures = null;
        Game.Content.UnloadAssets(new []
        {
            $"{Configs.GraphicSet}/PipeClosedLeft", 
            $"{Configs.GraphicSet}/PipeClosedUp", 
            $"{Configs.GraphicSet}/PipeClosedRight", 
            $"{Configs.GraphicSet}/PipeClosedDown"  
        });
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {_overlay};
}