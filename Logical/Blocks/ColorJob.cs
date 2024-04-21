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
    private static Texture2D _balls;
    private static readonly Vector2[] BallOffsets =
    {
        new(5f, 14f),
        new(14f, 5f),
        new(23f, 14f),
        new(14f, 23f)
    };

    private readonly Rectangle[] _rectangles = new Rectangle[4];
    #endregion

    public ColorJob(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, game.Content.Load<Texture2D>(@$"{Configs.GraphicSet}\ColorJob"), arrayPosition, xx, yy)
    {
        if (SteveJobs is not null)
            SteveJobs.DisableJobs = true;
        
        SteveJobs = this;
    }

    protected override void LoadContent()
    {
        _balls ??= Game.Content.Load<Texture2D>("SpinnerBalls");
        base.LoadContent();
    }

    public void Recharge()
    {
        for (int i = 0; i < 4; i++)
        {
            var random = Statics.Brandom.Next(0, 4);
            LevelState.ColorJobLayout.Add((BallColors)random);
            _rectangles[i] = new Rectangle(8 * random, 0, 8, 8);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        if (LevelState.ColorJobLayout.Count == 0 || DisableJobs)
            return;

        for (int i = 0; i < 4; i++)
            DrawAnotherTexture(_balls, BallOffsets[i], 1, _rectangles[i]);
    }

    protected override void Dispose(bool disposing)
    {
        if (SteveJobs.Equals(this))
            SteveJobs = null;
        base.Dispose(disposing);
    }

    protected override void UnloadContent()
    {
        _balls = null;
        Game.Content.UnloadAsset("SpinnerBalls");
        base.UnloadContent();
    }
}