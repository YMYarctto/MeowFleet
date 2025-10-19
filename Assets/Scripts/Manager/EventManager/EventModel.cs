using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace EventModel
{
    class EventList
    {
        Dictionary<string, UnityAction> _event;

        public EventList()
        {
            _event = new Dictionary<string, UnityAction>();
        }

        public void AddListener(UnityAction listener,string id="")
        {
            string key = listener.Method.Name+id;
            if (_event.ContainsKey(key))
            {
                Debug.LogWarning($"EventManager: 事件 {key} 已被覆盖");
            }
            _event[key]=listener;
        }

        public void RemoveListener(UnityAction listener,string id="")
        {
            string key = listener.Method.Name+id;
            if (!_event.ContainsKey(key))
            {
                return;
            }
            _event.Remove(key);
        }

        public void InvokeAll()
        {
            if (_event.Count == 0)
            {
                Debug.LogWarning("EventManager: 没有事件被触发");
                return;
            }
            foreach (var v in _event.Values)
            {
                v?.Invoke();
            }
        }

        public void Clear()
        {
            _event.Clear();
        }
    }
}