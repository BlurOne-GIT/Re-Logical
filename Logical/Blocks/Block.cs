using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public abstract class Block : SimpleImage
{
    #region Fields
    public readonly byte FileValue;
    public readonly bool HasArgument = false;
    public readonly byte Argument;
    protected Point Pos;
    #endregion

    public Block(Game game, Texture2D texture2D, Point arrayPosition, byte xx, byte yy = 0)
        : base(
            game,
            texture2D,
            new Vector2(16 + arrayPosition.X * 36, 46 + arrayPosition.Y * 36),
            0
            )
    {
        Pos = arrayPosition;
        FileValue = xx;
        
        if (yy == 0) 
            return;
        
        HasArgument = false;
        Argument = yy;
    }
}

public interface IReloadable
{
    public void Reload(Block[,] blocks);
}

public interface IOverlayable
{
    public IEnumerable<DrawableGameComponent> GetOverlayables();
}