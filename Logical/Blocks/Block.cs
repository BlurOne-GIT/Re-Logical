using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public abstract class Block : SimpleImage
{
    #region Fields
    public readonly byte FileValue;
    public readonly bool HasArgument;
    protected readonly byte Argument;
    protected Point Pos;
    protected static readonly byte[] VerticalAttachables = {
        0x01,
        0x03,
        0x04,
        0x06,
        0x07,
        0x09,
        0x0A,
        0x0C,
        0x0D,
        0x0E,
        0x0F,
        0x10,
        0x11,
        0x16
    };
    protected static readonly byte[] HorizontalAttachables = {
        0x01,
        0x02,
        0x04,
        0x05,
        0x07,
        0x08,
        0x0A,
        0x0B,
        0x0D,
        0x0E,
        0x0F,
        0x10,
        0x11
    };

    protected static Vector2 DetectionPoint { get; } = new(13f);
    #endregion

    protected Block(Game game, Texture2D texture2D, Point arrayPosition, byte xx, byte yy = 0)
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
        
        HasArgument = true;
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