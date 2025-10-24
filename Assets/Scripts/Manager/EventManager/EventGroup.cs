using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventGroup
{
    EventManager instance;
    string event_id;

    public EventGroup(string id,EventManager instance)
    {
        this.instance = instance;
        event_id = id;
    }

    public void AddListener(string url, UnityAction listener)
    {
        instance.AddListener(url, listener, event_id);
    }

    public void RemoveListener(string url, UnityAction listener)
    {
        instance.RemoveListener(url, listener, event_id);
    }
}
