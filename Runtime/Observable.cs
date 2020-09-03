using Newtonsoft.Json;
using UnityEngine;

namespace HGrandry.Observables
{
    public delegate void ObservableValueChange<in TOwner, in T>(TOwner sender, T value);
    public delegate void ObservableValueChange<in T>(T value);

    public interface IUntypedObservable
    {
        object UntypedValue { get; set; }
    }
    
    public interface IObservable<out TOwner, TValue>
    {
        TValue Value { get; set; }
        TValue Get();
        void Set(TValue value);
        event ObservableValueChange<TOwner, TValue> Changed;
    }

    public class Observable<TOwner, T> : IObservable<TOwner, T>, IUntypedObservable
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

        [JsonProperty("internalValue")]
        public T Value
        {
            get => _value;
            set
            {
                if (PreventSettingIdenticalValue && Equals(_value, value))
                    return;

                _value = value;

                ObservableValueChange<TOwner, T> handler = Changed;
                handler?.Invoke(_owner, value);
            }
        }

        public void Subscribe(ObservableValueChange<TOwner, T> onChange, ObserverScope scope)
        {
            Subscribe(onChange);
            scope.AddUnsubscribe(() => Unsubscribe(onChange));
        }
        
        public void Subscribe(ObservableValueChange<TOwner, T> onChange, GameObject scope)
        {
            Subscribe(onChange);
            scope.GetOrCreateComponent<ObserverScope>().AddUnsubscribe(() => Unsubscribe(onChange));
        }
        
        public void Subscribe(ObservableValueChange<TOwner, T> onChange)
        {
            Changed += onChange;
            onChange(_owner, Value);
        }
        
        public void Unsubscribe(ObservableValueChange<TOwner, T> onChange)
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

        object IUntypedObservable.UntypedValue
        {
            get => Value;
            set => Set((T) value);
        }
    }
    
    public interface IObservableReadOnly<out TValue>
    {
        TValue Value { get; }
        TValue Get();
        event ObservableValueChange<TValue> Changed;
        void Subscribe(ObservableValueChange<TValue> onChange);
        void Subscribe(ObservableValueChange<TValue> onChange, ObserverScope scope);
        void Subscribe(ObservableValueChange<TValue> onChange, GameObject scope);
        void Unsubscribe(ObservableValueChange<TValue> onChange);
    }
    
    public interface IObservable<TValue> : IObservableReadOnly<TValue>
    {
        new TValue Value { set; }
        void Set(TValue value);
    }

    public class Observable<T> : IObservable<T>, IUntypedObservable
    {
        [JsonProperty(PropertyName = "Value")]
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

        [JsonProperty("internalValue")]
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
        
        public void Subscribe(ObservableValueChange<T> onChange, ObserverScope scope)
        {
            Subscribe(onChange);
            scope.AddUnsubscribe(() => Unsubscribe(onChange));
        }
        
        public void Subscribe(ObservableValueChange<T> onChange, GameObject scope)
        {
            Subscribe(onChange);
            scope.GetOrCreateComponent<ObserverScope>().AddUnsubscribe(() => Unsubscribe(onChange));
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

        object IUntypedObservable.UntypedValue
        {
            get => Value;
            set => Set((T) value);
        }
    }
}