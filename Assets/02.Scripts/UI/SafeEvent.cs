using System;
using System.Collections.Generic;
using UnityEngine;

//해당 이벤트의 중복 구독을 막는다.
public class SafeEvent<T>
{
    private event Action<T> _event;
    private HashSet<Action<T>> _listeners = new();

    public void Subscribe(Action<T> listener)
    {
        if (_listeners.Add(listener))
            _event += listener;
    }

    public void Unsubscribe(Action<T> listener)
    {
        if (_listeners.Remove(listener))
            _event -= listener;
    }

    public void Invoke(T value)
    {
        _event?.Invoke(value);
    }
}
public class SafeEvent
{
    private event Action _event;
    private HashSet<Action> _listeners = new();

    public void Subscribe(Action listener)
    {
        if (_listeners.Add(listener))
            _event += listener;
    }

    public void Unsubscribe(Action listener)
    {
        if (_listeners.Remove(listener))
            _event -= listener;
    }

    public void Invoke()
    {
        _event?.Invoke();
    }
}