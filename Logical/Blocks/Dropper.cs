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
    private static SoundEffect _popIn;
    #endregion

    public Dropper(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.PipeVertical, arrayPosition, xx, yy) { }

    protected override void LoadContent()
    {
        _popIn ??= Game.Content.Load<SoundEffect>("PopIn");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (Pos.Y != 0)
            return;

        foreach (var ball in Ball.AllBalls) // This had .ToArray()
        {
            if (LevelState.MovesLeft > 1 && ball.Position == _inRegister + Position)
            {
                _ = new Ball(Game, _inSpawn + Position, Direction.Down, ball.BallColor, true);
                _popIn.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                ball.Dispose();
                continue;
            }

            if (ball.MovementDirection is Direction.Up && ball.Position == _bumpRegister + Position)
                ball.Bounce();
        }
    }

    protected override void UnloadContent()
    {
        _popIn = null;
        Game.Content.UnloadAsset("PopIn");
        base.UnloadContent();
    }
}