using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using MmgEngine;

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
    private const string File = "./config.lr";

    /* 0 */ private static int _scale;
    /* 1 */ private static bool _fullscreen;
    /* 2 */ private static int _musicVolume;
    /* 3 */ private static int _sfxVolume;
    /* 4 */ private static int _graphicSet;
    /* 5 */ private static int _stereoSeparation;
    /* 6 */ private static int _stage;
    /*7-9*/ private static int _score;
    /* A */ private static int _lives;
    /* B */ private static bool _autoUpdate;
    private static bool _initialized;
    #endregion

    // Instances
    private static FileStream _fileStream;

    #region Properties
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    public static int MaxScale => Math.Min(Width / NativeWidth, Height / NativeHeight);

    public static int XOffset
    {
        get
        {
            if (!_fullscreen)
                return 0;
            
            return (Width - NativeWidth * MaxScale) / 2;
        }
    }
    public static int YOffset
    {
        get
        {
            if (!_fullscreen)
                return 0;
            
            return (Height - NativeHeight * MaxScale) / 2;
        }
    }

    /* 0 */ public static int Scale 
    { 
        get => !_fullscreen ? _scale : MaxScale;
        set
        {
            SetConfig(value);
            _scale = value;
            EngineStatics.Scale = new Vector2(value);
            ResolutionChanged?.Invoke(null, EventArgs.Empty);
        } 
    }
    /* 1 */
    public static bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            SetConfig(value);
            _fullscreen = value;
            FullscreenChanged?.Invoke(null, EventArgs.Empty);
            ResolutionChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    /* 2 */
    public static int MusicVolume
    {
        get => _musicVolume;
        set
        {
            SetConfig(value);
            _musicVolume = value;
            MusicVolumeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    /* 3 */
    public static int SfxVolume
    {
        get => _sfxVolume;
        set
        {
            SetConfig(value);
            _sfxVolume = value;
            SfxVolumeChaged?.Invoke(null, EventArgs.Empty);
        }
    }
    /* 4 */
    public static int GraphicSet
    {
        get => _graphicSet;
        set
        {
            SetConfig(value);
            _graphicSet = value;
        }
    }
    /* 5 */
    public static int StereoSeparation
    {
        get => _stereoSeparation;
        set
        {
            SetConfig(value);
            _stereoSeparation = value;
        }
    }
    /* 6 */
    public static int Stage
    {
        get => _stage;
        set
        {
            SetConfig(value);
            _stage = value;
        }
    }
    /*7-9*/
    public static int Score
    {
        get => _score;
        set
        {
            SetConfig(value);
            _score = value;
        }
    }
    /* A */
    public static int Lives
    {
        get => _lives;
        set
        {
            SetConfig(value);
            _lives = value;
        }
    }
    /* B */
    public static bool AutoUpdate
    {
        get => _autoUpdate;
        set
        {
            SetConfig(value);
            _autoUpdate = value;
        }
    }

    #endregion

    // Constructor
    public static void Initialize(int width, int height)
    {
        Width = width; Height = height;
        try
        {
            _fileStream = System.IO.File.Open(File, FileMode.Open);
        } catch
        {
            _fileStream = System.IO.File.Create(File);
            _initialized = true;
            /* 0 */ Scale = 2;
            /* 1 */ Fullscreen = false;
            /* 2 */ MusicVolume = 10;
            /* 3 */ SfxVolume = 10;
            /* 4 */ GraphicSet = 0;
            /* 5 */ StereoSeparation = 3;
            /* 6 */ Stage = 0;
            /*7-9*/ Score = 0;
            /* A */ Lives = 0;
            /* B */ AutoUpdate = true;
            return;
        }
        /* 0 */ Scale = (byte)_fileStream.ReadByte();
        /* 1 */ Fullscreen = Convert.ToBoolean(_fileStream.ReadByte());
        /* 2 */ MusicVolume = (byte)_fileStream.ReadByte();
        /* 3 */ SfxVolume = (byte)_fileStream.ReadByte();
        /* 4 */ GraphicSet = (byte)_fileStream.ReadByte();
        /* 5 */ StereoSeparation = (byte)_fileStream.ReadByte();
        /* 6 */ Stage = (byte)_fileStream.ReadByte();
        var b = new byte[4];
        _fileStream.Read(b, 1, 3);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(b);
        /*7-9*/ Score = BitConverter.ToInt32(b, 0);
        /* A */ Lives = _fileStream.ReadByte();
        /* B */ AutoUpdate = Convert.ToBoolean(_fileStream.ReadByte());
        _initialized = true;
        Fixer();
    }

    #region Methods
    private static void SetConfig(object value, [CallerMemberName] string name = null)
    {
        if (!_initialized)
            return;
        
        switch (name)
        {
            case "Scale": _fileStream.Position = 0; break;
            case "Fullscreen": _fileStream.Position = 1; break;
            case "MusicVolume": _fileStream.Position = 2; break;
            case "SfxVolume": _fileStream.Position = 3; break;
            case "GraphicSet": _fileStream.Position = 4; break;
            case "StereoSeparation": _fileStream.Position = 5; break;
            case "Stage": _fileStream.Position = 6; break;
            case "Score":
                _fileStream.Position = 7;
                var b = BitConverter.GetBytes((int)value);
                if (BitConverter.IsLittleEndian) 
                    Array.Reverse(b);
                _fileStream.Write(b, 1, 3);
                return;
            case "Lives": _fileStream.Position = 0x0A; break;
            case "AutoUpdate": _fileStream.Position = 0x0B; break;
        }

        _fileStream.WriteByte(Convert.ToByte(value));
    }

    public static void CloseFile() => _fileStream.Close();

    private static void Fixer()
    {
        // 0
        if (_scale == 0 || NativeWidth * _scale > Width || NativeHeight * _scale > Height)
            Scale = 2;
        // 2
        if (_musicVolume > 10)
            MusicVolume = 10;
        // 3
        if (_sfxVolume > 10)
            SfxVolume = 10;
        // 4
        if (_graphicSet > 4)
            GraphicSet = 0;
        // 5
        if (_stereoSeparation > 10)
            StereoSeparation = 3;
        // 6
        if (_stage > 99)
            Stage = 0;
        // 7-9
        if (_score > 999999)
            Score = 0;
        // A
        if (_lives is <= 0 or > 7)
            Lives = 0;
    }
    #endregion
}