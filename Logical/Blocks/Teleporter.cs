using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Teleporter : Pipe, IReloadable, IOverlayable
{
    #region Field
    public static Teleporter FirstHorizontalTp;
    public static Teleporter SecondHorizontalTp;
    public static Teleporter FirstVerticalTp;
    public static Teleporter SecondVerticalTp;
    private readonly SimpleImage _overlay;
    private static Texture2D _shadow;
    private static readonly Vector2 ShadowOffset = new(10, 19);
    private Rectangle _shadowSource;
    private static readonly Vector2[] ClosedPipeOffsets =
    {
        new(0, 10),  // Left
        new(10, 0),  // Up
        new(26, 10), // Right
        new(10, 26)  // Down
    };
    private int _shadowNum;
    private bool _completelyOpen;
    private static Texture2D _pipeClosings;
    private Vector2 _closingsOffset;
    private Rectangle _closingsSource;
    
    #endregion

    public Teleporter(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy)
    {
        _overlay = new SimpleImage(game, "Teleporters", Position + new Vector2(7, 7), 9)
            { DefaultSource = new Rectangle((xx - 0x08) * 22, 0, 22, 22)};
        
        if (xx is not 0x09)
            if (FirstHorizontalTp is null)
                FirstHorizontalTp = this;
            else if (SecondHorizontalTp is null)
                SecondHorizontalTp = this;
            
        if (xx is not 0x08)
            if (FirstVerticalTp is null)
                FirstVerticalTp = this;
            else if (SecondVerticalTp is null)
                SecondVerticalTp = this;
    }

    protected override void LoadContent()
    {
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/TeleporterShadows");
        _pipeClosings ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/PipeClosings");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // TODO: add case where there is no second teleporter and the ball explodes
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == DetectionPoint + Position))
            if (ball.MovementDirection is Direction.Left or Direction.Right)
                if (FirstHorizontalTp.Equals(this))
                    ball.Position = DetectionPoint + SecondHorizontalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstHorizontalTp.Position;
            else
                if (FirstVerticalTp.Equals(this))
                    ball.Position = DetectionPoint + SecondVerticalTp.Position;
                else
                    ball.Position = DetectionPoint + FirstVerticalTp.Position;
    }

    public void Reload(Block[,] blocks)
    {
         var closedPipes = new[]
        {
            FileValue is not 0x09 &&
            (Pos.X is 0 || (!HorizontalAttachables.Contains(blocks[Pos.X - 1, Pos.Y].FileValue) &&
                            FileValue is 0x08 or 0x0A)), // Left
            FileValue is not 0x08 &&
            Pos.Y is 0 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y - 1].FileValue) &&
                           FileValue is 0x09 or 0x0A), // Up
            FileValue is not 0x09 &&
            Pos.X is 7 || (!HorizontalAttachables.Contains(blocks[Pos.X + 1, Pos.Y].FileValue) &&
                           FileValue is 0x08 or 0x0A), // Right
            FileValue is not 0x08 &&
            Pos.Y is 4 || (!VerticalAttachables.Contains(blocks[Pos.X, Pos.Y + 1].FileValue) &&
                           FileValue is 0x09 or 0x0A)  // Down
        };
        
        _shadowNum = (closedPipes[(int)Direction.Right] ? 1 : 0) | (closedPipes[(int)Direction.Down] ? 2 : 0);
        _shadowSource = new Rectangle(_shadowNum * 22 + Variation * 42, FileValue * 14, 22, 14);
         
        _completelyOpen = closedPipes.All(x => !x);
        if (_completelyOpen) return;
        
        _closingsSource = new Rectangle(
            closedPipes[(int)Direction.Left] ? 0 : 10,
            closedPipes[(int)Direction.Up] ? 0 : 10,
            36 - (closedPipes[(int)Direction.Left] ? 0 : 10) - (closedPipes[(int)Direction.Right] ? 0 : 10),
            36 - (closedPipes[(int)Direction.Up] ? 0 : 10) - (closedPipes[(int)Direction.Down] ? 0 : 10)
        );
        _closingsOffset = _closingsSource.Location.ToVector2();
        _closingsSource.X += Variation * 36;
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
            SecondHorizontalTp = null;
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
        _pipeClosings = _shadow = null;
        Game.Content.UnloadAssets(new []
        {
            "Teleporters",
            $"{Configs.GraphicSet}/TeleporterShadows", 
            $"{Configs.GraphicSet}/PipeClosings"
        });
        base.UnloadContent();
    }

    public IEnumerable<GameComponent> GetOverlayables()
        => new DrawableGameComponent[] { _overlay };

    public override IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;
    
    public override void Fix(IFixable.FidelityLevel fidelity)
    {
        if (FileValue is 0x0A)
            _overlay.DefaultSource = new Rectangle(66, 0, 22, 22);
        
        if (fidelity >= base.Fidelity)
            base.Fix(fidelity);
        
        _shadowSource.X = _shadowNum * 22 + Variation * 42;
        if (!_completelyOpen)
            _closingsSource.X = (int)_closingsOffset.X + Variation * 36;
    }
}