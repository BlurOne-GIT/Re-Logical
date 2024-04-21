using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class NextBall : Block
{
    #region Field
    private readonly Vector2 _holderOffset = new(9f);
    private readonly Vector2 _indicatorOffset = new(12f);
    private readonly Vector2 _shadowOffset = new(12f, 13f);
    private static Texture2D _holder;
    private static Texture2D _indicators;
    #endregion

    public NextBall(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, game.Content.Load<Texture2D>($"{Configs.GraphicSet}/EmptyBlock"), arrayPosition, xx, yy) { }

    protected override void LoadContent()
    {
        _holder ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/Holder");
        _indicators ??= Game.Content.Load<Texture2D>("Indicators");
        base.LoadContent();
    }
    
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(LevelResources.HolderShadowEmpty, _shadowOffset, 1);
        DrawAnotherTexture(_holder, _holderOffset, 2);
        DrawAnotherTexture(_indicators, _indicatorOffset, 3, new Rectangle(12 * (int)LevelState.NextBall, 0, 12, 12));
    }

    protected override void UnloadContent()
    {
        _indicators = null;
        _holder = null;
        Game.Content.UnloadAssets(new []{"Indicators", "Holder"});
        base.UnloadContent();
    }
}