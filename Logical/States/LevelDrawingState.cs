using Logical.Tiles;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.States;

public class LevelDrawingState : GameState
{
    public Level Level { get; }
    public GameTile[,] Tileset { get; } = new GameTile[8, 5];


    public LevelDrawingState(Game game) : this(game, Statics.CurrentLevel) {}
    
    public LevelDrawingState(Game game, Level level) : base(game)
    {
        Level = level;
        
        // FileTiles to GameTiles, then add to tileset, to components, enable (or not), reload, fix, and add overlays
        for (var i = 0; i < Level.Tiles.Length; i++)
        {
            var x = i % 8; var y = i / 8;
            var tile = ((FileTile)Level.Tiles[x, y]).ToGameTile(game);
            
            tile.Enabled = false;
            Components.Add(Tileset[x, y] = tile);
            
            if (tile is IReloadable reloadable)
                reloadable.Reload(Level.Tiles);

            if (tile is IFixable fixable && fixable.ShallFix(Configs.FidelityLevel))
                fixable.Fix(Configs.FidelityLevel);

            if (tile is not IOverlayable overlayable) continue;
            foreach (var component in overlayable.Overlays)
                Components.Add(component);
        }
        
        // Main pipe
        Components.Add(
            new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipe", new Vector2(16, 30), 0)
                { Enabled = false }
        );
        
        // Main pipe openings
        var intendedPipes = Configs.FidelityLevel >= IFixable.FidelityLevel.Intended;
        for (var x = 0; x < 8; x++)
            if (Level.Tiles[x, 0].TileType is ITile.TileTypes.Spinner or ITile.TileTypes.Dropper)
                Components.Add(
                    new SimpleImage(Game, $"{Configs.GraphicSet}/MainPipeOpen",
                            new Vector2(25 + 36 * x, intendedPipes ? 40 : 41), 1)
                    {
                        Enabled = false,
                        DefaultSource = new Rectangle(0, intendedPipes ? 0 : 1, 18, intendedPipes ? 6 : 5)
                    }
                );
    }
}