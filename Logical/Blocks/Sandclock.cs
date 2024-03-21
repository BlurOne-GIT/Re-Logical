using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Sandclock : Block, IUpdateable
{
    #region Field
    private static Sandclock bruceCook;
    public bool stuck;
    private Texture2D _usedSand;
    private Texture2D _sandLeft;
    private readonly Vector2 usPos = new Vector2(10f, 18f);
    private readonly Vector2 slPos = new Vector2(10f, 5f);
    //private Vector2 fsPos;
    private TimeSpan timeSpan = new TimeSpan(0, 0, 0);
    private readonly TimeSpan clockCycle = new TimeSpan(0, 1, 30);
    #endregion

    public Sandclock(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        if (bruceCook is not null)
            bruceCook.stuck = true;

        bruceCook = this;

        _texture = LevelTextures.Sandclock;
        _usedSand = LevelTextures.UsedSand[0];
        _sandLeft = LevelTextures.SandLeft[0];
    }

    public void Update(GameTime gameTime)
    {
        if (stuck)
            return;

        timeSpan += gameTime.ElapsedGameTime;
        LevelState.TimeSpanLeft -= gameTime.ElapsedGameTime;
        if (timeSpan >= clockCycle)
        {
            timeSpan = new TimeSpan(0, 0, 0);
            LevelState.TimeLeft--;
            ColorJob.SteveJobs?.Reload();
        }
    }

    // DEBUG //
    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        _spriteBatch.DrawString(
            Statics.LightFont,
            $"{LevelState.TimeSpanLeft.Minutes}:{LevelState.TimeSpanLeft.Seconds}",
            (_position - new Vector2(2f, 0f)) * Configs.Scale,
            Color.White * Statics.Opacity,
            0f,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
    }

    /* DISABLED
        public override void Render(SpriteBatch _spriteBatch)
        {
            base.Render(_spriteBatch);
            _spriteBatch.Draw(
                _usedSand,
                (_position + usPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
            _spriteBatch.Draw(
                LevelTextures.FallingSand,
                (_position + fsPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.2f
            );
            _spriteBatch.Draw(
                _sandLeft,
                (_position + slPos) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.3f
            );
        }*/

    public override void Dispose()
    {
        if (bruceCook.Equals(this))
            bruceCook = null;
        base.Dispose();
    }
}