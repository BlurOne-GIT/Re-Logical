using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Spinner : Block, IUpdateable, IReloadable
{
    #region Fields
    private static List<Spinner> explodedSpinners = new List<Spinner>(0);
    public static event EventHandler AllDone;
    public static event EventHandler ConditionClear;
    private readonly Button spinButton;
    private bool exploded = false;
    private Button[] slotButtons = new Button[4];
    private bool closedLeft;
    private bool closedUp;
    private bool closedRight;
    private bool closedDown;
    private readonly Vector2 cplPos = new Vector2(0f, 10f);
    private readonly Vector2 cpuPos = new Vector2(10f, 0f);
    private readonly Vector2 cprPos = new Vector2(32f, 10f);
    private readonly Vector2 cpdPos = new Vector2(10f, 32f);
    private readonly Animation<Vector2> blPos = new Animation<Vector2>(new Vector2[4]{
        new Vector2(11f, 6f),
        new Vector2(8f),
        new Vector2(6f, 11f),
        new Vector2(5f, 14f)
        }, false);
    private readonly Animation<Vector2> buPos = new Animation<Vector2>(new Vector2[4]{
        new Vector2(22f, 11f),
        new Vector2(20f, 8f),
        new Vector2(17f, 6f),
        new Vector2(14f, 5f)
    }, false);
    private readonly Animation<Vector2> brPos = new Animation<Vector2>(new Vector2[4]{
        new Vector2(17f, 22f),
        new Vector2(20f),
        new Vector2(22f, 17f),
        new Vector2(23f, 14f)
    }, false);
    private readonly Animation<Vector2> bdPos = new Animation<Vector2>(new Vector2[4]{
        new Vector2(6f, 17f),
        new Vector2(8f, 20f),
        new Vector2(11f, 22f),
        new Vector2(14f, 23f)
    }, false);
    private readonly Animation<Texture2D> spinAnimation = new Animation<Texture2D>(LevelTextures.SpinnerSpin, false);
    private readonly Animation<Texture2D> explodeAnimation = new Animation<Texture2D>(LevelTextures.SpinnerExplode, false);
    private readonly Vector2 spinPos = new Vector2(5f);
    private readonly Vector2 explodePos = new Vector2(4f);
    private readonly Vector2[] registers = new Vector2[4]
        {
            new Vector2(0f, 13f),  // Left
            new Vector2(13f, 0f),  // Up
            new Vector2(26f, 13f), // Right
            new Vector2(13f, 26f)  // Down
        };
    private List<BallColors?> slotBalls = new List<BallColors?>(4)
    {
        null,
        null,
        null,
        null
    };
    #endregion

    public Spinner(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        _texture = LevelTextures.Spinner;
        spinButton = new Button(_position, new Point(36), sfx: LevelTextures.Spin, soundClickTypes: 1);
        spinButton.RightClicked += Spin;
        registers[1] = pos.Y == 0 ? new Vector2(13f, -13f) : new Vector2(13f, 0f);
        explodedSpinners.Capacity++;
    }

    public void Reload(Block[,] blocks)
    {
        closedLeft = pos.X == 0 || !Statics.HorizontalAttachables.Contains(blocks[pos.X-1, pos.Y].FileValue);
        closedUp = pos.Y != 0 && !Statics.VerticalAttachables.Contains(blocks[pos.X, pos.Y-1].FileValue);
        closedRight = pos.X == 7 || !Statics.HorizontalAttachables.Contains(blocks[pos.X+1, pos.Y].FileValue);
        closedDown = pos.Y == 4 || !Statics.VerticalAttachables.Contains(blocks[pos.X, pos.Y+1].FileValue);
        
        if (!closedLeft)
        {
            slotButtons[0] = new Button(_position + new Vector2(4f, 13f), new Point(10, 9), enable: false);
            slotButtons[0].LeftClicked += PopOut;
        }
        if (!closedUp && pos.Y != 0 && blocks[pos.X, pos.Y-1].FileValue is not 0x16)
        {
            slotButtons[1] = new Button(_position + new Vector2(13f, 4f), new Point(9, 10), enable: false);
            slotButtons[1].LeftClicked += PopOut;
        }
        if (!closedRight)
        {
            slotButtons[2] = new Button(_position + new Vector2(23f, 13f), new Point(10, 9), enable: false);
            slotButtons[2].LeftClicked += PopOut;
        }
        if (!closedDown && blocks[pos.X, pos.Y+1].FileValue is not 0x16)
        {
            slotButtons[3] = new Button(_position + new Vector2(13f, 23f), new Point(9, 10), enable: false);
            slotButtons[3].LeftClicked += PopOut;
        }
    }

    private void Spin(object s, EventArgs e)
    {
        spinButton.IsEnabled = false;
        foreach (Button button in slotButtons)
        {
            if (button is not null)
                button.IsEnabled = false;
        }
        var f = slotBalls.First();
        slotBalls.RemoveAt(0);
        slotBalls.Add(f);
        spinAnimation.Start();
        if (exploded || slotBalls[0] is not null)
            blPos.Start();
        if (exploded || slotBalls[1] is not null)
            buPos.Start();
        if (exploded || slotBalls[2] is not null)
            brPos.Start();
        if (exploded || slotBalls[3] is not null)
            bdPos.Start();
        LevelTextures.Spin.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
        Check();
    }

    public void Pause(bool paused)
    {
        spinButton.IsEnabled = !paused;
        if (paused)
        {
            foreach (Button button in slotButtons)
                if (button is not null)
                    button.IsEnabled = false;
        }
        else
            for (int i = 0; i < 4; i++)
                if (slotBalls[i] is not null && slotButtons[i] is not null)
                    slotButtons[i].IsEnabled = true;
    }

    private void PopOut(object s, EventArgs e)
    {
        if (LevelState.MovesLeft == 0)
            return;

        int index = 4;
        for (int i = 0; i < 4; i++)
        {
            if (s.Equals(slotButtons[i]))
            {
                index = i;
                break;
            }
        }

        slotButtons[index].IsEnabled = false;
        new Ball(registers[index] + _position, (Direction)index, (BallColors)slotBalls[index], true);
        slotBalls[index] = null;
        LevelTextures.PopOut.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
    }

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (Ball ball in Ball.allBalls.ToArray())
            {
                if (ball.Position != registers[i] + _position || ball.MovementDirection == (Direction)i)
                    continue;

                if (slotBalls[i] is null && spinAnimation.IsFinished)
                {
                    slotBalls[i] = ball.BallColor;
                    if (slotButtons[i] is not null)
                        slotButtons[i].IsEnabled = true;
                    LevelTextures.PopIn.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
                    ball.Dispose();
                    Check();
                }
                else if (pos.Y != 0 || i != 1)
                    ball.Bounce();
            }
        }

        if(!spinButton.IsEnabled && spinAnimation.IsFinished)
        {
            spinButton.IsEnabled = true;
            for (int i = 0; i < 4; i++)
            {
                if (slotButtons[i] is not null && slotBalls[i] is not null)
                    slotButtons[i].IsEnabled = true;
            }
        }
    }

    public async void Check()
    {
        if (LevelState.ColorJobs.Count != 0 && slotBalls[0] == LevelState.ColorJobs[0] && slotBalls[1] == LevelState.ColorJobs[1] && slotBalls[2] == LevelState.ColorJobs[2] && slotBalls[3] == LevelState.ColorJobs[3])
        {
            await Explode();
            LevelState.ColorJobs.Clear();
            ConditionClear?.Invoke(this, new EventArgs());
            return;
        }

        if (LevelState.ColorJobs.Count != 0 || LevelState.TrafficLights.Count != 0 && slotBalls[0] != LevelState.TrafficLights[0])
            return;

        if (slotBalls.All(a => a == slotBalls[0]) && slotBalls[0] is not null)
        {
            await Explode();
            if (LevelState.TrafficLights.Count != 0)
            {
                LevelState.TrafficLights.RemoveAt(0);
                ConditionClear.Invoke(this, new EventArgs());
            }
        }
    }

    private async Task Explode(bool fb = false)
    {
        explodeAnimation.Start();
        exploded = true;
        for (int i = 0; i < 4; i++)
            slotBalls[i] = null;
        if (!fb && !explodedSpinners.Contains(this))
            explodedSpinners.Add(this);
        LevelTextures.Explode.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
        while (!explodeAnimation.IsFinished) {
            await Task.Delay(20);
        }
        await Task.Delay(fb ? 20 : 40);
        if (!fb && explodedSpinners.Count == explodedSpinners.Capacity)
            AllDone?.Invoke(explodedSpinners, new EventArgs());
    }

    public async Task<int> FinalBoom()
    {
        spinButton.IsEnabled = false;
        foreach (Button button in slotButtons)
            if (button is not null)
                button.IsEnabled = false;
        int r = slotBalls.Count(a => a is not null);
        await Explode(true);
        return r;
    }

    public static void ClearList()
    {
        explodedSpinners.Clear();
        explodedSpinners.Capacity = 0;
    }

    public override void Dispose()
    {
        base.Dispose();
        spinButton.RightClicked -= Spin;
        spinButton.Dispose();
        foreach (Button button in slotButtons)
        {
            if (button is not null)
                button.LeftClicked -= PopOut;
            button?.Dispose();
        }
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        if (!spinAnimation.IsFinished)
        {
            _spriteBatch.Draw(
                spinAnimation.NextFrame(),
                (_position + spinPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (slotBalls[0] is not null)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBall[(int)slotBalls[0]],
                (_position + blPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (exploded)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBallExploded,
                (_position + blPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (slotBalls[1] is not null)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBall[(int)slotBalls[1]],
                (_position + buPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (exploded)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBallExploded,
                (_position + buPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (slotBalls[2] is not null)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBall[(int)slotBalls[2]],
                (_position + brPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (exploded)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBallExploded,
                (_position + brPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (slotBalls[3] is not null)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBall[(int)slotBalls[3]],
                (_position + bdPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (exploded)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerBallExploded,
                (_position + bdPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (closedLeft)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerClosedLeft,
                (_position + cplPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedUp)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerClosedUp,
                (_position + cpuPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedRight)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerClosedRight,
                (_position + cprPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (closedDown)
        {
            _spriteBatch.Draw(
                LevelTextures.SpinnerClosedDown,
                (_position + cpdPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (!explodeAnimation.IsFinished)
        {
            _spriteBatch.Draw(
                explodeAnimation.NextFrame(),
                (_position + explodePos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.3f
            );
        }
    }
}