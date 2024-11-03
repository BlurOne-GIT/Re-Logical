using System;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.States;

public class GuruState(Game game) : GameState(game)
{
    /*
     * // Main loop
     * // 92 cycles
     * 00FD423C MOVE.L   #$00006809,D3           // $6809 = !26633
     * 00FD4242 JSR      $00FDFF76         // 20 // Returns D0 = /FIR0
     * 00FD4248 MOVE.W   D0,D0             //  4 // to set CCRs
     * 00FD424A BEQ.B    #$04              //  8 // as branch not taken // $00FD4250
     * 00FD424C MOVEQ    #$01,D5           //  4 //
     * 00FD424E CLR.W    D4                //  4 //
     * 00FD4250 JSR      $00FDFF64         // 20 // Returns D0 = DATLY
     * 00FD4256 MOVE.W   D0,D0             //  4 // to set CCRs
     * 00FD4258 BEQ.B    #$02              // 10 // as branch taken // $00FD425C
     * 00FD425A CLR.W    D4
     * 00FD425C SUBQ.L   #1,D3             //  8 // Decrease D3 counter
     * 00FD425E BNE.B    #$E2              // 10 // as branch taken // $00FD4242
     *
     * // Return D0 = /FIR0
     * // 66 cycles
     * 00FDFF76 MOVEA.L  #$00BFE001,A0    // 12 // this address points to CIA-A PRA
     * 00FDFF7C MOVE.B   (A0),D0          //  8 //
     * 00FDFF7E AND.B    #$40,D0          //  8 // keep 6th bit, correspondent to /FIR0 
     * 00FDFF82 BNE.B    #$08             // 10 // as branch taken $00FDFF8C
     * 00FDFF84 MOVE.L   #$00000001,D0
     * 00FDFF8A RTS
     * 00FDFF8C MOVE.L   #$00000000,D0    // 12 //
     * 00FDFF92 RTS                       // 16 //
     *
     * // Return D0 = DATLY
     * // 70 cycles (= 42 cycles + 28 cycles from after branching)
     * 00FDFF64 MOVEA.L  #$00DFF000,A0    // 12 // address is BLTDDAT
     * 00FDFF6A MOVE.W   (A0,$0016),D0    // 12 // offset address is POTINP
     * 00FDFF6E AND.W    #$0400,D0        //  8 // get 10th bit, correspondent to DATLY, I/O data Paula pin 36
     * 00FDFF72 BNE.B    #$18             // 10 // as branch taken // $00FDFF8C
     * 00FDFF74 BRA.B    #$0E                   // $00FDFF84
     * 
     * After the main loop there is code I assume is for rendering, so we will calculate the time for this part only
     * The rest can be attributed to our own rendering times.
     * https://wiki.neogeodev.org/index.php?title=68k_instructions_timings
     *
     * Total cycles per loop = 228 cycles
     * Total cycles for 26633 loops = 6072324 cycles
     * Clock speed = 7.09379MHz = 7093790Hz 
     * Cycle Time = 1 cycle / 7093790Hz = 0.000000141s = 141ns
     * Total time for 26633 loops = 6072324 * 141ns = 856197684ns
     */

    private const long GuruTicks = 8561977L;
    private int _fadeCounter = 3;
    private readonly Color[] _fadeInColors =
    [
        new(0xFF111111U),
        new(0xFFFFFFFFU),
        new(0xFF888888U),
        new(0xFF444444U)
    ];
    private TimeSpan[] _fadeInTimes =
    [
        TimeSpan.FromMilliseconds(1220),
        TimeSpan.FromMilliseconds(960),
        TimeSpan.FromMilliseconds(820),
        TimeSpan.FromMilliseconds(340)
    ];
    private TimeSpan _guruTime = TimeSpan.FromTicks(GuruTicks);
    private SimpleImage _guruMessage;
    private Rectangle? GuruSource => _guruMessage.DefaultSource is null ? new Rectangle(6, 6, 628, 66) : null;
    private float GuruY => _guruMessage.DefaultSource is null ? 0f : 6f * Scale.Y;
    private static Vector2 Scale => (Configs.Scale & 1) is 0 ? new Vector2(.5f) : new Vector2((Configs.Scale >> 1)/(float)Configs.Scale);

    protected override void LoadContent()
    {
        Statics.Backdrop.Visible = true;
        Statics.Backdrop.Opacity = 1f;
        Statics.Cursor.Visible = false;
        Components.Add(_guruMessage =
            new SimpleImage(Game, "GuruMeditation", new Vector2(160, 0), 10, Alignment.TopCenter)
                { Enabled = false, Visible = false, Scale = Scale }
        );
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (_fadeCounter >= 0)
        {
            FadeInUpdate(gameTime);
            return;
        }

        if ((_guruTime -= gameTime.ElapsedGameTime) > TimeSpan.Zero)
            return;
        
        _guruMessage.DefaultSource = GuruSource;
        _guruMessage.Position = _guruMessage.Position with { Y = GuruY };
        _guruTime = TimeSpan.FromTicks(GuruTicks);
    }

    private void FadeInUpdate(GameTime gameTime)
    {
        if ((_fadeInTimes[_fadeCounter] -= gameTime.ElapsedGameTime) <= TimeSpan.Zero)
            Statics.Backdrop.Color = _fadeInColors[_fadeCounter--];

        if (_fadeCounter is not -1) return;
        
        _guruMessage.Visible = true;
        Game.Services.GetService<ClickableWindow>().LeftButtonDown += Reset;
    }

    private void Reset(object sender, EventArgs e)
    {
        Game.Services.GetService<ClickableWindow>().LeftButtonDown -= Reset;
        _guruMessage.Visible = false;
        _fadeInTimes =
        [
            TimeSpan.FromMilliseconds(1221),
            TimeSpan.FromMilliseconds(960),
            TimeSpan.FromMilliseconds(820),
            TimeSpan.FromMilliseconds(340)
        ];
        Components.Add(new TimeDelayedAction(Game, TimeSpan.FromMilliseconds(3340), () =>
        {
            Statics.Backdrop.Color = Color.Black;
            Statics.Backdrop.Opacity = 0f;
            SwitchState(new TitleState(Game));
        }));
        _fadeCounter = 3;
    }
}