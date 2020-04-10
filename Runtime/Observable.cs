using System.Runtime.Serialization;

namespace HGrandry.Observables
{
  public delegate void ObservableValueChange<in TOwner, in T>(TOwner sender, T value);
  public delegate void ObservableValueChange<in T>(T value);

  public interface IObservable<out TOwner, TValue>
  {
    TValue Value { get; set; }
    TValue Get();
    void Set(TValue value);
    event ObservableValueChange<TOwner, TValue> Changed;
  }

  public class Observable<TOwner, T> : IObservable<TOwner, T>
  {
    private readonly TOwner _owner;
    private T _value;

    public event ObservableValueChange<TOwner, T> Changed;

    public bool PreventSettingIdenticalValue = true;

    public Observable(TOwner owner)
    {
      _owner = owner;
    }

    public Observable(TOwner owner, T defaultValue)
        : this(owner)
    {
      _value = defaultValue;
    }

    public T Value
    {
      get { return _value; }
      set
      {
        if (PreventSettingIdenticalValue && Equals(_value, value))
          return;

        _value = value;

        var handler = Changed;
        if (handler != null) handler(_owner, value);
      }
    }

    public void Subscribe(ObservableValueChange<TOwner, T> onChange)
    {
      Changed += onChange;
      onChange(_owner, Value);
    }

    public T Get()
    {
      return _value;
    }

    public void Set(T value)
    {
      Value = value;
    }
  }

  public interface IObservableReadOnly<out TValue>
  {
    TValue Value { get; }
    TValue Get();
    event ObservableValueChange<TValue> Changed;
  }

  public interface IObservable<TValue> : IObservableReadOnly<TValue>
  {
    new TValue Value { set; }
    void Set(TValue value);
  }

  public class Observable<T> : IObservable<T>
  {
    private T _value;

    public event ObservableValueChange<T> Changed;

    public bool PreventSettingIdenticalValue = true;

    public Observable()
    {
    }

    public Observable(T defaultValue)
    {
      _value = defaultValue;
    }

    public T Value
    {
      get => _value;
      set
      {
        if (PreventSettingIdenticalValue && Equals(_value, value))
          return;

        _value = value;

        ObservableValueChange<T> handler = Changed;
        handler?.Invoke(value);
      }
    }

    public void Subscribe(ObservableValueChange<T> onChange)
    {
      Changed += onChange;
      onChange(Value);
    }

    public void Unsubscribe(ObservableValueChange<T> onChange)
    {
      Changed -= onChange;
    }

    public T Get()
    {
      return _value;
    }

    public void Set(T value)
    {
      Value = value;
    }
  }
}