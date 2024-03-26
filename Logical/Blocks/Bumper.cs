using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Bumper : Block, IUpdateable, IReloadable, IOverlayable
{
    #region Field
    private readonly Direction _direction;
    private Texture2D _shadow;
    private bool closedPipeLeft;
    private bool closedPipeUp;
    private bool closedPipeRight;
    private bool closedPipeDown;
    private readonly Vector2 shadowPos = new Vector2(12f, 13f);
    private readonly Vector2 cplPos = new Vector2(0f, 10f);
    private readonly Vector2 cprPos = new Vector2(26f, 10f);
    private readonly Vector2 cpdPos = new Vector2(10f, 26f);
    private readonly Vector2 cpuPos = new Vector2(10f, 0f);
    #endregion

    public Bumper(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        switch (xx)
        {
            case 0x0E: _direction = Direction.Right; break;
            case 0x0F: _direction = Direction.Left; break;
            case 0x10: _direction = Direction.Up; break;
            case 0x11: _direction = Direction.Down; break;
            default: throw new Exception("Unhandeled");
        }
        Texture = LevelTextures.PipeCross;
    }

    public void Update(GameTime gameTime)
    {
        foreach (Ball ball in Ball.AllBalls)
        {
            if (ball.Position == Statics.DetectionPoint + _position)
                ball.MovementDirection = _direction;
        }
    }

    public void Reload(Block[,] blocks)
    {
        closedPipeLeft = Pos.X == 0 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue);
        closedPipeUp = Pos.Y == 0 || !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue);
        closedPipeRight = Pos.X == 7 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue);
        closedPipeDown = Pos.Y == 4 || !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue);

        _shadow = !closedPipeRight && !closedPipeDown ? LevelTextures.HolderShadowCross : !closedPipeRight ? LevelTextures.HolderShadowHorizontal : !closedPipeDown ? LevelTextures.HolderShadowVertical : LevelTextures.HolderShadowEmpty;
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
        );
    }

    public IEnumerable<Component> GetOverlayables() => new Component[] {new SimpleImage(LevelTextures.Holder, _position + new Vector2(9f), 8), new SimpleImage(LevelTextures.Bumper[(int)_direction], _position + new Vector2(14f), 9)};
}