using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Block : Component
{
    #region Fields
    public readonly byte FileValue;
    public readonly bool HasArgument = false;
    public readonly byte Argument;
    protected Point Pos;
    #endregion

    #region Properties

    #endregion

    public Block(Point arrayPosition, byte xx, byte yy = 0)
    {
        Pos = arrayPosition;
        zIndex = 0;
        _position = new Vector2(16 + Pos.X * 36, 46 + Pos.Y * 36);
        FileValue = xx;
        if (yy != 0)
        {
            HasArgument = false;
            Argument = yy;
        }
        IsEnabled = true;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            Texture,
            _position * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            zIndex * 0.1f
        );
    }

    public override void Dispose()
    {
        
    }
}

public interface IReloadable
{
    public abstract void Reload(Block[,] blocks);
}

public interface IOverlayable
{
    public abstract IEnumerable<Component> GetOverlayables();
}