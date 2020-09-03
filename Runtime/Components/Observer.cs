using UnityEngine;

namespace HGrandry.Observables.Components
{
    public abstract class Observer<TValue, TObservable> : MonoBehaviour 
        where TObservable : MonoBehaviour, IObservableComponent<TValue>
    {
        [SerializeField] private TObservable _source;

        private bool _immediate = true;
        private bool _subscribed;

        protected TValue Value => _source.Value.Get();

        public TObservable Source
        {
            get => _source;
            set
            {
                if (_source != null)
                {
                    _source.Value.Unsubscribe(HandleValueChanged);
                }
                _source = value;
                _source.Value.Subscribe(HandleValueChanged);
                _subscribed = true;
            }
        }

        protected virtual void Awake()
        {
            if(_subscribed || _source == null)
                return;
            
            _source.Value.Subscribe(HandleValueChanged);
            _subscribed = true;
        }

        protected virtual void OnDestroy()
        {
            if(!_subscribed || _source == null)
                return;
            
            _source.Value.Unsubscribe(HandleValueChanged);
            _subscribed = false;
        }

        protected void SetValue(TValue value)
        {
            _source.Value.Set(value);
        }

        private void HandleValueChanged(TValue value)
        {
            bool immediate = _immediate;
            _immediate = false;
            OnValueChanged(value, immediate);
        }

        protected abstract void OnValueChanged(TValue value, bool immediate);
    }
}