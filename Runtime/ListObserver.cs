namespace HGrandry.Observables
{
  public interface IListObserver<in T>
  {
    void Add(T model);
    void Remove(T model);
  }

  public class ListObserver<T> : IListObserver<T>
  {
    private readonly HandleItemAdded<T> _add;
    private readonly HandleItemRemoved<T> _remove;

    public ListObserver(HandleItemAdded<T> add, HandleItemRemoved<T> remove)
    {
      _add = add;
      _remove = remove;
    }

    public void Add(T model)
    {
      _add(model);
    }

    public void Remove(T model)
    {
      _remove(model);
    }
  }
}