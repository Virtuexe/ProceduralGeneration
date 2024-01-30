using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameActions
{
    public List<GameAction> actionList = new List<GameAction>();
}
public enum ActionType
{ 
    use,
    primary,
    secondary
}
[System.Serializable]
public class ActionEvent : UnityEvent<bool>
{
}
[System.Serializable]
public class GameAction
{
    public ActionEvent action;
    public ActionType type;
    public int priority;

    public GameAction(ActionEvent _action, ActionType _type,int _priority)
    {
        action = _action;
        type = _type;
        priority = _priority;
    }
}

