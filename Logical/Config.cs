using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Xna.Framework;

namespace Logical;

public static class Configs
{
    #region Events
    public static event EventHandler ResolutionChanged;
    public static event EventHandler FullscreenChanged;
    public static event EventHandler MusicVolumeChanged;
    public static event EventHandler SfxVolumeChaged;
    #endregion

    #region Fields
    public const int NativeWidth = 320;
    public const int NativeHeight = 256;
    private const string File = "./config.json";
    private static byte _stage;
    private static uint _score;
    private static byte _lives;
    private static readonly Vector2 NativeSize = new(NativeWidth, NativeHeight);
    #endregion

    // Instances
    private static FileStream _fileStream;
    private static JsonNode _jsonNode;

    #region Properties
    public static Vector2 ScreenSize { get; set; }

    public static int MaxScale => Math.Min((int)ScreenSize.X / NativeWidth, (int)ScreenSize.Y / NativeHeight);

    public static Vector2 ScreenOffset
    {
        get
        {
            if (!Fullscreen)
                return Vector2.Zero;
            
            return (ScreenSize - NativeSize * MaxScale) / 2;
        }
    }
    
    public static int Scale 
    { 
        get => !Fullscreen ? _jsonNode[nameof(Scale)]!.GetValue<int>() : MaxScale;
        set
        {
            if (Fullscreen) return;
            _jsonNode[nameof(Scale)] = Math.Clamp(1, MaxScale, value);
            ResolutionChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    
    public static bool Fullscreen
    {
        get => _jsonNode[nameof(Fullscreen)]!.GetValue<bool>();
        set
        {
            _jsonNode[nameof(Fullscreen)] = value;
            FullscreenChanged?.Invoke(null, EventArgs.Empty);
            ResolutionChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    
    public static int MusicVolume
    {
        get => _jsonNode[nameof(MusicVolume)]!.GetValue<int>();
        set
        {
            _jsonNode[nameof(MusicVolume)] = Math.Clamp(0, 10, value);
            MusicVolumeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    
    public static int SfxVolume
    {
        get => _jsonNode[nameof(SfxVolume)]!.GetValue<int>();
        set
        {
            _jsonNode[nameof(SfxVolume)] = Math.Clamp(0, 10, value);
            SfxVolumeChaged?.Invoke(null, EventArgs.Empty);
        }
    }
    
    public static int GraphicSet
    {
        get
        {
            var value = _jsonNode[nameof(GraphicSet)]!.GetValue<int>();
            if (value is 4 && GraphicSet4Remastered)
                return 5;
            return value;
        }
        set => _jsonNode[nameof(GraphicSet)] = value > 4 ? 1 : value < 1 ? 4 : value;
    }

    public static int StereoSeparation
    {
        get => _jsonNode[nameof(StereoSeparation)]!.GetValue<int>();
        set => _jsonNode[nameof(StereoSeparation)] = Math.Clamp(0, 10, value);
    }
    
    public static IFixable.FidelityLevel FidelityLevel
    {
        get => (IFixable.FidelityLevel)_jsonNode[nameof(FidelityLevel)]!.GetValue<int>();
        set => _jsonNode[nameof(FidelityLevel)] =
            (int)(value > IFixable.FidelityLevel.Remastered ? IFixable.FidelityLevel.Faithful :
                value < IFixable.FidelityLevel.Faithful ? IFixable.FidelityLevel.Remastered : value);
    }
    
    public static bool GraphicSet4Remastered
    {
        get => _jsonNode[nameof(GraphicSet4Remastered)]!.GetValue<bool>();
        set => _jsonNode[nameof(GraphicSet4Remastered)] = value;
    }
    
    public static byte Stage
    {
        get => _stage;
        set
        {
            _stage = value;
            SaveGame();
        }
    }
    
    public static uint Score
    {
        get => _score;
        set
        {
            _score = value;
            SaveGame();
        }
    }
    /* A */
    public static byte Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            SaveGame();
        }
    }

    #endregion

    // "Constructor"
    public static void Initialize(int width, int height)
    {
        ScreenSize = new Vector2(width, height);
        try
        {
            _fileStream = System.IO.File.Open(File, FileMode.Open);
            _jsonNode = JsonNode.Parse(_fileStream)!;
        } 
        catch
        {
            _fileStream?.Close();
            _fileStream = System.IO.File.Create(File);
            _fileStream.Write(new[] {(byte)'{', (byte)'}'});
            _fileStream.Flush();
            _fileStream.Close();
            _fileStream = System.IO.File.Open(File, FileMode.Open);
            _jsonNode = JsonNode.Parse(_fileStream)!;
        }
        
        if (_jsonNode[nameof(Fullscreen)] is null)
            Fullscreen = false;
        
        if (_jsonNode[nameof(Scale)] is null)
            Scale = MaxScale / 2;
        _jsonNode[nameof(Scale)] = Math.Clamp(Scale, 1, MaxScale);
        
        if (_jsonNode[nameof(MusicVolume)] is null)
            MusicVolume = 8;
        _jsonNode[nameof(MusicVolume)] = Math.Clamp(MusicVolume, 0, 10);
        
        if (_jsonNode[nameof(SfxVolume)] is null)
            SfxVolume = 8;
        _jsonNode[nameof(SfxVolume)] = Math.Clamp(SfxVolume, 0, 10);
        
        if (_jsonNode[nameof(GraphicSet)] is null)
            GraphicSet = 1;
        _jsonNode[nameof(GraphicSet)] = Math.Clamp(GraphicSet, 1, 5);
        
        if (_jsonNode[nameof(StereoSeparation)] is null)
            StereoSeparation = 3;
        _jsonNode[nameof(StereoSeparation)] = Math.Clamp(StereoSeparation, 0, 10);
        
        if (_jsonNode[nameof(FidelityLevel)] is null)
            FidelityLevel = IFixable.FidelityLevel.Faithful;
        _jsonNode[nameof(FidelityLevel)] = Math.Clamp((int)FidelityLevel, 0, 3);
        
        if (_jsonNode[nameof(GraphicSet4Remastered)] is null)
            GraphicSet4Remastered = false;
        
        LoadGame();
    }

    #region Methods

    public static void SaveFile()
    {
        _fileStream.SetLength(0);
        _fileStream.Seek(0, SeekOrigin.Begin);
        _fileStream.Write(System.Text.Encoding.UTF8.GetBytes(_jsonNode.ToJsonString(new JsonSerializerOptions{ WriteIndented = true })));
        _fileStream.Flush();
    }
    
    public static void CloseFile()
    {
        SaveFile();
        _fileStream.Close();
    }

    private static void SaveGame()
    {
        var availablePositions = new List<int>{0, 1, 2, 3, 4}; // 5 Left
        var seed = Statics.Brandom.Next();
        var random = new Random(seed);
        var buffer = new char[9];
        buffer[1] = (char)(byte)seed;
        seed >>= 8;
        buffer[5] = (char)(byte)seed;
        seed >>= 8;
        buffer[3] = (char)(byte)seed;
        seed >>= 8;
        buffer[7] = (char)(byte)seed;
        
        var masks = new byte[5];
        random.NextBytes(masks); // Call 1
        
        var position = availablePositions[random.Next(0, 5)]; // Call 2
        buffer[position * 2] = (char)(byte)(_stage ^ masks[position]);
        availablePositions.Remove(position); // 4 Left
        
        position = availablePositions[random.Next(0, 4)]; // Call 3
        buffer[position * 2] = (char)(byte)(_score ^ masks[position]);
        availablePositions.Remove(position); // 3 Left

        position = availablePositions[random.Next(0, 3)]; // Call 4
        buffer[position * 2] = (char)(byte)(_score >> 8 ^ masks[position]);
        availablePositions.Remove(position); // 2 Left

        position = availablePositions[random.Next(0, 2)]; // Call 5
        buffer[position * 2] = (char)(byte)(_score >> 16 ^ masks[position]);
        availablePositions.Remove(position); // 1 Left

        position = availablePositions[0];
        buffer[position * 2] = (char)(byte)(_lives ^ masks[position]);
        
        _jsonNode["game"] = new string(buffer);
    }

    private static void LoadGame()
    {
        if (_jsonNode["game"] is null)
            return;

        var buffer = _jsonNode["game"]!.GetValue<string>().ToCharArray();
        var availablePositions = new List<int>{0, 1, 2, 3, 4}; // 5 Left
        var seed = (byte)buffer[1] | (byte)buffer[5] << 8 | (byte)buffer[3] << 16 | (byte)buffer[7] << 24;
        var random = new Random(seed);
        var masks = new byte[5];
        random.NextBytes(masks); // Call 1

        var position = availablePositions[random.Next(0, 5)]; // Call 2
        _stage = (byte)((byte)buffer[position * 2] ^ masks[position]);
        if (_stage > 99)
        {
            _stage = 0;
            return;
        }
        availablePositions.Remove(position); // 4 Left
        
        position = availablePositions[random.Next(0, 4)]; // Call 3
        _score = (uint)((byte)buffer[position * 2] ^ masks[position]);
        availablePositions.Remove(position); // 3 Left

        position = availablePositions[random.Next(0, 3)]; // Call 4
        _score |= (uint)((byte)buffer[position * 2] ^ masks[position]) << 8;
        availablePositions.Remove(position); // 2 Left

        position = availablePositions[random.Next(0, 2)]; // Call 5
        _score |= (uint)((byte)buffer[position * 2] ^ masks[position]) << 16;
        if (_score > 999999)
        {
            _stage = 0;
            _score = 0;
            return;
        }
        availablePositions.Remove(position); // 1 Left

        position = availablePositions[0];
        _lives = (byte)((byte)buffer[position * 2] ^ masks[position]);
        
        if (_lives <= 7) return;
        _stage = 0;
        _score = 0;
        _lives = 0;
    }
    #endregion
}