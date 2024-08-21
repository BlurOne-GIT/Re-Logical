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
    private static Texture2D _shadow;
    private static readonly Vector2 ShadowOffset = new(12f, 13f);
    private Rectangle _shadowSource;

    private static Texture2D _pipeClosings;
    private Vector2 _closingsOffset;
    private Rectangle _closingsSource;
    private bool _completelyOpen;
    private SimpleImage _holder;
    private SimpleImage _arrow;

    #endregion

    public DirectionArrow(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "Pipes", arrayPosition, xx, yy)
    {
        _direction = xx switch
        {
            0x0E => Direction.Right,
            0x0F => Direction.Left,
            0x10 => Direction.Up,
            0x11 => Direction.Down,
            _ => throw new ArgumentException("Invalid Bumper direction")
        };
        DefaultSource = new Rectangle(0, 72, 36, 36);
    }

    protected override void LoadContent()
    {
        _pipeClosings ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosings");
        base.LoadContent();
        _holder = new SimpleImage(Game, "Holder", Position + new Vector2(9f), 8)
            { DefaultSource = new Rectangle(0, 1, 18, 17)};
        _arrow = new SimpleImage(Game, $"{Configs.GraphicSet}/DirectionArrows", Position + new Vector2(13f, 12f), 9)
            { DefaultSource = new Rectangle(9 * (int)_direction, 0, 10, 10) };
        _shadow = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/HolderShadows");
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            ball.MovementDirection = _direction;
    }

    public void Reload(Block[,] blocks)
    {
        var closedPipes = new[]
        {
            Point.X == 0 || !IBlock.HorizontalAttachables.Contains(blocks[Point.X - 1, Point.Y].FileValue), // Left
            Point.Y == 0 || !IBlock.VerticalAttachables.Contains(blocks[Point.X, Point.Y - 1].FileValue),  // Up
            Point.X == 7 || !IBlock.HorizontalAttachables.Contains(blocks[Point.X+1, Point.Y].FileValue), // Right
            Point.Y == 4 || !IBlock.VerticalAttachables.Contains(blocks[Point.X, Point.Y + 1].FileValue) // Down
        };

        var shadowNum = (closedPipes[(int)Direction.Right] ? 1 : 0) | (closedPipes[(int)Direction.Down] ? 2 : 0);
        _shadowSource = new Rectangle(0, shadowNum * 18, 18, 18);

        _completelyOpen = closedPipes.All(x => !x);
        if (_completelyOpen) return;
        
        _closingsSource = new Rectangle(
            closedPipes[(int)Direction.Left] ? 0 : 10,
            closedPipes[(int)Direction.Up] ? 0 : 10,
            36 - (closedPipes[(int)Direction.Left] ? 0 : 10) - (closedPipes[(int)Direction.Right] ? 0 : 10),
            36 - (closedPipes[(int)Direction.Up] ? 0 : 10) - (closedPipes[(int)Direction.Down] ? 0 : 10)
        );
        _closingsOffset = _closingsSource.Location.ToVector2();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        // Pipe Closings
        if (!_completelyOpen)
            DrawAnotherTexture(_pipeClosings, _closingsOffset, 1, _closingsSource);
        
        // Shadow
        DrawAnotherTexture(_shadow, ShadowOffset, 2, _shadowSource);
    }

    protected override void UnloadContent()
    {
        _pipeClosings = _shadow = null;
        Game.Content.UnloadAssets([
            $"{Configs.GraphicSet}/PipeClosings",
            "Holder",
            $"{Configs.GraphicSet}/DirectionArrows",
            $"{Configs.GraphicSet}/HolderShadows"
        ]);
        base.UnloadContent();
    }

    public IEnumerable<GameComponent> GetOverlayables()
        => new DrawableGameComponent[] { _holder, _arrow };

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;
    
    public void Fix(IFixable.FidelityLevel fidelity)
    {
        _holder.DefaultSource = null;
        _arrow.Position += Vector2.UnitY;

        if (fidelity is not IFixable.FidelityLevel.Remastered) return;

        var variation = Statics.Brandom.Next(3);
        DefaultSource = new Rectangle(72, variation * 36, 36, 36);
        _shadowSource.X = 18 * variation;
        if (!_completelyOpen)
            _closingsSource.X += 36 * variation;
    }
}