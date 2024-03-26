using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Lexer
{
    public Lexer()
    {
        _hexFile = File.ReadAllBytes(Statics.Set);
        if (_hexFile[0] is not 0x00 && _hexFile[1] is not 0x00)
            throw new Exception("Not Logical Set File.");
    }

    #region Fields
    private readonly byte[] _hexFile;
    private List<byte> _stack = new();
    #endregion

    public Block[,] GetLevelBlocks(int level, out int oTime, out int time, out bool isTimed, out string name)
    {
        var tempBlocks = new Block[8,5];
        int btr = (level - 1) * 100 + 2;
        for (int i = 0; i < 39; i++)
        {
            tempBlocks[i % 8, i / 8] = GetBlockType(new Point(i % 8, i / 8), _hexFile[btr], _hexFile[btr+1]);
            btr += 2;
        }
        tempBlocks[7,4] = GetBlockType(new Point(7, 4), _hexFile[btr], (byte)(_hexFile[btr+1] % 4));
        btr++;
        oTime = _hexFile[btr++];
        time = _hexFile[btr++];
        isTimed = tempBlocks.OfType<Block>().Any(b => b is Sandclock);
        name = GetLevelName(level);
        return tempBlocks;
    }

    public string GetLevelName(int level)
    {
        string r = "";
        int btr = (level - 1) * 100 + 83;
        for (int i = 0; i < 15; i++)
            r += $"{(CharEncode)_hexFile[btr + i]}";
        return r.Replace("space", " ").TrimEnd();
    }

    public int GetLevelNumber(string name)
    {
        for (int i = 0; i < 99; i++)
        {
            string r = "";
            int btr = i * 100 + 83;
            for (int j = 0; j < 15; j++)
                r += $"{(CharEncode)_hexFile[btr + j]}";
            if (name == r.Replace("space", " ").TrimEnd())
                return i;
        }
        return 0;
    }

    public Level[] GetAllLevels()
    {
        var levels = new Level[99];
        for (int i = 0; i < 99; i++)
        {
            levels[i] = new Level();
            levels[i].Blocks = GetLevelBlocks(i+1, out levels[i].BallTime, out levels[i].Time, out levels[i].IsTimed, out levels[i].Name);
        }
        return levels;
    }

    private Block GetBlockType(Point index, byte fileValue, byte arguments) => fileValue switch
    {
        0x00 => new EmptyBlock(index, fileValue, arguments),
        0x01 => new Spinner(index, fileValue, arguments),
        <= 0x04 => new Pipe(index, fileValue, arguments),
        <= 0x07 => new Filter(index, fileValue, arguments),
        <= 0x0A => new Tp(index, fileValue, arguments),
        <= 0x0D => new Changer(index, fileValue, arguments),
        <= 0x11 => new Bumper(index, fileValue, arguments),
        0x12 => new Moves(index, fileValue, arguments),
        0x13 => new Sandclock(index, fileValue, arguments),
        0x14 => new ColorJob(index, fileValue, arguments),
        0x15 => new TrafficLight(index, fileValue, arguments),
        0x16 => new Dropper(index, fileValue, arguments),
        0x17 => new NextBall(index, fileValue, arguments),
        _ => new EmptyBlock(index, fileValue, arguments)
    };
}

public enum CharEncode
{
    A = 0x05,
    B = 0x06,
    C = 0x07,
    D = 0x08,
    E = 0x09,
    F = 0x0A,
    G = 0x0B,
    H = 0x0C,
    I = 0x0D,
    J = 0x0E,
    K = 0x0F,
    L = 0x10,
    M = 0x11,
    N = 0x12,
    O = 0x13,
    P = 0x14,
    Q = 0x15,
    R = 0x16,
    S = 0x17,
    T = 0x18,
    U = 0x19,
    V = 0x1A,
    W = 0x1B,
    X = 0x1C,
    Y = 0x1D,
    Z = 0x1E,
    space = 0xE4
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