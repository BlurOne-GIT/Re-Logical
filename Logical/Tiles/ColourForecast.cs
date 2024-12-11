using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Tiles;

public class ColourForecast : GameTile, IFixable
{
    private static readonly Vector2 HolderOffset = new(9f);
    private static readonly Vector2 ShadowOffset = new(12f, 13f);
    private static readonly Rectangle ShadowSource = new(0, 54, 18, 18);
    private static Texture2D _holder;
    private static Texture2D _indicators;
    private static Texture2D _shadow;
    private Rectangle? _holderSource = new(0, 1, 18, 17);
    private Vector2 _indicatorOffset = new(12f, 11f);
    private bool _firstDraw = true; //TODO: remove this and reimplement it properly

    public ColourForecast(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "EmptyTile", arrayPosition, xx, yy) =>
        DefaultSource = new Rectangle(0, 0, 36, 36);

    protected override void LoadContent()
    {
        _holder ??= Game.Content.Load<Texture2D>("Holder");
        _indicators ??= Game.Content.Load<Texture2D>("Indicators");
        _shadow ??= Game.Content.Load<Texture2D>($"{Configs.GraphicSet}/HolderShadows");
        base.LoadContent();
    }
    
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        DrawAnotherTexture(_shadow, ShadowOffset, 1, ShadowSource);
        DrawAnotherTexture(_holder, HolderOffset, 2, _holderSource);
        DrawAnotherTexture(_indicators, _indicatorOffset, 3, new Rectangle(12 * (int)LevelState.NextBall, 0, 12, 12));
        
        if (!_firstDraw) return;
        _firstDraw = false;
        DrawAnotherTexture(_holder, HolderOffset, 2, _holderSource);
    }

    protected override void UnloadContent()
    {
        _holder = _indicators = _shadow = null;
        Game.Content.UnloadAssets(["Holder", "Indicators", $"{Configs.GraphicSet}/HolderShadows"]);
        base.UnloadContent();
    }

    public IFixable.FidelityLevel Fidelity => IFixable.FidelityLevel.Intended;

    public void Fix(IFixable.FidelityLevel fidelity)
    {
        _holderSource = null;
        ++_indicatorOffset.Y;
    }
}