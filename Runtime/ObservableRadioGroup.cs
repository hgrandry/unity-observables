using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace HGrandry.Observables
{
  public class ObservableRadioGroup<T>
  {
    private readonly Action<UnityAction> _onTryChange;
    private readonly List<Observable<T, bool>> _tabs;
    private readonly List<ObservableValueChange<T, bool>> _listeners = new List<ObservableValueChange<T, bool>>();
    private readonly int _defaultIndex;
    private int _currentIndex;

    public event Action<int> ValueChanged;

    public int TabCount => _tabs.Count;

    public ObservableRadioGroup(IEnumerable<Observable<T, bool>> tabs = null, int defaultIndex = 0, Action<UnityAction> onTryChange = null)
    {
      _defaultIndex = defaultIndex;
      _onTryChange = onTryChange;
      _tabs = tabs?.ToList() ?? new List<Observable<T, bool>>();
      _currentIndex = defaultIndex;

      if (!_tabs.Any())
        return;

      // set current tab on
      Observable<T, bool> current = _tabs[defaultIndex];
      current.Value = true;

      for (var i = 0; i < _tabs.Count; i++)
      {
        var index = i; // store in local variable to access in closure
        Observable<T, bool> tab = _tabs[i];
        ObservableValueChange<T, bool> listener = (sender, value) => TabValueChanged(index, value);
        _listeners.Add(listener);
        tab.Changed += listener;
      }
    }

    public void Add(Observable<T, bool> tab)
    {
      if (_tabs.Count == _defaultIndex)
      {
        tab.Value = true;
      }

      var index = _tabs.Count;
      _tabs.Add(tab);
      ObservableValueChange<T, bool> listener = (sender, value) => TabValueChanged(index, value);
      _listeners.Add(listener);
      tab.Changed += listener;
    }

    public void Dispose()
    {
      for (var index = 0; index < _tabs.Count; index++)
      {
        Observable<T, bool> tab = _tabs[index];
        ObservableValueChange<T, bool> listener = _listeners[index];
        tab.Changed -= listener;
      }
    }

    private void TabValueChanged(int index, bool value)
    {
      if (!value)
      {
        if (index == _currentIndex)
          _tabs[index].Value = true;
        return;
      }

      if (index == _currentIndex)
        return;

      if (_onTryChange != null)
      {
        // keep toggle off until callback validate the change
        _tabs[index].Value = false;
        _onTryChange(() => SetCurrent(index));
      }
      else
      {
        SetCurrent(index);
      }
    }

    public void Select(int index)
    {
      if (index < 0 || _tabs.Count <= index)
        return;

      _tabs[index].Value = true;
    }

    private void SetCurrent(int index)
    {
      _currentIndex = index;

      Observable<T, bool> tab = _tabs[index];
      tab.Value = true;

      for (var i = 0; i < _tabs.Count; i++)
      {
        if (i == index)
          continue;

        Observable<T, bool> other = _tabs[i];
        other.Value = false;
      }

      Action<int> handler = ValueChanged;
      handler?.Invoke(index);
    }
  }
}