using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Hourglass : Block, IFixable
{
    #region Field
    public static Hourglass BruceCook { get; private set; }
    public static event EventHandler TimeOut;
    private Vector2 _sandLeftOffset = new(10f, 7f);
    private Vector2 _sandStreamOffset = new(16f, 18f);
    private Vector2 _sandUsedOffset = new(10f, 20f);
    private Rectangle _sandLeftSource = new(0, 0, 16, 10);
    private Rectangle _sandStreamSource = new(0, 0, 5, 12);
    private Rectangle _sandUsedSource = new(0, 0, 16, 10);
    private static Texture2D _sandLeft;
    private Texture2D _sandStream;
    private static Texture2D _sandUsed;

    // TODO: investigate real hourglass behaviour, using this for visual parity
    public int InitialCycles
    {
        set
        {
            _sandLeftOffset.Y += 10 - value;
            _sandLeftSource.Height = value;
            _sandStreamSource.Height = value + 2;
            _sandUsedOffset.Y += value;
            _sandUsedSource.Height -= value;
            _cyclesLeft = value + 1;
            _initialCycles = value + 1;
            _initialTicks = ClockCycleReference.Ticks * value; // TODO: check if when having a double clock half time, the reference initial ticks is just 
        }
    }

    // TODO: check if this Ticks system is correct or if it's actually cycle-based
    public int TimeLeftPoints
        => (int)((ClockCycleReference.Ticks * (_cyclesLeft + 1) - _currentCycle.Ticks) * 100 / _initialTicks);

    private long _initialTicks;
    private int _initialCycles;
    private int _cyclesLeft;
    private TimeSpan _currentCycle = TimeSpan.Zero;
    private static readonly TimeSpan ClockCycleReference = new(0, 1, 30);
    #endregion

    public Hourglass(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "Hourglass", arrayPosition, xx, yy)
    {
        if (BruceCook is not null)
            BruceCook.Enabled = false;

        BruceCook = this;
        
        if (Configs.GraphicSet is 1)
            DefaultSource = new Rectangle(0, 0, 36, 36);
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        _sandLeft ??= Game.Content.Load<Texture2D>("SandLeft");
        _sandStream = Game.Content.Load<Texture2D>("SandStreamTemplate");
        _sandUsed ??= Game.Content.Load<Texture2D>("SandUsed");
    }

    private static readonly Color LightSand = new(0xFF1199CCU); // packed ABGR for #9C1
    private static readonly Color DarkSand = new(0xFF1155AAU); // packed ABGR for #A51
    
    private static Color RandomSandColor() => Statics.Brandom.NextSingle() < .75f ? LightSand : DarkSand;

    private void RandomizeStream()
    {
        var elementCount = 5 * _sandStreamSource.Height;
        var data = new Color[elementCount];
        _sandStream.GetData(0, _sandStreamSource, data, 0, elementCount);
        for (int i = 0; i < elementCount; ++i)
            if (data[i].A is 0xFF)
                data[i] = RandomSandColor();
        _sandStream.SetData(0, _sandStreamSource, data, 0, elementCount);
    }
    
    public override void Update(GameTime gameTime)
    {
        RandomizeStream();
        _currentCycle += gameTime.ElapsedGameTime;
        if (_currentCycle < ClockCycleReference) return;

        // First updates the textures
        ++_sandLeftOffset.Y;
        --_sandLeftSource.Height;
        --_sandStreamSource.Height;
        --_sandUsedOffset.Y;
        ++_sandUsedSource.Height;
        
        // Then checks for TimeOut
        if (_cyclesLeft-- is 0)
        {
            Enabled = false;
            TimeOut?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Finally, reloads
        _currentCycle = TimeSpan.Zero;
        ColourHandicap.SteveJobs?.Recharge();
    }
    
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        // Sand Left
        DrawAnotherTexture(
            _sandLeft,
            _sandLeftOffset,
            1,
            _sandLeftSource,
            spriteEffectsOverride: SpriteEffects.FlipVertically
        );
        
        // Sand Stream
        DrawAnotherTexture(
            _sandStream,
            _sandStreamOffset,
            1,
            _sandStreamSource,
            spriteEffectsOverride: SpriteEffects.FlipVertically
        );
        
        // Sand Used
        DrawAnotherTexture(
            _sandUsed,
            _sandUsedOffset,
            1,
            _sandUsedSource,
            spriteEffectsOverride: SpriteEffects.FlipVertically
        );
        
        #if DEBUG
        spriteBatch.DrawString(
            Statics.TopazFont,
            $"{_cyclesLeft} *\n{(ClockCycleReference-_currentCycle).Minutes}:{(ClockCycleReference-_currentCycle).Seconds:00}",
            Position - new Vector2(2f, 0f),
            Color.Black,
            0f,
            Vector2.Zero,
            new Vector2(1f, .5f),
            SpriteEffects.None,
            1f
        );
        #endif
    }
    
    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;
    
    public void Fix(IFixable.FidelityLevel fidelity)
    {
        --_sandStreamOffset.Y;
        ++_sandStreamSource.Height;

        if (Configs.GraphicSet is 1 && Fidelity >= IFixable.FidelityLevel.Refined)
            DefaultSource = new Rectangle(36, 0, 36, 36);
    }
    
    protected override void UnloadContent()
    {
        _sandUsed = _sandLeft = null;
        Game.Content.UnloadAssets(new [] { "SandLeft", "SandStreamTemplate", "SandUsed" });
        base.UnloadContent();
    }

    protected override void Dispose(bool disposing)
    {
        BruceCook = null;
        base.Dispose(disposing);
    }
}