using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EventModel;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, EventList> _events;
    static int _id;

    private static EventManager _eventManager;
    public static EventManager instance
    {
        get
        {
            if (!_eventManager)
            {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!_eventManager)
                    return null;
            }
            return _eventManager;
        }
    }
    public void Init()
    {
        _events ??= new();
        _id = 0;
    }

    public static EventGroup GroupBy(string event_id)
    {
        return new(event_id,instance);
    }

    public void AddListener(string url, UnityAction listener)
    {
        AddListener(url, listener, "");
    }
    public void AddListener(string url, UnityAction listener, string id)
    {
        if (!_events.ContainsKey(url))
        {
            EventList eventList = new EventList();
            eventList.AddListener(listener, id);
            _events.Add(url, eventList);
            return;
        }
        _events[url].AddListener(listener, id);
    }

    public void RemoveListener(string url, UnityAction listener)
    {
        RemoveListener(url, listener, "");
    }
    public void RemoveListener(string url, UnityAction listener, string id)
    {
        if (!_events.ContainsKey(url))
        {
            return;
        }
        _events[url].RemoveListener(listener, id);
    }
    public void RemoveListener(string url)
    {
        if (!_events.ContainsKey(url))
        {
            return;
        }
        _events[url].Clear();
        _events.Remove(url);
    }

    public void Invoke(string url)
    {
        if (!_events.ContainsKey(url))
        {
            Debug.LogError($"EventManager: \"{url}\" 不存在");
            return;
        }
        _events[url].InvokeAll();
    }

    public static string GetUniqueID()
    {
        _id++;
        return _id.ToString();
    }
}