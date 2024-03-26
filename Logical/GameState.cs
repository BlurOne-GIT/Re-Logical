using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Logical;

public abstract class GameState : IDisposable
{
    public GameState()
    {
        Input.KeyDown += HandleInput;
        Input.ButtonDown += HandleInput;
    }
    public virtual void Dispose()
    {
        Input.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        foreach (Component gameObject in _gameObjects)
            gameObject.Dispose();
    }
    private readonly List<Component> _gameObjects = new();
    public abstract void LoadContent(ContentManager content);
    public abstract void UnloadContent(ContentManager content);
    public abstract void HandleInput(object s, ButtonEventArgs e);
    public abstract void HandleInput(object s, InputKeyEventArgs e);
    public event EventHandler<GameState> OnStateSwitched;
    protected void SwitchState(GameState gameState)
    {
        OnStateSwitched?.Invoke(this, gameState);
    }
    public event EventHandler<GameEvents> OnEventNotification;
    protected void NotifyEvent(GameEvents eventType, object argument = null)
    {
        OnEventNotification?.Invoke(this, eventType);

        foreach (Component gameObject in _gameObjects)
        {
            gameObject.OnNotify(eventType);
        }
    }
    protected void AddGameObject(Component gameObject)
    {
        _gameObjects.Add(gameObject);
    }
    protected void RemoveGameObject(Component gameObject)
    {
        _gameObjects.Remove(gameObject);
    }
    public virtual void Update(GameTime gameTime)
    {
        foreach (Component gameObject in _gameObjects.ToArray())
        {
            if (gameObject is IUpdateable && gameObject.IsEnabled)
                (gameObject as IUpdateable).Update(gameTime);
        }
    }
    public void Render(SpriteBatch spriteBatch)
    {
        foreach (Component gameObject in _gameObjects.OrderBy(a => a.zIndex))
        {
            if (gameObject.IsEnabled)
                gameObject.Render(spriteBatch);
        }
    }
 }