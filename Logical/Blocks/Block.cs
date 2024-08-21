using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.Blocks;

public abstract class Block(Game game, string textureName, Point arrayPosition, byte xx, byte yy = 0)
    : SimpleImage(game,
        $"{Configs.GraphicSet}/{textureName}",
        new Vector2(16 + arrayPosition.X * 36, 46 + arrayPosition.Y * 36),
        0), IBlock
{
    #region Fields
    public byte FileValue { get; } = xx;
    public byte Argument { get; } = yy;
    public Point Point { get; } = arrayPosition;

    protected static Vector2 DetectionPoint { get; } = new(13f);
    #endregion

    public static explicit operator FileBlock(Block block)
        => new(block.FileValue, block.Argument, block.Point);
}

public interface IReloadable
{
    public void Reload(IBlock[,] blocks);
}

public interface IOverlayable
{
    public IEnumerable<GameComponent> GetOverlayables();
}