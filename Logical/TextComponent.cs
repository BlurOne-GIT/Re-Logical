using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class TextComponent : Component
{
    public TextComponent(SpriteFont font, Vector2 position, Color color, string defaultText, int layer, Alignment alignment = Alignment.Left, bool enabled = true)
    {
        _font = font;
        _originPosition = position;
        _color = color;
        _text = defaultText;
        zIndex = layer;
        _alignment = alignment;
        RelocatePos();
        IsEnabled = enabled;
    }
    private Vector2 _originPosition;
    private string _text;
    private SpriteFont _font;
    private Color _color;
    private Alignment _alignment;
    public enum Alignment
    {
        Left,
        Center,
        Right
    }
    public string Text { get => _text; set {_text = value; RelocatePos();} }
    private void RelocatePos() => _position = _alignment is Alignment.Left ? _originPosition : _alignment is Alignment.Right ? _originPosition - new Vector2(_font.MeasureString(Text).X/2, 0f) : _originPosition - new Vector2(_font.MeasureString(Text).X/2, 0f);

    public override void Render(SpriteBatch _spriteBatch)
    {
        _spriteBatch.DrawString
        (
            _font,
            Text,
            (_position * Configs.Scale) + new Vector2(Configs.XOffset, Configs.YOffset),
            _color * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            (float)zIndex * 0.1f
        );
    }
    public override void Dispose() {}
}