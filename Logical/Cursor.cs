using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical;

public class Cursor : DrawableGameComponent
{
    private static readonly Vector2 Origin = new(7f);
    private readonly Texture2D _texture;
    private Vector2 _position;

    public Cursor(Game game) : base(game)
        => _texture = Game.Content.Load<Texture2D>("Cursor");


    public override void Update(GameTime gameTime)
        => _position = HoverableArea.MouseVector;

    public override void Draw(GameTime gameTime)
        => Game.Services.GetService<SpriteBatch>().Draw(
            _texture,
            _position,
            null,
            Color.White,
            0f,
            Origin,
            1f,
            SpriteEffects.None,
            DrawOrder * 0.1f
        );
}