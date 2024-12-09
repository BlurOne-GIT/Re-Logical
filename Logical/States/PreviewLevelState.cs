using Microsoft.Xna.Framework;

namespace Logical.States;

public class PreviewLevelState(Game game, Level level) : LevelDrawingState(game, level)
{
    public override void Draw(GameTime gameTime)
    {
        foreach (var component in Components)
            if (component is DrawableGameComponent drawable)
                drawable.Draw(gameTime);
    }
}