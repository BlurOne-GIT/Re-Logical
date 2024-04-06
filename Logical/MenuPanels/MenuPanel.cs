using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Logical.MenuPanels;

public abstract class MenuPanel : DrawableGameComponent
{
    private readonly SoundEffect _clickSfx;

    protected MenuPanel(Game game) : base(game)
    {
        _clickSfx = Game.Content.Load<SoundEffect>("Menu Button");
        EnabledChanged += OnEnableChanged;
    }

    protected void PlaySfx(object s, EventArgs e) => _clickSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);

    protected abstract void OnEnableChanged(object s, EventArgs e);
}