using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.Tiles;

public abstract class GameTile(Game game, string textureName, Point arrayPosition, byte xx, byte yy = 0)
    : SimpleImage(game,
        $"{Configs.GraphicSet}/{textureName}",
        new Vector2(16 + arrayPosition.X * 36, 46 + arrayPosition.Y * 36),
        0), ITile
{
    public byte FileValue { get; } = xx;
    public byte Argument { get; } = yy;
    public Point Point { get; } = arrayPosition;

    protected static Vector2 DetectionPoint { get; } = new(13f);

    public static explicit operator FileTile(GameTile gameTile)
        => new(gameTile.FileValue, gameTile.Argument, gameTile.Point);
}

public interface IReloadable
{
    public void Reload(ITile[,] tiles);
}

public interface IOverlayable
{
    public IEnumerable<GameComponent> Overlays { get; }
}