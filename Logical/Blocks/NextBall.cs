using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class NextBall : Block
{
    #region Field
    private readonly Vector2 _holderPos = new(9f);
    private readonly Vector2 _indicatorPos = new(12f);
    private readonly Vector2 _shadowPos = new(12f, 13f);
    #endregion

    public NextBall(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, game.Content.Load<Texture2D>($"{Configs.GraphicSet}/EmptyBlock"), arrayPosition, xx, yy) { }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(LevelResources.HolderShadowEmpty, _shadowPos, 1);
        DrawAnotherTexture(LevelResources.Holder, _holderPos, 2);
        DrawAnotherTexture(LevelResources.Indicator[(int)LevelState.NextBall], _indicatorPos, 3);
    }
}