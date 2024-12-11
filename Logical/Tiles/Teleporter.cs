using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Tiles;

public class Teleporter : Pipe, IReloadable, IOverlayable, IFixable
{
    public static Teleporter FirstHorizontalTp;
    public static Teleporter SecondHorizontalTp;
    public static Teleporter FirstVerticalTp;
    public static Teleporter SecondVerticalTp;
    private readonly SimpleImage _overlay;
    private static Texture2D _shadow;
    private static readonly Vector2 ShadowOffset = new(10, 19);
    private Rectangle _shadowSource;
    private int _shadowNum;
    private bool _completelyOpen;
    private Texture2D _pipeClosings;
    private Vector2 _closingsOffset;
    private Rectangle _closingsSource;

    public Teleporter(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy)
    {
        _overlay = new SimpleImage(game, "Teleporters", Position + new Vector2(7, 7), 9)
            { DefaultSource = new Rectangle((xx - 0x08) * 22, 0, 22, 22)};
        
        if (xx is not (byte)ITile.TileTypes.VerticalTeleporter)
            if (FirstHorizontalTp is null)
                FirstHorizontalTp = this;
            else if (SecondHorizontalTp is null)
                SecondHorizontalTp = this;
            
        if (xx is not (byte)ITile.TileTypes.HorizontalTeleporter)
            if (FirstVerticalTp is null)
                FirstVerticalTp = this;
            else if (SecondVerticalTp is null)
                SecondVerticalTp = this;
    }

    protected override void LoadContent()
    {
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/TeleporterShadows");
        _pipeClosings = Texture;
        Texture = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/EmptyTile");
        _closingsSource = DefaultSource!.Value;
        if (Configs.GraphicSet < 4)
            DefaultSource = new Rectangle(Configs.GraphicSet switch
            {
                1 or 3 => 0,
                2 => 36,
                _ => throw new ArgumentException("Invalid GraphicsSet")
            }, 0, 36, 36);
        Overlays = new DrawableGameComponent[] { _overlay };
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // TODO: add case where there is no second teleporter and the ball explodes
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            if (ball.MovementDirection is Direction.Left or Direction.Right)
                if (FirstHorizontalTp == this)
                    ball.Position = DetectionPoint + SecondHorizontalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstHorizontalTp.Position;
            else
                if (FirstVerticalTp == this)
                    ball.Position = DetectionPoint + SecondVerticalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstVerticalTp.Position;
    }

    public void Reload(ITile[,] tiles)
    {
         var closedPipes = new[]
        {
            Orientation is not Orientations.Vertical &&
            (Point.X is 0 || !ITile.HorizontalAttachables.Contains(tiles[Point.X - 1, Point.Y].FileValue)), // Left
            Orientation is not Orientations.Horizontal &&
            (Point.Y is 0 || !ITile.VerticalAttachables.Contains(tiles[Point.X, Point.Y - 1].FileValue)), // Up
            Orientation is not Orientations.Vertical &&
            (Point.X is 7 || !ITile.HorizontalAttachables.Contains(tiles[Point.X + 1, Point.Y].FileValue)), // Right
            Orientation is not Orientations.Horizontal &&
            (Point.Y is 4 || !ITile.VerticalAttachables.Contains(tiles[Point.X, Point.Y + 1].FileValue)) // Down
        };
        
        _shadowNum = (closedPipes[(int)Direction.Right] ? 1 : 0) | (closedPipes[(int)Direction.Down] ? 2 : 0);
        _shadowSource = new Rectangle(_shadowNum * 22 + Variation * 88, (int)Orientation * 14, 22, 14);
        _completelyOpen = closedPipes.All(x => !x);
        
        if (_completelyOpen)
        {
            Texture = _pipeClosings;
            DefaultSource = _closingsSource;
            return;
        }
        
        if (Configs.GraphicSet >= 4)
            DefaultSource = new Rectangle(
                36 - (closedPipes[(int)Direction.Left] ? 36 : 0) + (closedPipes[(int)Direction.Right] ? 36 : 0),
                closedPipes[(int)Direction.Left] || closedPipes[(int)Direction.Right] ? 36 : 0,
                36, 36
            );
        
        if (closedPipes[(int)Direction.Left])
        {
            _closingsOffset.X = 10f;
            _closingsSource.X += 10;
            _closingsSource.Width -= 10;
        }
        
        if (closedPipes[(int)Direction.Up])
        {
            _closingsOffset.Y = 10f;
            _closingsSource.Y += 10;
            _closingsSource.Height -= 10;
        }
        
        if (closedPipes[(int)Direction.Right]) _closingsSource.Width -= 10;
        if (closedPipes[(int)Direction.Down]) _closingsSource.Height -= 10;
    }

    protected override void Dispose(bool disposing)
    {
        if (FirstHorizontalTp == this)
            FirstHorizontalTp = null;
        else if (SecondHorizontalTp == this)
            SecondHorizontalTp = null;

        if (FirstVerticalTp == this)
            FirstVerticalTp = null;
        else if (SecondVerticalTp == this)
            SecondVerticalTp = null;
        
        base.Dispose(disposing);
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
            "Teleporters",
            $"{Configs.GraphicSet}/TeleporterShadows", 
            $"{Configs.GraphicSet}/PipeClosings"
        ]);
        base.UnloadContent();
    }

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;

    public void Fix(IFixable.FidelityLevel fidelity)
    {
        if (FileValue is 0x0A)
            _overlay.DefaultSource = new Rectangle(66, 0, 22, 22);
        
        if (Configs.GraphicSet is 1 || _completelyOpen) return;
        
        Texture = _pipeClosings;
        _pipeClosings = Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosings");
        DefaultSource = new Rectangle(
            _closingsSource.X - (int)_closingsOffset.X,
            _closingsSource.Y - (int)_closingsOffset.Y,
            36, 36
        );
        _closingsSource.Offset(-_closingsOffset);
        _closingsOffset = new Vector2(10) - _closingsOffset;
        _closingsSource.Offset(_closingsOffset);
        _closingsSource.Width += 2*(26 - _closingsSource.Width);
        _closingsSource.Height += 2*(26 - _closingsSource.Height);
        _shadowSource.Y += 42;
    }

    public IEnumerable<GameComponent> Overlays { get; private set; }
}