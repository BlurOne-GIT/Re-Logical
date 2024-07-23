using System;
using Microsoft.Xna.Framework;

namespace Logical.Blocks;

public class Pipe : Block, IFixable
{
    protected int Variation { get; private set; }
    public enum Orientation
    {
        Vertical,
        Cross,
        Horizontal
    }
    
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy) : base(game, "Pipes", arrayPosition, xx, yy)
    {
        if (xx % 3 is not (int)Orientation.Cross)
            Variation = Statics.Brandom.Next(2);
        
        DefaultRectangle = new Rectangle(Variation * 36, xx * 36, 36, 36);
    }

    public virtual IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Remastered;
    
    public virtual void Fix(IFixable.FidelityLevel _)
    {
        Variation = Statics.Brandom.Next(3);
        DefaultRectangle = new Rectangle(Variation * 36, FileValue * 33, 36, 36);
    }
}