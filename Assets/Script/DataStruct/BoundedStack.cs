using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundedStack<T>
{
    private readonly int _capacity;
    private readonly LinkedList<T> _list;

    public BoundedStack(int capacity)
    {
        _capacity = capacity;
        _list = new LinkedList<T>();
    }

    public void Push(T item)
    {
        if (_list.Count >= _capacity)
        {
            _list.RemoveFirst();
        }

        _list.AddLast(item);
    }

    public T Pop()
    {
        if (_list.Count == 0)
        {
            throw new System.InvalidOperationException("Stack is empty.");
        }

        var lastNode = _list.Last;
        _list.RemoveLast();
        return lastNode.Value;
    }

    public int Count => _list.Count;
}