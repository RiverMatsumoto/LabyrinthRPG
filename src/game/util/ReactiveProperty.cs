using System;
using System.Collections.Generic;
//
// ReactiveProperty<T> - Simple, efficient reactive property
public class ReactiveProperty<T>
{
    private T _value;
    public event Action<T> OnChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnChanged?.Invoke(value);
            }
        }
    }

    public ReactiveProperty(T initialValue)
    {
        _value = initialValue;
    }
}
