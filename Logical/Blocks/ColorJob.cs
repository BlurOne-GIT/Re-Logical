using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class ColorJob : Block
{
    #region Field
    public static ColorJob SteveJobs;
    public bool DisableJobs;
    private Texture2D _ballLeft;
    private Texture2D _ballRight;
    private Texture2D _ballUp;
    private Texture2D _ballDown;
    private readonly Vector2 _blPos = new(5f, 14f);
    private readonly Vector2 _brPos = new(23f, 14f);
    private readonly Vector2 _buPos = new(14f, 5f);
    private readonly Vector2 _bdPos = new(14f, 23f);
    #endregion

    public ColorJob(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.ColorJob, arrayPosition, xx, yy)
    {
        if (SteveJobs is not null)
            SteveJobs.DisableJobs = true;
        
        SteveJobs = this;
    }

    public void Recharge()
    {
        for (int i = 0; i < 4; i++)
            LevelState.ColorJobs.Add((BallColors)Statics.Brandom.Next(0, 4));

        _ballLeft = LevelResources.SpinnerBall[(int)LevelState.ColorJobs[0]];
        _ballUp = LevelResources.SpinnerBall[(int)LevelState.ColorJobs[1]];
        _ballRight = LevelResources.SpinnerBall[(int)LevelState.ColorJobs[2]];
        _ballDown = LevelResources.SpinnerBall[(int)LevelState.ColorJobs[3]];
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        if (LevelState.ColorJobs.Count == 0 || DisableJobs)
            return;
        spriteBatch.Draw(
            _ballLeft,
            (Position + _blPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        spriteBatch.Draw(
            _ballRight,
            (Position + _brPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        spriteBatch.Draw(
            _ballDown,
            (Position + _bdPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        spriteBatch.Draw(
            _ballUp,
            (Position + _buPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
    }
    
    public new void Dispose()
    {
        if (SteveJobs.Equals(this))
            SteveJobs = null;
        base.Dispose();
    }
}