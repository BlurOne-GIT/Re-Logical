using Logical.Blocks;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.States;

public class LevelDrawingState : GameState
{
    public Level Level { get; }
    public Block[,] Tileset { get; } = new Block[8, 5];


    public LevelDrawingState(Game game) : this(game, Statics.CurrentLevel) {}
    
    public LevelDrawingState(Game game, Level level) : base(game)
    {
        Level = level;
        
        // FileBlocks to Blocks, then add to tileset, to components, enable (or not), reload, fix, and add overlays
        for (var i = 0; i < Level.Blocks.Length; i++)
        {
            var x = i % 8; var y = i / 8;
            var block = ((FileBlock)Level.Blocks[x, y]).ToGameBlock(game);
            
            block.Enabled = false;
            Components.Add(Tileset[x, y] = block);
            
            if (block is IReloadable reloadable)
                reloadable.Reload(Level.Blocks);

            if (block is IFixable fixable && fixable.ShallFix(Configs.FidelityLevel))
                fixable.Fix(Configs.FidelityLevel);

            if (block is not IOverlayable overlayable) continue;
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
            if (Level.Blocks[x, 0].BlockType is IBlock.BlockTypes.Spinner or IBlock.BlockTypes.Dropper)
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