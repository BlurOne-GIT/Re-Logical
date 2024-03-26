using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Dropper : Block, IUpdateable
{
    #region Fields
    private readonly Vector2 inRegister = new Vector2(13f, -13f);
    private readonly Vector2 inSpawn = new Vector2(13f, -4f);
    private readonly Vector2 bumpRegister = new Vector2(13f, 0f);
    #endregion

    public Dropper(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        Texture = LevelTextures.PipeVertical;
    }

    public void Update(GameTime gameTime)
    {
        if (Pos.Y != 0)
            return;

        foreach (Ball ball in Ball.AllBalls.ToArray())
        {
            if (LevelState.MovesLeft > 1 && ball.Position == inRegister + _position)
            {
                new Ball(inSpawn + _position, Direction.Down, ball.BallColor, true);
                LevelTextures.PopIn.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);
                ball.Dispose();
                continue;
            }

            if (ball.MovementDirection is Direction.Up && ball.Position == bumpRegister + _position)
                ball.Bounce();
        }
    }
}