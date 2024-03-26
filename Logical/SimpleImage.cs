using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class SimpleImage : Component
{
    public SimpleImage(Texture2D texture, Vector2 position, int layer, bool enable = true)
    {
        Texture = texture;
        _position = position;
        zIndex = layer;
        IsEnabled = enable;
    }

    public override void Dispose() {}

    public void ChangeTexture(Texture2D texture) => Texture = texture;
    public void ChangePosition(Vector2 position) => _position = position;
}