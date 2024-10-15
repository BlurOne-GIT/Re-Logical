using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Logical;

public class LevelSet : IDisposable, IAsyncDisposable
{
    private readonly FileStream _stream;
    private readonly string[] _levelNames = new string[99];
    
    public LevelSet(string setPath)
    {
        _stream = File.OpenRead(setPath);
        if (_stream.Length is not 10000L)
            throw new Exception("Not Logical Set File.");

        var buffer = new byte[15];
        for (int i = 0; i < 99; ++i)
        {
            _stream.Position += _stream.Read(buffer, 0, 2) + 81;
            if (buffer[0] is not 0x00)
                throw new Exception("Not Logical Set File.");
            //if (buffer[1] is not 0x00)
            //    Console.WriteLine($"{i+1:00} {buffer[1]:00}");
            _stream.Position += _stream.Read(buffer, 0, 15);
            _levelNames[i] = DecodeName(buffer);
            _stream.Position += _stream.Read(buffer, 0, 2);
            if ((buffer[0] | buffer[1]) is not 0x00)
                throw new Exception("Not Logical Set File.");
        }
    }

    public Level GetLevel(int level)
    {
        var tempBlocks = new IBlock[8,5];
        _stream.Position = (level - 1) * 100 + 2; //int btr = (level - 1) * 100 + 2;
        for (int i = 0; i < 39; i++)
            tempBlocks[i % 8, i / 8] = new FileBlock(
                (byte)_stream.ReadByte(),
                (byte)_stream.ReadByte(),
                new Point(i % 8, i / 8)
            );
        byte oTime;
        tempBlocks[7,4] = new FileBlock((byte)_stream.ReadByte(), (byte)((oTime = (byte)_stream.ReadByte()) % 4), new Point(7, 4));
        return new Level(tempBlocks, (byte)level, oTime, (byte)_stream.ReadByte(), _levelNames[level - 1]);
    }

    public string GetLevelName(int level)
        => _levelNames[level - 1];
    
    
    public static string DecodeName(byte[] encodedName)
        => encodedName.Aggregate("", (current, b) => current + $"{(char)(byte)unchecked(b - 0xC4)}").TrimEnd();

    public static byte[] EncodeName(string name)
        => name.PadRight(15, ' ').Select(c => (byte)unchecked((byte)c + 0xC4)).ToArray();

    public byte GetLevelNumber(string name) => (byte)(Array.IndexOf(_levelNames, name) + 1);

    public bool CheckValidStage(int stage)
    {
        if (stage is < 0 or > 99)
            return false;
        
        _stream.Position = (stage - 1) * 100 + 2;
        for (int i = 0; i < 100; ++i)
            if (_stream.ReadByte() is not 0x00)
                return true;
        
        return false;
    }
    
    public void Dispose()
    {
        _stream?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream != null) await _stream.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

/*
0x00 unknown
0x01 =
0x02 >
0x03 ?
0x04 @
0x05 A
0x06 B
0x07 C
0x08 D
0x09 E
0x0A F
0x0B G
0x0C H
0x0D I
0x0E J
0x0F K
0x10 L
0x11 M
0x12 N
0x13 O
0x14 P
0x15 Q
0x16 R
0x17 S
0x18 T
0x19 U
0x1A V
0x1B W
0x1C X
0x1D Y
0x1E Z
0x1F [
0x20 \
0x21 ]
0x22 ^
0x23 _
0x24 '
0x25 a
0x26 b
0x27 c
0x28 d
0x29 e
0x2A f
0x2B g
0x2C h
0x2D i
0x2E j
0x2F k
0x30 l
0x31 m
0x32 n
0x33 o
0x34 p
0x35 q
0x36 r
0x37 s
0x38 t
0x39 u
0x3A v
0x3B w
0x3C x
0x3D y
0x3E z
0x3F {
0x40 |
0x41 }
0x42 ~
0x43 unknown
0x44 █ unknown
0x45 █ unknown
0x46 █ unknown
0x47 █ unknown
0x48 █ unknown
0x49 █ unknown
0x4A █ unknown
0x4B █ unknown
0x4C █ unknown
0x4D █ unknown
0x4E █ unknown
0x4F █ unknown
0x50 █ unknown
0x51 █ unknown
0x52 █ unknown
0x53 █ unknown
0x54 █ unknown
0x55 █ unknown
0x56 █ unknown
0x57 █ unknown
0x58 █ unknown
0x59 █ unknown
0x5A █ unknown
0x5B █ unknown
0x5C █ unknown
0x5D █ unknown
0x5E █ unknown
0x5F █ unknown
0x60 █ unknown
0x61 █ unknown
0x62 █ unknown
0x63 █ unknown
0x64   unknown
0x65 ¡
0x66 ¢ 
0x67 £
0x68 ¤
0x69 ¥
0x6A ¦
0x6B §
0x6C ¨
0x6D ©
0x6E unknown, maybe ⫑
0x6F «
0x70 ¬
0x71 unknown, - but thicker
0x72 ®
0x73 ¯
0x74 °
0x75 ±
0x76 ²
0x77 ³
0x78 ´
0x79 µ
0x7A ¶ (20) or ¶ (244)
0x7B • or ·
0x7C unknown, , but flatter maybe ¸
0x7D ¹
0x7E unknown, ° over -
0x7F »
0x80 ¼
0x81 ½
0x82 ¾
0x83 ¿
0x84 À
0x85 Á
0x86 Â
0x87 Ã
0x88 Ä
0x89 Å
0x8A Æ
0x8B Ç
0x8C È
0x8D É
0x8E Ê
0x8F Ë
0x90 Ì
0x91 Í
0x92 Î
0x93 Ï
0x94 Ð
0x95 Ñ
0x96 Ò
0x97 Ó
0x98 Ô
0x99 Õ
0x9A Ö
0x9B ×
0x9C Ø
0x9D Ù
0x9E Ú
0x9F Û
0xA0 Ü
0xA1 Ý
0xA2 Þ
0xA3 ß
0xA4 à
0xA5 á
0xA6 â
0xA7 ã
0xA8 ä
0xA9 å
0xAA æ
0xAB ç
0xAC è
0xAD é
0xAE ê
0xAF ë
0xB0 ì
0xB1 í
0xB2 î
0xB3 ï
0xB4 ð
0xB5 ñ
0xB6 ò
0xB7 ó
0xB8 ô
0xB9 õ
0xBA ö
0xBB ÷
0xBC ø
0xBD ù
0xBE ú
0xBF û
0xC0 ü
0xC1 ý
0xC2 þ
0xC3 ÿ
0xC4 █ unknown
0xC5 █ unknown
0xC6 █ unknown
0xC7 █ unknown
0xC8 █ unknown
0xC9 █ unknown
0xCA █ unknown
0xCB █ unknown
0xCC █ unknown
0xCD █ unknown
0xCE █ unknown
0xCF █ unknown
0xD0 █ unknown
0xD1 █ unknown
0xD2 █ unknown
0xD3 █ unknown
0xD4 █ unknown
0xD5 █ unknown
0xD6 █ unknown
0xD7 █ unknown
0xD8 █ unknown
0xD9 █ unknown
0xDA █ unknown
0xDB █ unknown
0xDC █ unknown
0xDD █ unknown
0xDE █ unknown
0xDF █ unknown
0xE0 █ unknown
0xE1 █ unknown
0xE2 █ unknown
0xE3 █ unknown
0xE4   (space)
0xE5 !
0xE6 "
0xE7 #
0xE8 $
0xE9 %
0xEA &
0xEB '
0xEC (
0xED )
0xEE *
0xEF +
0xF0 ,
0xF1 -
0xF2 .
0xF3 /
0xF4 0
0xF5 1
0xF6 2
0xF7 3
0xF8 4
0xF9 5
0xFA 6
0xFB 7
0xFC 8
0xFD 9
0xFE :
0xFF ;
*/