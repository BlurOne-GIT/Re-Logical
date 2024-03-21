using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public abstract class Component : IDisposable
{
    public bool IsEnabled { get; set; }
    protected Texture2D _texture;
    protected Vector2 _position;
    public int zIndex;
    public virtual void OnNotify(GameEvents eventType) { }
    public virtual void Render(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw
        (
            _texture,
            (_position * Configs.Scale) + new Vector2(Configs.XOffset, Configs.YOffset),
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0
        );
    }
    public abstract void Dispose();
}

public interface IUpdateable
{
    public abstract void Update(GameTime gameTime);
}