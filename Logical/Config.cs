using System;
using System.IO;
using System.Runtime.CompilerServices;

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
    private const int nativeWidth = 320;
    private const int nativeHeight = 256;
    private const string file = ".\\config.lr";
    private static int _width;
    private static int _height;

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
    #endregion

    // Instances
    private static FileStream _fileStream;

    #region Properties
    public static int Width { get => _width; }
    public static int Height { get => _height; }
    public static int NativeWidth { get => nativeWidth; }
    public static int NativeHeight { get => nativeHeight; }
    public static int MaxScale
    {
        get
        {
            return Math.Min(_width / nativeWidth, _height / nativeHeight);
        }
    }
    public static int XOffset
    {
        get
        {
            if (!_fullscreen)
                return 0;
            
            return (_width - nativeWidth * MaxScale) / 2;
        }
    }
    public static int YOffset
    {
        get
        {
            if (!_fullscreen)
                return 0;
            
            return (_height - nativeHeight * MaxScale) / 2;
        }
    }

    /* 0 */ public static int Scale 
    { 
        get 
        {
            if (!_fullscreen) return _scale;
            else return MaxScale;
        } 
        set {SetConfig(value); _scale = value; ResolutionChanged?.Invoke(null, new EventArgs());} 
    }
    /* 1 */ public static bool Fullscreen { get => _fullscreen; set {SetConfig(value); _fullscreen = value; FullscreenChanged?.Invoke(null, new EventArgs()); ResolutionChanged?.Invoke(null, new EventArgs());}}
    /* 2 */ public static int MusicVolume { get => _musicVolume; set {SetConfig(value); _musicVolume = value; MusicVolumeChanged?.Invoke(null, new EventArgs());}}
    /* 3 */ public static int SfxVolume { get => _sfxVolume; set {SetConfig(value); _sfxVolume = value; SfxVolumeChaged?.Invoke(null, new EventArgs());}}
    /* 4 */ public static int GraphicSet { get => _graphicSet; set {SetConfig(value); _graphicSet = value; }}
    /* 5 */ public static int StereoSeparation { get => _stereoSeparation; set {SetConfig(value); _stereoSeparation = value; }}
    /* 6 */ public static int Stage { get => _stage; set {SetConfig(value); _stage = value; }}
    /*7-9*/ public static int Score { get => _score; set {SetConfig(value); _score = value; }}
    /* A */ public static int Lives { get => _lives; set {SetConfig(value); _lives = value;}}
    /* B */ public static bool AutoUpdate { get => _autoUpdate; set {SetConfig(value); _autoUpdate = value;}}
    #endregion

    // Constructor
    public static void Initialize(int width, int height)
    {
        _width = width; _height = height;
        try
        {
            _fileStream = File.Open(file, FileMode.Open);
        } catch
        {
            _fileStream = File.Create(file);
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
        /* 0 */ _scale = (byte)_fileStream.ReadByte();
        /* 1 */ _fullscreen = Convert.ToBoolean(_fileStream.ReadByte());
        /* 2 */ _musicVolume = (byte)_fileStream.ReadByte();
        /* 3 */ _sfxVolume = (byte)_fileStream.ReadByte();
        /* 4 */ _graphicSet = (byte)_fileStream.ReadByte();
        /* 5 */ _stereoSeparation = (byte)_fileStream.ReadByte();
        /* 6 */ _stage = (byte)_fileStream.ReadByte();
        var b = new byte[4];
        _fileStream.Read(b, 1, 3);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(b);
        /*7-9*/ _score = BitConverter.ToInt32(b, 0);
        /* A */ _lives = _fileStream.ReadByte();
        /* B */ _autoUpdate = Convert.ToBoolean(_fileStream.ReadByte());
        Fixer();
    }

    #region Methods
    private static void SetConfig(object value, [CallerMemberName] string name = null)
    {
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
                byte[] b = BitConverter.GetBytes((int)value);
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
        if (_scale == 0 || nativeWidth * _scale > _width || nativeHeight * _scale > _height)
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
        if (_lives <= 0 || _lives > 7)
            Lives = 0;
    }
    #endregion
}