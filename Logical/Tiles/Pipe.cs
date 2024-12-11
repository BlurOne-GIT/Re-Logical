using Microsoft.Xna.Framework;

namespace Logical.Tiles;

public class Pipe : GameTile
{
    protected int Variation { get; }
    protected Orientations Orientation { get; }
    private Rectangle DefaultRectangleFormula => new(Variation * 36, FileValue * 36, 36, 36);
    public enum Orientations
    {
        Vertical,
        Cross,
        Horizontal
    }
    
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy) : base(game, "Pipes", arrayPosition, xx, yy)
    {
        if ((Orientation = (Orientations)(xx % 3)) is not Orientations.Cross)
            Variation = Statics.Brandom.Next(2);

        DefaultSource = DefaultRectangleFormula;
    }
}