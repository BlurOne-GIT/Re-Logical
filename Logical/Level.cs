using System.Linq;
using Logical.Blocks;
using Microsoft.Xna.Framework;

namespace Logical;

public readonly struct Level
{
    private Level(IBlock[,] blocks, byte number, byte ballTime, byte time, string name, byte[] encodedName)
    {
        Blocks = blocks;
        Number = number;
        BallTime = ballTime;
        Time = time;
        Name = name;
        EncodedName = encodedName;
    }
    
    public Level(IBlock[,] blocks, byte number, byte ballTime, byte time, string name)
        : this(blocks, number, ballTime, time, name, LevelSet.EncodeName(name)) { }
    
    public Level(IBlock[,] blocks, byte number, byte ballTime, byte time, byte[] encodedName)
        : this(blocks, number, ballTime, time, LevelSet.DecodeName(encodedName), encodedName) { }
    
    public IBlock[,] Blocks { get; init; }
    public byte Number { get; init; }
    public byte BallTime { get; init; }
    public byte Time { get; init; }
    public bool IsTimed => Blocks.OfType<IBlock>().Any(x => x.FileValue is 0x13); // Hourglass
    public string Name { get; init; }
    public byte[] EncodedName { get; init; }
}

public interface IBlock
{
    public byte FileValue { get; }
    public byte Argument { get; }
    public Point Point { get; }
    public sealed bool HasArgument => Argument is not 0;
    protected static readonly byte[] VerticalAttachables =
    [
        0x01,
        0x03,
        0x04,
        0x06,
        0x07,
        0x09,
        0x0A,
        0x0C,
        0x0D,
        0x0E,
        0x0F,
        0x10,
        0x11,
        0x16
    ];
    protected static readonly byte[] HorizontalAttachables =
    [
        0x01,
        0x02,
        0x04,
        0x05,
        0x07,
        0x08,
        0x0A,
        0x0B,
        0x0D,
        0x0E,
        0x0F,
        0x10,
        0x11
    ];
}

public readonly struct FileBlock(byte xx, byte yy, Point point) : IBlock
{
    public byte FileValue { get; init; } = xx;
    public byte Argument { get; init; } = yy;
    public Point Point { get; init; } = point;

    public Block ToGameBlock(Game game) => FileValue switch
    {
        0x00 => new EmptyBlock(game, Point, FileValue, Argument),
        0x01 => new Spinner(game, Point, FileValue, Argument) { Enabled = false },
        <= 0x04 => new Pipe(game, Point, FileValue, Argument),
        <= 0x07 => new ColourStopper(game, Point, FileValue, Argument),
        <= 0x0A => new Teleporter(game, Point, FileValue, Argument),
        <= 0x0D => new ColourChanger(game, Point, FileValue, Argument),
        <= 0x11 => new DirectionArrow(game, Point, FileValue, Argument),
        0x12 => new MarbleDisplay(game, Point, FileValue, Argument),
        0x13 => new Hourglass(game, Point, FileValue, Argument),
        0x14 => new ColourHandicap(game, Point, FileValue, Argument),
        0x15 => new TrafficLights(game, Point, FileValue, Argument),
        0x16 => new Dropper(game, Point, FileValue, Argument),
        0x17 => new ColourForecast(game, Point, FileValue, Argument),
        _ => new EmptyBlock(game, Point, FileValue, Argument)
    };
}