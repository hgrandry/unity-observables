using System;
using System.Collections.Generic;
using UnityEngine;

namespace HGrandry.Observables
{
    public class FilteredListView<TModel, TView> : IListObserver<TModel> where TView : MonoBehaviour
    {
        private readonly Func<TModel, Observable<TModel, bool>> _filter;
        private readonly Dictionary<TModel, TView> _items = new Dictionary<TModel, TView>();
        private IListObserver<TModel> _observer;
        private readonly Func<TModel, TView> _createView;
        private readonly Action<TModel, TView> _disposeView;

        public IEnumerable<TModel> Models => _items.Keys;
        public IEnumerable<TView> Views => _items.Values;

        public FilteredListView(
            Func<TModel, Observable<TModel, bool>> filter,
            Func<TModel, TView> createView,
            Action<TModel, TView> disposeView = null)
        {
            _filter = filter;
            _createView = createView;
            _disposeView = disposeView;
        }

        void IListObserver<TModel>.Add(TModel media)
        {
            if(media == null)
                return;
            
            Observable<TModel, bool> property = _filter(media);

            if (property.Value)
                AddMedia(media);

            property.Changed += OnMediaPropertyChanged;
        }

        void IListObserver<TModel>.Remove(TModel media)
        {
            if(media == null)
                return;
            
            Observable<TModel, bool> property = _filter(media);
            property.Changed -= OnMediaPropertyChanged;
        }

        private void OnMediaPropertyChanged(TModel media, bool value)
        {
            if (value)
            {
                AddMedia(media);
            }
            else
            {
                RemoveMedia(media);
            }
        }

        private void AddMedia(TModel media)
        {
            if (_items.ContainsKey(media))
                return;

            TView view = _createView(media);
            _items.Add(media, view);
        }

        private void RemoveMedia(TModel media)
        {
            if (!_items.ContainsKey(media)) 
                return;
            
            TView view = _items[media];
            UnityEngine.Object.Destroy(view.gameObject);
            _disposeView?.Invoke(media, view);

            _items.Remove(media);
        }
    }
}