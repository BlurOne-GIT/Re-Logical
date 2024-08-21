using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Logical.Blocks;

public class Dropper : Block
{
    #region Fields
    private readonly Vector2 _inRegister = new(13f, -13f);
    private readonly Vector2 _inSpawn = new(13f, -4f);
    private readonly Vector2 _bumpRegister = new(13f, 0f);
    private static SoundEffect _popInSfx;
    #endregion

    public Dropper(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "Pipes", arrayPosition, xx, yy)
    {
        DefaultSource = new Rectangle(0, 0, 36, 36);
    }

    protected override void LoadContent()
    {
        _popInSfx ??= Game.Content.Load<SoundEffect>("Sfx/PopIn");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (Point.Y != 0)
            return;

        foreach (var ball in Ball.AllBalls.ToArray())
        {
            if (LevelState.MovesLeft > 1 && ball.Position == _inRegister + Position)
            {
                _ = new Ball(Game, _inSpawn + Position, Direction.Down, ball.BallColor, true);
                _popInSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                ball.Dispose();
                continue;
            }

            if (ball.MovementDirection is Direction.Up && ball.Position == _bumpRegister + Position)
                ball.Bounce();
        }
    }

    protected override void UnloadContent()
    {
        _popInSfx = null;
        Game.Content.UnloadAsset("Sfx/PopIn");
        base.UnloadContent();
    }
}