using System;
using Microsoft.Xna.Framework;

namespace Logical.Blocks;

public class Pipe : Block, IFixable
{
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
        DefaultRectangle = new Rectangle(Statics.Brandom.Next(2) * 36, 0, 36, 36);
    }

    public virtual IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Remastered;

    public virtual bool ShallFix(IFixable.FidelityLevel fidelity)
    {
        if (fidelity < Fidelity)
            return false;
        
        var xx = FileValue % 3;
        return xx is 2 && Configs.GraphicSet is 1 || xx is 0 && Configs.GraphicSet is 1 or 3;
    }
    
    public virtual void Fix(IFixable.FidelityLevel _)
    {
        var rectangle = DefaultRectangle!.Value;
        rectangle.X += 36;
        DefaultRectangle = rectangle;
    }
}