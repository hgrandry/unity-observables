using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HGrandry.Observables
{
  public delegate void HandleItemAdded<in T>(T item);
  public delegate void HandleItemRemoved<in T>(T item);

  public interface IObservableCollection<out T> : IObservableCollection<T, IListObserver<T>> { }

  public interface IObservableCollection<out T, in TObserver> : IEnumerable<T>
      where TObserver : IListObserver<T>
  {
    event HandleItemAdded<T> Added;
    event HandleItemRemoved<T> Removed;

    void Subscribe(TObserver observer);
    void Unsubscribe(TObserver observer, bool callRemoveOnCurrentItems = false);
  }

  public abstract class ObservableCollection<T, TInnerCollection, TCollectionObserver> : IObservableCollection<T, TCollectionObserver>
      where TInnerCollection : class, ICollection<T>, new()
      where TCollectionObserver : IListObserver<T>
  {
    protected readonly TInnerCollection Items = new TInnerCollection();

    public event HandleItemAdded<T> Added;
    public event HandleItemRemoved<T> Removed;

    public int Count => Items.Count;

    public bool IsReadOnly => false;

    protected ObservableCollection(IEnumerable<T> items)
    {
      if (items == null)
        return;

      foreach (T item in items)
      {
        Items.Add(item);
      }
    }

    protected ObservableCollection(params T[] items)
    {
      foreach (T item in items)
      {
        Items.Add(item);
      }
    }

    public virtual void Subscribe(TCollectionObserver observer)
    {
      if (observer == null)
        return;

      foreach (T item in Items)
      {
        observer.Add(item);
      }

      Added += observer.Add;
      Removed += observer.Remove;
    }

    public virtual void Unsubscribe(TCollectionObserver observer, bool callRemoveOnCurrentItems = false)
    {
      if (observer == null)
        return;

      Added -= observer.Add;
      Removed -= observer.Remove;

      if (!callRemoveOnCurrentItems)
        return;

      foreach (T item in Items)
      {
        observer.Remove(item);
      }
    }

    public virtual void Unsubscribe(HandleItemAdded<T> add, HandleItemRemoved<T> remove, bool callRemoveOnCurrentItems = false)
    {
      Added -= add;
      Removed -= remove;

      if (!callRemoveOnCurrentItems)
        return;

      foreach (T item in Items)
      {
        remove(item);
      }
    }

    /// <summary>
    /// Add an item to the list.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
      Items.Add(item);
      OnAdded(item);
    }

    /// <summary>
    /// Add a collection of item to the list.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<T> items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
    }

    public void CopyTo(T[] array)
    {
      Items.CopyTo(array, 0);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      Items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Remove an item from the list.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
      if (Items.Remove(item))
      {
        OnElementRemoved(item);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Clear the list.
    /// </summary>
    public void Clear()
    {
      while (Items.Any())
      {
        Remove(Items.Last());
      }
    }

    public bool Contains(T item)
    {
      return Items.Contains(item);
    }

    // Internal

    public IEnumerator<T> GetEnumerator()
    {
      return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    protected virtual void OnElementRemoved(T item)
    {
      HandleItemRemoved<T> handler = Removed;
      handler?.Invoke(item);
    }

    protected virtual void OnAdded(T item)
    {
      HandleItemAdded<T> handler = Added;
      handler?.Invoke(item);
    }

    public override string ToString()
    {
      return string.Join(", ", Items.Select(i => i.ToString()).ToArray());
    }
  }
}