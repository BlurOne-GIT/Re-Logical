using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MmgEngine;

namespace Logical.MenuPanels;

public abstract class MenuPanel : GameState
{
    private SoundEffect _clickSfx;
    protected SimpleImage PanelBackground;
    private static readonly Vector2 BackgroundOffset = new(108, 84);

    protected MenuPanel(Game game) : base(game)
    {
        Components.ComponentAdded += OnButtonAdded;
        Components.ComponentRemoved += OnButtonRemoved;
    }

    protected override void LoadContent()
    {
        _clickSfx = Game.Content.Load<SoundEffect>("Sfx/Button");
        Components.Add(
            PanelBackground = new SimpleImage(Game, $"{Configs.GraphicSet}/UI/{GetType().Name}", BackgroundOffset, 1)
            );
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAssets(new []
        {
            "Sfx/Button",
            $"{Configs.GraphicSet}/UI/{GetType().Name}"
        });
        base.UnloadContent();
    }

    protected void PlaySfx(object s, EventArgs e) => _clickSfx.Play(MathF.Pow(Configs.SfxVolume * 0.1f, 2), 0, 0);

    private void OnButtonAdded(object s, GameComponentCollectionEventArgs e)
    {
        if (e.GameComponent is Button button) button.LeftClicked += PlaySfx;
    }
    
    private void OnButtonRemoved(object s, GameComponentCollectionEventArgs e)
    {
        if (e.GameComponent is Button button) button.LeftClicked -= PlaySfx;
    }

    protected override void Dispose(bool disposing)
    {
        Components.ComponentAdded -= OnButtonAdded;
        Components.ComponentRemoved -= OnButtonRemoved;
        base.Dispose(disposing);
    }
}