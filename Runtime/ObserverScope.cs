using System;
using System.Collections.Generic;
using UnityEngine;

namespace HGrandry.Observables
{
    public class ObserverScope : MonoBehaviour
    {
        private readonly HashSet<Action> _unsubscribeActions = new HashSet<Action>();

        public void AddUnsubscribe(Action unsubscribeAction)
        {
            _unsubscribeActions.Add(unsubscribeAction);
        }

        public void Unsubscribe()
        {
            foreach (Action unsubscribe in _unsubscribeActions)
            {
                unsubscribe();
            }
            _unsubscribeActions.Clear();
        }
        
        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}