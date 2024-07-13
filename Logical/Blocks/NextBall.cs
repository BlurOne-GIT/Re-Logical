using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class NextBall : Block, IFixable
{
    #region Field
    private readonly Vector2 _holderOffset = new(9f);
    private readonly Vector2 _shadowOffset = new(12f, 13f);
    private static Texture2D _holder;
    private static Texture2D _indicators;
    private static Texture2D _shadow;
    private Rectangle? _holderSource = new(0, 1, 18, 17);
    private Vector2 _indicatorOffset = new(12f, 11f);
    #endregion

    public NextBall(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "EmptyBlock", arrayPosition, xx, yy)
    {
        DefaultRectangle = new Rectangle(0, 0, 36, 36);
    }

    protected override void LoadContent()
    {
        _holder ??= Game.Content.Load<Texture2D>("Holder");
        _indicators ??= Game.Content.Load<Texture2D>("Indicators");
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/HolderShadowEmpty");
        base.LoadContent();
    }
    
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_shadow, _shadowOffset, 1);
        DrawAnotherTexture(_holder, _holderOffset, 2, _holderSource);
        DrawAnotherTexture(_indicators, _indicatorOffset, 3, new Rectangle(12 * (int)LevelState.NextBall, 0, 12, 12));
    }

    protected override void UnloadContent()
    {
        _holder = _indicators = _shadow = null;
        Game.Content.UnloadAssets(new []{"Holder", "Indicators", $"{Configs.GraphicSet}/HolderShadowEmpty"});
        base.UnloadContent();
    }

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;
    
    public void Fix(IFixable.FidelityLevel fidelity)
    {
        _holderSource = null;
        ++_indicatorOffset.Y;
    }
}