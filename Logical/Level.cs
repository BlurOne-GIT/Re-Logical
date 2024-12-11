using System.Collections.Immutable;
using System.Linq;
using Logical.Tiles;
using Microsoft.Xna.Framework;

namespace Logical;

public readonly struct Level
{
    private Level(ITile[,] tiles, byte number, byte ballTime, byte time, string name, byte[] encodedName)
    {
        Tiles = tiles;
        Number = number;
        BallTime = ballTime;
        Time = time;
        Name = name;
        EncodedName = encodedName;
        var list = Tiles.OfType<ITile>().ToImmutableList();
        IsTimed = list.Any(x => x.TileType is ITile.TileTypes.Hourglass);
        AutoWin = !list.Any(x => x.TileType is ITile.TileTypes.Spinner);
    }
    
    public Level(ITile[,] tiles, byte number, byte ballTime, byte time, string name)
        : this(tiles, number, ballTime, time, name, LevelSet.EncodeName(name)) { }
    
    public Level(ITile[,] tiles, byte number, byte ballTime, byte time, byte[] encodedName)
        : this(tiles, number, ballTime, time, LevelSet.DecodeName(encodedName), encodedName) { }
    
    public ITile[,] Tiles { get; init; }
    public byte Number { get; init; }
    public byte BallTime { get; init; }
    public byte Time { get; init; }
    public bool IsTimed { get; init; }
    public bool AutoWin { get; init; }
    public string Name { get; init; }
    public byte[] EncodedName { get; init; }
}

public interface ITile
{
    public byte FileValue { get; }
    public TileTypes TileType => (TileTypes)FileValue;
    public byte Argument { get; }
    public Point Point { get; }
    public sealed bool HasArgument => Argument is not 0;
    
    protected static readonly ImmutableHashSet<byte> VerticalAttachables =
    [
        (byte)TileTypes.Spinner,
        (byte)TileTypes.VerticalPipe,
        (byte)TileTypes.CrossPipe,
        (byte)TileTypes.VerticalColourStopper,
        (byte)TileTypes.CrossColourStopper,
        (byte)TileTypes.VerticalTeleporter,
        (byte)TileTypes.CrossTeleporter,
        (byte)TileTypes.VerticalColourChanger,
        (byte)TileTypes.CrossColourChanger,
        (byte)TileTypes.RightDirectionArrow,
        (byte)TileTypes.LeftDirectionArrow,
        (byte)TileTypes.UpDirectionArrow,
        (byte)TileTypes.DownDirectionArrow,
        (byte)TileTypes.Dropper
    ];
    
    protected static readonly ImmutableHashSet<byte> HorizontalAttachables =
    [
        (byte)TileTypes.Spinner,
        (byte)TileTypes.HorizontalPipe,
        (byte)TileTypes.CrossPipe,
        (byte)TileTypes.HorizontalColourStopper,
        (byte)TileTypes.CrossColourStopper,
        (byte)TileTypes.HorizontalTeleporter,
        (byte)TileTypes.CrossTeleporter,
        (byte)TileTypes.HorizontalColourChanger,
        (byte)TileTypes.CrossColourChanger,
        (byte)TileTypes.RightDirectionArrow,
        (byte)TileTypes.LeftDirectionArrow,
        (byte)TileTypes.UpDirectionArrow,
        (byte)TileTypes.DownDirectionArrow
    ];
    
    public enum TileTypes : byte
    {
        EmptyTile = 0x00,
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

public readonly record struct FileTile(byte FileValue, byte Argument, Point Point) : ITile
{
    public GameTile ToGameTile(Game game) => FileValue switch
    {
        0x00 => new EmptyTile(game, Point, FileValue, Argument),
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
        _ => new EmptyTile(game, Point, FileValue, Argument)
    };
}