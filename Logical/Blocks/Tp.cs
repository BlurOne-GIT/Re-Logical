using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Tp : Block, IUpdateable, IReloadable, IOverlayable
{
    #region Field
    public static Tp FirstHorizontalTp;
    public static Tp SecondHorizontalTp;
    public static Tp FirstVerticalTp;
    public static Tp SecondVerticalTp;
    private readonly SimpleImage Overlay;
    //private Texture2D _shadow;
    private bool closedPipeLeft;
    private bool closedPipeUp;
    private bool closedPipeRight;
    private bool closedPipeDown;
    private readonly Vector2 shadowPos = new Vector2(10, 11);
    private readonly Vector2 cplPos = new Vector2(0, 10);
    private readonly Vector2 cprPos = new Vector2(26, 10);
    private readonly Vector2 cpdPos = new Vector2(10, 26);
    private readonly Vector2 cpuPos = new Vector2(10, 0);
    #endregion

    public Tp(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        switch (xx)
        {
            case 0x08:
                Texture = LevelTextures.PipeHorizontal;
                Overlay = new SimpleImage(LevelTextures.TpHorizontal, (_position + new Vector2(7, 7)), 9);
                if (FirstHorizontalTp is null)
                    FirstHorizontalTp = this;
                else if (SecondHorizontalTp is null)
                    SecondHorizontalTp = this;
                break;
            case 0x09:
                Texture = LevelTextures.PipeVertical;
                Overlay = new SimpleImage(LevelTextures.TpVertical, (_position + new Vector2(7, 7)), 9);
                if (FirstVerticalTp is null)
                    FirstVerticalTp = this;
                else if (SecondVerticalTp is null)
                    SecondVerticalTp = this;
                break;
            case 0x0A:
                Texture = LevelTextures.PipeCross;
                Overlay = new SimpleImage(LevelTextures.TpCross, (_position + new Vector2(7, 7)), 9);
                if (FirstHorizontalTp is null)
                    FirstHorizontalTp = this;
                else if (SecondHorizontalTp is null)
                    SecondHorizontalTp = this;
                if (FirstVerticalTp is null)
                    FirstVerticalTp = this;
                else if (SecondVerticalTp is null)
                    SecondVerticalTp = this;
                break;
            default: throw new Exception("Unhandeled");
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach(Ball ball in Ball.AllBalls)
        {
            if (ball.Position == Statics.DetectionPoint + _position)
            {
                if (ball.MovementDirection is Direction.Left or Direction.Right)
                {
                    if (FirstHorizontalTp.Equals(this))
                        ball.Position = Statics.DetectionPoint + SecondHorizontalTp._position;
                    else
                        ball.Position = Statics.DetectionPoint + FirstHorizontalTp._position;    
                } else
                {
                    if (FirstVerticalTp.Equals(this))
                        ball.Position = Statics.DetectionPoint + SecondVerticalTp._position;
                    else
                        ball.Position = Statics.DetectionPoint + FirstVerticalTp._position;
                }
            }
        }
    }

    public void Reload(Block[,] blocks)
    {
        closedPipeLeft = Pos.X == 0 || (!Statics.HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue) && FileValue is 0x08 or 0x0A);
        closedPipeUp = Pos.Y == 0 || (!Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue) && FileValue is 0x09 or 0x0A);
        closedPipeRight = Pos.X == 7 || (!Statics.HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue) && FileValue is 0x08 or 0x0A);
        closedPipeDown = Pos.Y == 4 || (!Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue) && FileValue is 0x09 or 0x0A);
    }

    public override void Dispose()
    {
        base.Dispose();
        if (FirstHorizontalTp == this)
            FirstHorizontalTp = null;
        else if (SecondHorizontalTp == this)
            SecondHorizontalTp = null;

        if (FirstVerticalTp == this)
            FirstVerticalTp = null;
        else if (SecondVerticalTp == this)
            SecondHorizontalTp = null;
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        if (closedPipeLeft)
        {
            _spriteBatch.Draw(
                LevelTextures.PipeClosedLeft,
                (_position + cplPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedPipeUp)
        {
            _spriteBatch.Draw(
                LevelTextures.PipeClosedUp,
                (_position + cpuPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedPipeRight)
        {
            _spriteBatch.Draw(
                LevelTextures.PipeClosedRight,
                (_position + cprPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedPipeDown)
        {
            _spriteBatch.Draw(
                LevelTextures.PipeClosedDown,
                (_position + cpdPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        /* DISABLED
        _spriteBatch.Draw(
            _shadow,
            (_position + shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.2f
        );*/
    }

    public IEnumerable<Component> GetOverlayables() => new Component[] {Overlay};
}