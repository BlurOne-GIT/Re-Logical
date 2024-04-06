using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Moves : Block
{
    #region Field
    private readonly Vector2 _bluePos1 = new(7f, 9f);
    private readonly Vector2 _bluePos2 = new(7f, 14f);
    private readonly Vector2 _bluePos3 = new(7f, 19f);
    private readonly Vector2 _bluePos4 = new(7f, 24f);
    #endregion

    public Moves(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.Moves, arrayPosition, xx, yy) { }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        switch (LevelState.MovesLeft)
        {
            case 0:
                DrawAnotherTexture(LevelResources.MovesBlue, _bluePos4, 1);
                goto case 1;
            case 1:
                DrawAnotherTexture(LevelResources.MovesBlue, _bluePos3, 1);
                goto case 2;
            case 2:
                DrawAnotherTexture(LevelResources.MovesBlue, _bluePos2, 1);
                goto case 3;
            case 3:
                DrawAnotherTexture(LevelResources.MovesBlue, _bluePos1, 1);
                break;
        }
    }
}