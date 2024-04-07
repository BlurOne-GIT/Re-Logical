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
        if (LevelState.ColorJobs.Count == 0 || DisableJobs)
            return;
        
        DrawAnotherTexture(_ballLeft, _bdPos, 1);
        DrawAnotherTexture(_ballUp, _buPos, 1);
        DrawAnotherTexture(_ballRight, _brPos, 1);
        DrawAnotherTexture(_ballDown, _bdPos, 1);
    }

    protected override void Dispose(bool disposing)
    {
        if (SteveJobs.Equals(this))
            SteveJobs = null;
        base.Dispose(disposing);
    }
}