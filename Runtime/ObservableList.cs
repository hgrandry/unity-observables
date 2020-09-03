using System;
using System.Collections.Generic;
using UnityEngine;

namespace HGrandry.Observables
{
    public interface IObservableList<T> : IObservableCollection<T>
    {
        IListObserver<T> Subscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove);
        void Unsubscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove, bool callRemoveOnCurrentItems = false);
    }

    public class ObservableList<T> : ObservableCollection<T, HashSet<T>, IListObserver<T>>, IObservableList<T>
    {
        public IListObserver<T> Subscribe(Action listChanged)
        {
            var observer = new ListObserver<T>(added => listChanged(), removed => listChanged());
            Subscribe(observer);

            return observer;
        }
        
        public IListObserver<T> Subscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove)
        {
            var observer = new ListObserver<T>(add, remove);
            Subscribe(observer);

            return observer;
        }
        
        public IListObserver<T> Subscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove, ObserverScope scope)
        {
            IListObserver<T> observer = Subscribe(add, remove);
            scope.AddUnsubscribe(() => Unsubscribe(observer));
            return observer;
        }
        
        public IListObserver<T> Subscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove, GameObject scope)
        {
            IListObserver<T> observer = Subscribe(add, remove);
            scope.GetOrCreateComponent<ObserverScope>().AddUnsubscribe(() => Unsubscribe(observer));
            return observer;
        }
    }
}