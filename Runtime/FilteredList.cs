using System;
using System.Collections;
using System.Collections.Generic;

namespace HGrandry.Observables
{
    public class FilteredList<T> : IObservableCollection<T>, IListObserver<T>
    {
        private readonly Func<T, bool> _filter;
        private readonly ObservableList<T> _filteredItems = new ObservableList<T>();
        private readonly HashSet<T> _allItems = new HashSet<T>();

        public FilteredList(Func<T, bool> filter)
        {
            _filter = filter;
        }

        void IListObserver<T>.Add(T media)
        {
            _allItems.Add(media);
            
            if(_filter(media))
                _filteredItems.Add(media);
        }

        void IListObserver<T>.Remove(T media)
        {
            _allItems.Add(media);
            _filteredItems.Remove(media);
        }

        public void Refresh()
        {
            _filteredItems.Clear();
            
            foreach (T item in _allItems)
            {
                if (_filter(item))
                {
                    _filteredItems.Add(item);
                }
            }
        }
        
        public event HandleItemAdded<T> Added
        {
            add { _filteredItems.Added += value;  }
            remove { _filteredItems.Added -= value;  }
        }
        
        public event HandleItemRemoved<T> Removed
        {
            add { _filteredItems.Removed += value;  }
            remove { _filteredItems.Removed -= value;  }
        }

        public IListObserver<T> Subscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove)
        {
            return _filteredItems.Subscribe(add, remove);
        }

        public void Subscribe(IListObserver<T> observer)
        {
            _filteredItems.Subscribe(observer);
        }

        public void Unsubscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove, bool callRemoveOnCurrentItems = false)
        {
            _filteredItems.Unsubscribe(add, remove, callRemoveOnCurrentItems);
        }

        public void Unsubscribe(IListObserver<T> observer, bool callRemoveOnCurrentItems = false)
        {
            _filteredItems.Unsubscribe(observer, callRemoveOnCurrentItems);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _filteredItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}