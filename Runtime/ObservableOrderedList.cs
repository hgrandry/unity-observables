using System.Collections.Generic;

namespace HGrandry.Observables
{
  public delegate void HandledItemMoved<in T>(T item, int index);

  public interface IOrderedListObserver<in T> : IListObserver<T>
  {
    void Insert(T item, int index);
    void Move(T item, int index);
  }

  public class ObservableOrderedList<T> : ObservableCollection<T, List<T>, IOrderedListObserver<T>>
  {
    public ObservableOrderedList(IEnumerable<T> items) : base(items)
    {
    }

    public ObservableOrderedList() : base()
    {
    }

    event HandledItemMoved<T> Moved;
    event HandledItemMoved<T> Inserted;

    public void Insert(T item, int index)
    {
      Items.Insert(index, item);
      Inserted?.Invoke(item, index);
    }

    public void InsertBefore(T item, T otherItem)
    {
      int index = Items.IndexOf(otherItem);
      if (index < 0)
        return;

      Insert(item, index);
    }

    public void InsertAfter(T item, T otherItem)
    {
      int index = Items.IndexOf(otherItem);
      if (index < 0)
        return;

      Insert(item, index + 1);
    }

    public void Move(T item, int index)
    {
      if (index < 0 || Count <= index)
        return;

      var currentIndex = Items.IndexOf(item);
      if (currentIndex < 0 || currentIndex == index)
        return;

      Items.RemoveAt(currentIndex);

      //int insertIndex = currentIndex < index ? index - 1 : index;
      Items.Insert(index, item);

      Moved?.Invoke(item, index);
    }

    public void MoveUp(T item)
    {
      Move(item, Items.IndexOf(item) - 1);
    }

    public void MoveDown(T item)
    {
      Move(item, Items.IndexOf(item) + 1);
    }

    public void MoveFirst(T item)
    {
      Move(item, 0);
    }

    public void MoveLast(T item)
    {
      Move(item, Count - 1);
    }

    public void MoveBefore(T item, T otherItem)
    {
      int index = Items.IndexOf(otherItem);
      if (index < 0)
        return;

      Move(item, index);
    }

    public void MoveAfter(T item, T otherItem)
    {
      int index = Items.IndexOf(otherItem);
      if (index < 0)
        return;

      Move(item, index + 1);
    }

    public T this[int key] => Items[key];

    public override void Subscribe(IOrderedListObserver<T> observer)
    {
      base.Subscribe(observer);

      Moved += observer.Move;
      Inserted += observer.Insert;
    }

    public override void Unsubscribe(IOrderedListObserver<T> observer, bool callRemoveOnCurrentItems = false)
    {
      base.Unsubscribe(observer, callRemoveOnCurrentItems);

      Moved -= observer.Move;
      Inserted -= observer.Insert;
    }

    public int IndexOf(T item)
    {
      return Items.IndexOf(item);
    }
  }
}