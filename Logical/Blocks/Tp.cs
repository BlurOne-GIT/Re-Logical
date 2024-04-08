using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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
    private bool _closedPipeLeft;
    private bool _closedPipeUp;
    private bool _closedPipeRight;
    private bool _closedPipeDown;
    private readonly Vector2 _shadowPos = new(10, 11);
    private readonly Vector2 _cplPos = new(0, 10);
    private readonly Vector2 _cprPos = new(26, 10);
    private readonly Vector2 _cpdPos = new(10, 26);
    private readonly Vector2 _cpuPos = new(10, 0);
    #endregion

    public Tp(Game game, Point arrayPosition, byte xx, byte yy):base(game, arrayPosition, xx, yy, false)
    {
        switch (xx)
        {
            case 0x08:
                _overlay = new SimpleImage(game, LevelResources.TpHorizontal, Position + new Vector2(7, 7), 9);
                if (FirstHorizontalTp is null)
                    FirstHorizontalTp = this;
                else if (SecondHorizontalTp is null)
                    SecondHorizontalTp = this;
                break;
            case 0x09:
                _overlay = new SimpleImage(game, LevelResources.TpVertical, Position + new Vector2(7, 7), 9);
                if (FirstVerticalTp is null)
                    FirstVerticalTp = this;
                else if (SecondVerticalTp is null)
                    SecondVerticalTp = this;
                break;
            case 0x0A:
                _overlay = new SimpleImage(game, LevelResources.TpCross, Position + new Vector2(7, 7), 9);
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
        _closedPipeLeft = Pos.X == 0 || (!HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue) && FileValue is 0x08 or 0x0A);
        _closedPipeUp = Pos.Y == 0 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue) && FileValue is 0x09 or 0x0A);
        _closedPipeRight = Pos.X == 7 || (!HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue) && FileValue is 0x08 or 0x0A);
        _closedPipeDown = Pos.Y == 4 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue) && FileValue is 0x09 or 0x0A);
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
        
        if (_closedPipeLeft) DrawAnotherTexture(LevelResources.PipeClosedLeft, _cplPos, 1);
        
        if (_closedPipeUp) DrawAnotherTexture(LevelResources.PipeClosedUp, _cpuPos, 1);
        
        if (_closedPipeRight) DrawAnotherTexture(LevelResources.PipeClosedRight, _cprPos, 1);
        
        if (_closedPipeDown) DrawAnotherTexture(LevelResources.PipeClosedDown, _cpdPos, 1);
        
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

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {_overlay};
}