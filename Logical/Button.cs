using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Button : Component
{
    // Event
    public event EventHandler LeftClicked;
    public event EventHandler RightClicked;

    #region Fields
    private SoundEffect _sound;
    private Point _size;
    private int _layer;
    #endregion

    #region Properties
    private Rectangle _rectangle;
    #endregion

    //Constructor
    public Button(Vector2 position, Point size, Texture2D texture = null, int layer = 0, SoundEffect sfx = null, int soundClickTypes = 0, bool enable = true)
    {
        _position = position;
        _size = size;
        Texture = texture;
        _layer = layer;
        _sound = sfx;
        IsEnabled = enable;
        ResetRectangle(null, null);
        Configs.ResolutionChanged += ResetRectangle;
        Input.ButtonDown += Check;

        if (sfx is not null)
        {
            if (soundClickTypes <= 0)
                LeftClicked += PlaySound;
            if (soundClickTypes >= 0)
                RightClicked += PlaySound;
        }
    }

    #region Methods
    public override void Render(SpriteBatch spriteBatch)
    {
        if (Texture == null)
            return;

        base.Render(spriteBatch);
    }

    private void Check(object s, ButtonEventArgs e)
    {
        if (!IsEnabled)
            return;

        Rectangle mouseRectangle = new Rectangle(e.Position, new Point(1, 1));

        if (mouseRectangle.Intersects(_rectangle))
        {
            if (e.Button == "LeftButton")
                LeftClicked?.Invoke(this, EventArgs.Empty);
            else if (e.Button == "RightButton")
                RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlaySound(object s, EventArgs e) => _sound.Play(MathF.Pow((float)Configs.SfxVolume * 0.1f, 2), 0, 0);

    private void ResetRectangle(object s, EventArgs e) => _rectangle = new Rectangle((int)_position.X * Configs.Scale + Configs.XOffset, (int)_position.Y * Configs.Scale + Configs.YOffset, _size.X * Configs.Scale, _size.Y * Configs.Scale);

    public override void Dispose()
    {
        Configs.ResolutionChanged -= ResetRectangle;
        Input.ButtonDown -= Check;
        LeftClicked -= PlaySound;
        RightClicked -= PlaySound;
    }
    #endregion
}