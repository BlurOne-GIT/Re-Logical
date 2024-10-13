using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MmgEngine;

namespace Logical.MenuPanels;

public class InfoPanel : MenuPanel
{
    private readonly ClickableArea _manualButton;
    private readonly ClickableArea _wikiButton;
    private readonly ClickableArea _repoButton;
    private readonly ClickableArea _reviewsButton;
    private readonly ClickableArea _creditsButton;
    private readonly ClickableArea _backButton;
    
    public InfoPanel(Game game) : base(game)
    {
        Components.Add(_manualButton = new ClickableArea(Game, new Rectangle(108, 87, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_wikiButton = new ClickableArea(Game, new Rectangle(108, 109, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_repoButton = new ClickableArea(Game, new Rectangle(108, 132, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_reviewsButton = new ClickableArea(Game, new Rectangle(108, 155, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_creditsButton = new ClickableArea(Game, new Rectangle(108, 179, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        Components.Add(_backButton = new ClickableArea(Game, new Rectangle(108, 201, 103, 16), outsideBehaviour: ClickableArea.OutsideBehaviour.None));
        
        _manualButton.LeftButtonDown += OpenManual;
        _wikiButton.LeftButtonDown += OpenWiki;
        _repoButton.LeftButtonDown += OpenRepo;
        _reviewsButton.LeftButtonDown += OpenReviews;
        _creditsButton.LeftButtonDown += Credits;
        _backButton.LeftButtonDown += Back;
    }
    
    private static void OpenManual(object sender, EventArgs e) =>
        Process.Start( new ProcessStartInfo
            {
                FileName = "https://amiga.abime.net/manual/0901-1000/906_manual0.pdf?v=1104",
                UseShellExecute = true
            }
        );

    private static void OpenWiki(object sender, EventArgs e) =>
        Process.Start( new ProcessStartInfo
            {
                FileName = "https://github.com/BlurOne-GIT/Re-Logical/wiki",
                UseShellExecute = true
            }
        );
    
    private static void OpenRepo(object sender, EventArgs e) =>
        Process.Start( new ProcessStartInfo
            {
                FileName = 
                    #if DEBUG
                    "https://github.com/BlurOne-GIT/Re-Logical/tree/develop",
                    #else
                    "https://github.com/BlurOne-GIT/Re-Logical",
                    #endif
                UseShellExecute = true
            }
        );

    private static void OpenReviews(object sender, EventArgs e) =>
        Process.Start( new ProcessStartInfo
            {
                FileName = "https://www.amigareviews.leveluphost.com/logical.htm",
                UseShellExecute = true
            }
        );

    private void Credits(object sender, EventArgs e) => SwitchState(new CreditsPanel(Game));

    private void Back(object s, EventArgs e) => SwitchState(new MainPanel(Game));
    
    protected override void Dispose(bool disposing)
    {
        _manualButton.LeftButtonDown -= OpenManual;
        _wikiButton.LeftButtonDown -= OpenWiki;
        _repoButton.LeftButtonDown -= OpenRepo;
        _reviewsButton.LeftButtonDown -= OpenReviews;
        _creditsButton.LeftButtonDown -= Credits;
        _backButton.LeftButtonDown -= Back;

        base.Dispose(disposing);
    }
}