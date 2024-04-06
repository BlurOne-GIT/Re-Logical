using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Bumper : Block, IReloadable, IOverlayable
{
    #region Field
    private readonly Direction _direction;
    private Texture2D _shadow;
    private bool _closedPipeLeft;
    private bool _closedPipeUp;
    private bool _closedPipeRight;
    private bool _closedPipeDown;
    private readonly Vector2 _shadowPos = new(12f, 13f);
    private readonly Vector2 _cplPos = new(0f, 10f);
    private readonly Vector2 _cprPos = new(26f, 10f);
    private readonly Vector2 _cpdPos = new(10f, 26f);
    private readonly Vector2 _cpuPos = new(10f, 0f);
    #endregion

    public Bumper(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.PipeCross, arrayPosition, xx, yy)
    {
        _direction = xx switch
        {
            0x0E => Direction.Right,
            0x0F => Direction.Left,
            0x10 => Direction.Up,
            0x11 => Direction.Down,
            _ => throw new ArgumentException("Invalid Bumper direction")
        };
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == Statics.DetectionPoint + Position))
            ball.MovementDirection = _direction;
    }

    public void Reload(Block[,] blocks)
    {
        _closedPipeLeft = Pos.X == 0 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue);
        _closedPipeUp = Pos.Y == 0 || !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue);
        _closedPipeRight = Pos.X == 7 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue);
        _closedPipeDown = Pos.Y == 4 || !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue);

        _shadow = !_closedPipeRight && !_closedPipeDown ? LevelResources.HolderShadowCross : !_closedPipeRight ? LevelResources.HolderShadowHorizontal : !_closedPipeDown ? LevelResources.HolderShadowVertical : LevelResources.HolderShadowEmpty;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        if (_closedPipeLeft)
        {
            spriteBatch.Draw(
                LevelResources.PipeClosedLeft,
                (Position + _cplPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedPipeUp)
        {
            spriteBatch.Draw(
                LevelResources.PipeClosedUp,
                (Position + _cpuPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedPipeRight)
        {
            spriteBatch.Draw(
                LevelResources.PipeClosedRight,
                (Position + _cprPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedPipeDown)
        {
            spriteBatch.Draw(
                LevelResources.PipeClosedDown,
                (Position + _cpdPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        spriteBatch.Draw(
            _shadow,
            (Position + _shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.2f
        );
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {new SimpleImage(Game, LevelResources.Holder, Position + new Vector2(9f), 8), new SimpleImage(Game, LevelResources.Bumper[(int)_direction], Position + new Vector2(14f), 9)};
}