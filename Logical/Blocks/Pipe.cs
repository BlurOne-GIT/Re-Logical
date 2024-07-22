using System;
using Microsoft.Xna.Framework;

namespace Logical.Blocks;

public class Pipe : Block, IFixable
{
    protected int RandomAspect { get; private set; }
    
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy) : base
    (
        game,
        (xx % 3) switch
        {
            2 => "PipeHorizontal",
            0 => "PipeVertical",
            1 => "PipeCross",
            _ => throw new ArgumentException("Invalid Pipe direction")
        },
        arrayPosition, xx, yy
    )
    {
        RandomAspect = Statics.Brandom.Next(2);
        DefaultRectangle = new Rectangle(RandomAspect * 36, 0, 36, 36);
    }

    public virtual IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Remastered;
    
    public virtual void Fix(IFixable.FidelityLevel _)
    {
        RandomAspect = Statics.Brandom.Next(3);
        DefaultRectangle = new Rectangle(RandomAspect * 36, 0, 36, 36);
    }
}