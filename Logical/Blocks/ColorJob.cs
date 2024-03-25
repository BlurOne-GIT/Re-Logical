using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class ColorJob : Block
{
    #region Field
    public static ColorJob SteveJobs;
    public bool DisableJobs = false;
    private Texture2D _ballLeft;
    private Texture2D _ballRight;
    private Texture2D _ballUp;
    private Texture2D _ballDown;
    private readonly Vector2 blPos = new Vector2(5f, 14f);
    private readonly Vector2 brPos = new Vector2(23f, 14f);
    private readonly Vector2 buPos = new Vector2(14f, 5f);
    private readonly Vector2 bdPos = new Vector2(14f, 23f);
    #endregion

    public ColorJob(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        _texture = LevelTextures.ColorJob;
        if (SteveJobs is not null)
            SteveJobs.DisableJobs = true;
        
        SteveJobs = this;
    }

    public void Reload()
    {
        for (int i = 0; i < 4; i++)
            LevelState.ColorJobs.Add((BallColors)Statics.Brandom.Next(0, 4));

        _ballLeft = LevelTextures.SpinnerBall[(int)LevelState.ColorJobs[0]];
        _ballUp = LevelTextures.SpinnerBall[(int)LevelState.ColorJobs[1]];
        _ballRight = LevelTextures.SpinnerBall[(int)LevelState.ColorJobs[2]];
        _ballDown = LevelTextures.SpinnerBall[(int)LevelState.ColorJobs[3]];
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        if (LevelState.ColorJobs.Count != 0 && !DisableJobs)
        {
            _spriteBatch.Draw(
                _ballLeft,
                (_position + blPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
            _spriteBatch.Draw(
                _ballRight,
                (_position + brPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
            _spriteBatch.Draw(
                _ballDown,
                (_position + bdPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
            _spriteBatch.Draw(
                _ballUp,
                (_position + buPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
    }

    public override void Dispose()
    {
        if (SteveJobs.Equals(this))
            SteveJobs = null;
        base.Dispose();
    }
}