using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Sandclock : Block
{
    #region Field
    private static Sandclock _bruceCook;
    public bool Stuck;
    private Texture2D _usedSand;
    private Texture2D _sandLeft;
    private readonly Vector2 _usPos = new(10f, 18f);
    private readonly Vector2 _slPos = new(10f, 5f);
    //private Vector2 fsPos;
    private TimeSpan _timeSpan = new(0, 0, 0);
    private readonly TimeSpan _clockCycle = new(0, 1, 30);
    #endregion

    public Sandclock(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Sandclock"), arrayPosition, xx, yy)
    {
        if (_bruceCook is not null)
            _bruceCook.Stuck = true;

        _bruceCook = this;

        _usedSand = LevelResources.UsedSand[0];
        _sandLeft = LevelResources.SandLeft[0];
    }

    public override void Update(GameTime gameTime)
    {
        if (Stuck)
            return;

        _timeSpan += gameTime.ElapsedGameTime;
        LevelState.TimeSpanLeft -= gameTime.ElapsedGameTime;
        if (_timeSpan < _clockCycle) return;
        
        _timeSpan = new TimeSpan(0, 0, 0);
        LevelState.TimeLeft--;
        ColorJob.SteveJobs?.Recharge();
    }

    // DEBUG //

    public override void Draw(GameTime gameTime){
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawString(
            Statics.LightFont,
            $"{LevelState.TimeSpanLeft.Minutes}:{LevelState.TimeSpanLeft.Seconds:00}",
            Position - new Vector2(2f, 0f),
            Color,
            0f,
            Vector2.Zero,
            1f,
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

    protected override void Dispose(bool disposing)
    {
        if (_bruceCook.Equals(this))
            _bruceCook = null;
        base.Dispose(disposing);
    }
}