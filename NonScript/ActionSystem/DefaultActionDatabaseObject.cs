using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Actions
{
    public List<Action> actionList = new List<Action>();
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
public class Action
{
    public ActionEvent action;
    public ActionType type;
    public int priority;

    public Action(ActionEvent _action, ActionType _type,int _priority)
    {
        action = _action;
        type = _type;
        priority = _priority;
    }
}

