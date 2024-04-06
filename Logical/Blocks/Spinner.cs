using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Spinner : Block, IReloadable
{
    #region Fields
    private static readonly List<Spinner> ExplodedSpinners = new(0);
    public static event EventHandler AllDone;
    public static event EventHandler ConditionClear;
    private readonly Button _spinButton;
    private bool _exploded;
    private readonly Button[] _slotButtons = new Button[4];
    private bool _closedLeft;
    private bool _closedUp;
    private bool _closedRight;
    private bool _closedDown;
    private readonly Vector2 _cplPos = new(0f, 10f);
    private readonly Vector2 _cpuPos = new(10f, 0f);
    private readonly Vector2 _cprPos = new(32f, 10f);
    private readonly Vector2 _cpdPos = new(10f, 32f);
    private readonly Animation<Vector2> _blPos = new(new Vector2[]{
        new(11f, 6f),
        new(8f),
        new(6f, 11f),
        new(5f, 14f)
        }, false);
    private readonly Animation<Vector2> _buPos = new(new Vector2[]{
        new(22f, 11f),
        new(20f, 8f),
        new(17f, 6f),
        new(14f, 5f)
    }, false);
    private readonly Animation<Vector2> _brPos = new(new Vector2[]{
        new(17f, 22f),
        new(20f),
        new(22f, 17f),
        new(23f, 14f)
    }, false);
    private readonly Animation<Vector2> _bdPos = new(new Vector2[]{
        new(6f, 17f),
        new(8f, 20f),
        new(11f, 22f),
        new(14f, 23f)
    }, false);
    private readonly Animation<Texture2D> _spinAnimation = new(LevelResources.SpinnerSpin, false);
    private readonly Animation<Texture2D> _explodeAnimation = new(LevelResources.SpinnerExplode, false);
    private readonly Vector2 _spinPos = new(5f);
    private readonly Vector2 _explodePos = new(4f);
    private readonly Vector2[] _registers = new Vector2[]
    {
        new(0f, 13f),  // Left
        new(13f, 0f),  // Up
        new(26f, 13f), // Right
        new(13f, 26f)  // Down
    };
    private readonly List<BallColors?> _slotBalls = new(4)
    {
        null,
        null,
        null,
        null
    };
    #endregion

    public Spinner(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.Spinner, arrayPosition, xx, yy)
    {
        _spinButton = new Button(game, new Rectangle(Position.ToPoint(), new Point(36)));
        _spinButton.RightClicked += Spin;
        _registers[1] = Pos.Y == 0 ? new Vector2(13f, -13f) : new Vector2(13f, 0f);
        ExplodedSpinners.Capacity++;
    }

    public void Reload(Block[,] blocks)
    {
        _closedLeft = Pos.X == 0 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X-1, Pos.Y].FileValue);
        _closedUp = Pos.Y != 0 && !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y-1].FileValue);
        _closedRight = Pos.X == 7 || !Statics.HorizontalAttachables.Contains(blocks[Pos.X+1, Pos.Y].FileValue);
        _closedDown = Pos.Y == 4 || !Statics.VerticalAttachables.Contains(blocks[Pos.X, Pos.Y+1].FileValue);
        
        if (!_closedLeft)
        {
            _slotButtons[0] = new Button(Game, new Rectangle((Position + new Vector2(4f, 13f)).ToPoint(), new Point(10, 9)), enabled: false);
            _slotButtons[0].LeftClicked += PopOut;
        }
        if (!_closedUp && Pos.Y != 0 && blocks[Pos.X, Pos.Y-1].FileValue is not 0x16)
        {
            _slotButtons[1] = new Button(Game, new Rectangle((Position + new Vector2(13f, 4f)).ToPoint(), new Point(9, 10)), enabled: false);
            _slotButtons[1].LeftClicked += PopOut;
        }
        if (!_closedRight)
        {
            _slotButtons[2] = new Button(Game, new Rectangle((Position + new Vector2(23f, 13f)).ToPoint(), new Point(10, 9)), enabled: false);
            _slotButtons[2].LeftClicked += PopOut;
        }
        if (!_closedDown && blocks[Pos.X, Pos.Y+1].FileValue is not 0x16)
        {
            _slotButtons[3] = new Button(Game, new Rectangle((Position + new Vector2(13f, 23f)).ToPoint(), new Point(9, 10)), enabled: false);
            _slotButtons[3].LeftClicked += PopOut;
        }
    }

    private void Spin(object s, EventArgs e)
    {
        _spinButton.Enabled = false;
        foreach (var button in _slotButtons)
            if (button is not null)
                button.Enabled = false;
        var f = _slotBalls.First();
        _slotBalls.RemoveAt(0);
        _slotBalls.Add(f);
        _spinAnimation.Start();
        if (_exploded || _slotBalls[0] is not null)
            _blPos.Start();
        if (_exploded || _slotBalls[1] is not null)
            _buPos.Start();
        if (_exploded || _slotBalls[2] is not null)
            _brPos.Start();
        if (_exploded || _slotBalls[3] is not null)
            _bdPos.Start();
        LevelResources.Spin.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        Check();
    }

    public void Pause(bool paused)
    {
        _spinButton.Enabled = !paused;
        if (paused)
        {
            foreach (var button in _slotButtons)
                if (button is not null)
                    button.Enabled = false;
        }
        else
            for (int i = 0; i < 4; i++)
                if (_slotBalls[i] is not null && _slotButtons[i] is not null)
                    _slotButtons[i].Enabled = true;
    }

    private void PopOut(object s, EventArgs e)
    {
        if (LevelState.MovesLeft == 0)
            return;

        int index = 4;
        for (int i = 0; i < 4; i++)
        {
            if (!s.Equals(_slotButtons[i])) continue;
            
            index = i;
            break;
        }

        _slotButtons[index].Enabled = false;
        new Ball(Game, _registers[index] + Position, (Direction)index, (BallColors)_slotBalls[index], true);
        _slotBalls[index] = null;
        LevelResources.PopOut.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
    }

    public override void Update(GameTime gameTime)
    {
        for (int i = 0; i < 4; i++)
            foreach (var ball in Ball.AllBalls.ToArray().Where(ball => ball.Position == _registers[i] + Position && ball.MovementDirection != (Direction)i))
                if (_slotBalls[i] is null && _spinAnimation.IsAtEnd)
                {
                    _slotBalls[i] = ball.BallColor;
                    if (_slotButtons[i] is not null)
                        _slotButtons[i].Enabled = true;
                    LevelResources.PopIn.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                    ball.Dispose();
                    Check();
                }
                else if (Pos.Y != 0 || i != 1)
                    ball.Bounce();

        if (_spinButton.Enabled || !_spinAnimation.IsAtEnd) return;
        
        _spinButton.Enabled = true;
        for (int i = 0; i < 4; i++)
            if (_slotButtons[i] is not null && _slotBalls[i] is not null)
                _slotButtons[i].Enabled = true;
    }

    public async void Check()
    {
        if (LevelState.ColorJobs.Count != 0 && _slotBalls[0] == LevelState.ColorJobs[0] && _slotBalls[1] == LevelState.ColorJobs[1] && _slotBalls[2] == LevelState.ColorJobs[2] && _slotBalls[3] == LevelState.ColorJobs[3])
        {
            await Explode();
            LevelState.ColorJobs.Clear();
            ConditionClear?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (LevelState.ColorJobs.Count != 0 || LevelState.TrafficLights.Count != 0 && _slotBalls[0] != LevelState.TrafficLights[0])
            return;

        if (_slotBalls.Any(a => a != _slotBalls[0]) || _slotBalls[0] is null) return;
        
        await Explode();
        if (LevelState.TrafficLights.Count == 0) return;
        
        LevelState.TrafficLights.RemoveAt(0);
        ConditionClear?.Invoke(this, EventArgs.Empty);
    }

    private async Task Explode(bool fb = false)
    {
        _explodeAnimation.Start();
        _exploded = true;
        for (int i = 0; i < 4; i++)
            _slotBalls[i] = null;
        if (!fb && !ExplodedSpinners.Contains(this))
            ExplodedSpinners.Add(this);
        LevelResources.Explode.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
        while (!_explodeAnimation.IsAtEnd) {
            await Task.Delay(20);
        }
        await Task.Delay(fb ? 20 : 40);
        if (!fb && ExplodedSpinners.Count == ExplodedSpinners.Capacity)
            AllDone?.Invoke(ExplodedSpinners, EventArgs.Empty);
    }

    public async Task<int> FinalBoom()
    {
        _spinButton.Enabled = false;
        foreach (var button in _slotButtons)
            if (button is not null)
                button.Enabled = false;
        var r = _slotBalls.Count(a => a is not null);
        await Explode(true);
        return r;
    }

    public static void ClearList()
    {
        ExplodedSpinners.Clear();
        ExplodedSpinners.Capacity = 0;
    }

    public new void Dispose()
    {
        base.Dispose();
        _spinButton.RightClicked -= Spin;
        _spinButton.Dispose();
        foreach (var button in _slotButtons)
        {
            if (button is not null)
                button.LeftClicked -= PopOut;
            button?.Dispose();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        if (!_spinAnimation.IsAtEnd)
        {
            spriteBatch.Draw(
                _spinAnimation.NextFrame(),
                (Position + _spinPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_slotBalls[0] is not null)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBall[(int)_slotBalls[0]],
                (Position + _blPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (_exploded)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBallExploded,
                (Position + _blPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (_slotBalls[1] is not null)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBall[(int)_slotBalls[1]],
                (Position + _buPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (_exploded)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBallExploded,
                (Position + _buPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (_slotBalls[2] is not null)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBall[(int)_slotBalls[2]],
                (Position + _brPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (_exploded)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBallExploded,
                (Position + _brPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (_slotBalls[3] is not null)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBall[(int)_slotBalls[3]],
                (Position + _bdPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        else if (_exploded)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerBallExploded,
                (Position + _bdPos.NextFrame()) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
        }
        if (_closedLeft)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerClosedLeft,
                (Position + _cplPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedUp)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerClosedUp,
                (Position + _cpuPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedRight)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerClosedRight,
                (Position + _cprPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (_closedDown)
        {
            spriteBatch.Draw(
                LevelResources.SpinnerClosedDown,
                (Position + _cpdPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0f,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (!_explodeAnimation.IsAtEnd)
        {
            spriteBatch.Draw(
                _explodeAnimation.NextFrame(),
                (Position + _explodePos) * Configs.Scale,
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