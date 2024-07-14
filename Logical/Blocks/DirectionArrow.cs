using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class DirectionArrow : Block, IReloadable, IOverlayable, IFixable
{
    #region Field
    private readonly Direction _direction;
    private Texture2D _shadow;
    private bool[] _closedPipes = new bool[4];
    private readonly Vector2 _shadowOffset = new(12f, 13f);
    private static readonly Vector2[] ClosedPipeOffsets =
    {
        new( 0f, 10f), // Left
        new(10f,  0f), // Up
        new(26f, 10f), // Right
        new(10f, 26f)  // Down
    };

    private static Texture2D[] _closedPipeTextures;

    private SimpleImage _holder;
    private SimpleImage _arrow;
    #endregion

    public DirectionArrow(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "PipeCross", arrayPosition, xx, yy)
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

    protected override void LoadContent()
    {
        if (_closedPipeTextures is null)
        {
            _closedPipeTextures = new Texture2D[4];
            for (var i = 0; i < 4; i++)
                _closedPipeTextures[i] = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosed{(Direction)i}");
        }
        base.LoadContent();
        _holder = new SimpleImage(Game, "Holder", Position + new Vector2(9f), 8)
            { DefaultRectangle = new Rectangle(0, 1, 18, 17)};
        _arrow = new SimpleImage(Game, $"{Configs.GraphicSet}/DirectionArrows", Position + new Vector2(13f, 12f), 9)
            { DefaultRectangle = new Rectangle(9 * (int)_direction, 0, 10, 10) };
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            ball.MovementDirection = _direction;
    }

    public void Reload(Block[,] blocks)
    {
        _closedPipes = new[]
        {
            Pos.X == 0 || !HorizontalAttachables.Contains(blocks[Pos.X - 1, Pos.Y].FileValue), // Left
            Pos.Y == 0 || !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y - 1].FileValue),  // Up
            Pos.X == 7 || !HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue), // Right
            Pos.Y == 4 || !VerticalAttachables.Contains(blocks[Pos.X, Pos.Y + 1].FileValue) // Down
        };

        _shadow = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/HolderShadow" + 
            (!_closedPipes[(int)Direction.Right] && !_closedPipes[(int)Direction.Down] ? "Cross" :
            !_closedPipes[(int)Direction.Right] ? "Horizontal" :
            !_closedPipes[(int)Direction.Down] ? "Vertical" : 
            "Empty")
        );
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        for (var i = 0; i < 4; i++)
            if (_closedPipes[i])
                DrawAnotherTexture(_closedPipeTextures[i], ClosedPipeOffsets[i], 1);
        
        DrawAnotherTexture(_shadow, _shadowOffset, 2);
    }

    protected override void UnloadContent()
    {
        _closedPipeTextures = null;
        Game.Content.UnloadAssets(new []
        {
            $"{Configs.GraphicSet}/PipeClosedLeft", 
            $"{Configs.GraphicSet}/PipeClosedUp", 
            $"{Configs.GraphicSet}/PipeClosedRight", 
            $"{Configs.GraphicSet}/PipeClosedDown",
            "Holder",
            $"{Configs.GraphicSet}/DirectionArrows",
            _shadow.Name
        });
        base.UnloadContent();
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables()
        => new DrawableGameComponent[] { _holder, _arrow };

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;
    
    public void Fix(IFixable.FidelityLevel fidelity)
    {
        _holder.DefaultRectangle = null;
        _arrow.Position += Vector2.UnitY;
    }
}