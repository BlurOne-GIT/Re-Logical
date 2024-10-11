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

    private Texture2D _pipeClosings;
    private Vector2 _closingsOffset;
    private Rectangle _closingsSource;
    private bool _completelyOpen;
    private SimpleImage _holder;
    private SimpleImage _arrow;

    #endregion

    public DirectionArrow(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "EmptyBlock", arrayPosition, xx, yy)
    {
        _direction = xx switch
        {
            0x0E => Direction.Right,
            0x0F => Direction.Left,
            0x10 => Direction.Up,
            0x11 => Direction.Down,
            _ => throw new ArgumentException("Invalid Bumper direction")
        };
        DefaultSource = new Rectangle(Configs.GraphicSet switch
        {
            1 or 3 => 0,
            2 or 4 or 5 => 36,
            _ => throw new ArgumentException("Invalid GraphicsSet")
        }, 0, 36, 36);
    }

    protected override void LoadContent()
    {
        _pipeClosings = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Pipes");
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

    public void Reload(IBlock[,] blocks)
    {
        var closedPipes = new[]
        {
            Point.X is 0 || !IBlock.HorizontalAttachables.Contains(blocks[Point.X - 1, Point.Y].FileValue), // Left
            Point.Y is 0 || !IBlock.VerticalAttachables.Contains(blocks[Point.X, Point.Y - 1].FileValue),  // Up
            Point.X is 7 || !IBlock.HorizontalAttachables.Contains(blocks[Point.X + 1, Point.Y].FileValue), // Right
            Point.Y is 4 || !IBlock.VerticalAttachables.Contains(blocks[Point.X, Point.Y + 1].FileValue) // Down
        };

        var shadowNum = (closedPipes[(int)Direction.Right] ? 1 : 0) | (closedPipes[(int)Direction.Down] ? 2 : 0);
        _shadowSource = new Rectangle(0, shadowNum * 18, 18, 18);
        
        if (_completelyOpen = closedPipes.All(x => !x))
        {
            Texture = _pipeClosings;
            return;
        }
        
        _closingsSource = new Rectangle(
            closedPipes[(int)Direction.Left] ? 10 : 0,
            closedPipes[(int)Direction.Up] ? 46 : 36,
            16 + (closedPipes[(int)Direction.Left] ? 0 : 10) + (closedPipes[(int)Direction.Right] ? 0 : 10),
            16 + (closedPipes[(int)Direction.Up] ? 0 : 10) + (closedPipes[(int)Direction.Down] ? 0 : 10)
        );
        _closingsOffset = new Vector2(_closingsSource.X, _closingsSource.Y - 36);
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
        _shadow = null;
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

        if (fidelity < IFixable.FidelityLevel.Intended || Configs.GraphicSet is 1 || _completelyOpen)
            return;

        Texture = _pipeClosings;
        DefaultSource = new Rectangle(0, 36, 36, 36);
        _pipeClosings = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosings");
        _closingsOffset.X = _closingsSource.X = 10 - _closingsSource.X;
        _closingsOffset.Y = 10 - _closingsOffset.Y;
        _closingsSource.Y = 82 - _closingsSource.Y;
        _closingsSource.Width += 2*(26 - _closingsSource.Width);
        _closingsSource.Height += 2*(26 - _closingsSource.Height);
    }
}