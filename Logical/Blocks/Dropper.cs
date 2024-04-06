using System;
using Logical.States;
using Microsoft.Xna.Framework;

namespace Logical.Blocks;

public class Dropper : Block
{
    #region Fields
    private readonly Vector2 _inRegister = new(13f, -13f);
    private readonly Vector2 _inSpawn = new(13f, -4f);
    private readonly Vector2 _bumpRegister = new(13f, 0f);
    #endregion

    public Dropper(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.PipeVertical, arrayPosition, xx, yy) { }

    public override void Update(GameTime gameTime)
    {
        if (Pos.Y != 0)
            return;

        foreach (var ball in Ball.AllBalls) // This had .ToArray()
        {
            if (LevelState.MovesLeft > 1 && ball.Position == _inRegister + Position)
            {
                new Ball(Game, _inSpawn + Position, Direction.Down, ball.BallColor, true);
                LevelResources.PopIn.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);
                ball.Dispose();
                continue;
            }

            if (ball.MovementDirection is Direction.Up && ball.Position == _bumpRegister + Position)
                ball.Bounce();
        }
    }
}