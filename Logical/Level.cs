using System.Collections.Immutable;
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
        IsTimed = Blocks.OfType<IBlock>().Any(x => x.FileValue is 0x13); // Hourglass
    }
    
    public Level(IBlock[,] blocks, byte number, byte ballTime, byte time, string name)
        : this(blocks, number, ballTime, time, name, LevelSet.EncodeName(name)) { }
    
    public Level(IBlock[,] blocks, byte number, byte ballTime, byte time, byte[] encodedName)
        : this(blocks, number, ballTime, time, LevelSet.DecodeName(encodedName), encodedName) { }
    
    public IBlock[,] Blocks { get; init; }
    public byte Number { get; init; }
    public byte BallTime { get; init; }
    public byte Time { get; init; }
    public bool IsTimed { get; init; }
    public string Name { get; init; }
    public byte[] EncodedName { get; init; }
}

public interface IBlock
{
    public byte FileValue { get; }
    public byte Argument { get; }
    public Point Point { get; }
    public sealed bool HasArgument => Argument is not 0;
    
    protected static readonly ImmutableHashSet<byte> VerticalAttachables =
    [
        (byte)BlockTypes.Spinner,
        (byte)BlockTypes.VerticalPipe,
        (byte)BlockTypes.CrossPipe,
        (byte)BlockTypes.VerticalColourStopper,
        (byte)BlockTypes.CrossColourStopper,
        (byte)BlockTypes.VerticalTeleporter,
        (byte)BlockTypes.CrossTeleporter,
        (byte)BlockTypes.VerticalColourChanger,
        (byte)BlockTypes.CrossColourChanger,
        (byte)BlockTypes.RightDirectionArrow,
        (byte)BlockTypes.LeftDirectionArrow,
        (byte)BlockTypes.UpDirectionArrow,
        (byte)BlockTypes.DownDirectionArrow,
        (byte)BlockTypes.Dropper
    ];
    
    protected static readonly ImmutableHashSet<byte> HorizontalAttachables =
    [
        (byte)BlockTypes.Spinner,
        (byte)BlockTypes.HorizontalPipe,
        (byte)BlockTypes.CrossPipe,
        (byte)BlockTypes.HorizontalColourStopper,
        (byte)BlockTypes.CrossColourStopper,
        (byte)BlockTypes.HorizontalTeleporter,
        (byte)BlockTypes.CrossTeleporter,
        (byte)BlockTypes.HorizontalColourChanger,
        (byte)BlockTypes.CrossColourChanger,
        (byte)BlockTypes.RightDirectionArrow,
        (byte)BlockTypes.LeftDirectionArrow,
        (byte)BlockTypes.UpDirectionArrow,
        (byte)BlockTypes.DownDirectionArrow
    ];
    
    public enum BlockTypes : byte
    {
        EmptyBlock = 0x00,
        Spinner = 0x01,
        HorizontalPipe = 0x02,
        VerticalPipe = 0x03,
        CrossPipe = 0x04,
        HorizontalColourStopper = 0x05,
        VerticalColourStopper = 0x06,
        CrossColourStopper = 0x07,
        HorizontalTeleporter = 0x08,
        VerticalTeleporter = 0x09,
        CrossTeleporter = 0x0A,
        HorizontalColourChanger = 0x0B,
        VerticalColourChanger = 0x0C,
        CrossColourChanger = 0x0D,
        RightDirectionArrow = 0x0E,
        LeftDirectionArrow = 0x0F,
        UpDirectionArrow = 0x10,
        DownDirectionArrow = 0x11,
        MarbleDisplay = 0x12,
        Hourglass = 0x13,
        ColourHandicap = 0x14,
        TrafficLights = 0x15,
        Dropper = 0x16,
        ColourForecast = 0x17
    }
}

public readonly record struct FileBlock(byte FileValue, byte Argument, Point Point) : IBlock
{
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