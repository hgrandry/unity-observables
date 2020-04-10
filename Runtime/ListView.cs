using System;
using System.Collections;
using System.Collections.Generic;

namespace HGrandry.Observables
{
  /// <summary>
  /// Maintains a model-view mapping to ensure a unique view is created for each new model.
  /// Subscribe the view to an observable collection using myCollection.Subscribe(myListView) to keep the view in sync with the list of model.
  /// </summary>
  /// <typeparam name="TModel"></typeparam>
  /// <typeparam name="TView"></typeparam>
  public abstract class CollectionView<TModel, TView> : IListObserver<TModel>, IEnumerable<TView>
  {
    protected readonly Func<TModel, TView> CreateView;
    protected readonly Action<TModel, TView> DeleteView;

    protected readonly Dictionary<TModel, TView> ModelViewMapping = new Dictionary<TModel, TView>();

    /// <param name="createView">Method to create the view for a new model added to the model list</param>
    /// <param name="deleteView">Method to delete the view when a model is removed from the model list</param>
    public CollectionView(Func<TModel, TView> createView, Action<TModel, TView> deleteView)
    {
      DeleteView = deleteView;
      CreateView = createView;
    }

    public IEnumerable<TModel> Models => ModelViewMapping.Keys;

    public IEnumerable<TView> Views => ModelViewMapping.Values;

    public TView GetView(TModel model)
    {
      if (!ModelViewMapping.TryGetValue(model, out TView view))
        view = default;
      return view;
    }

    void IListObserver<TModel>.Add(TModel model)
    {
      if (ModelViewMapping.ContainsKey(model))
        return;

      TView view = CreateView(model);
      ModelViewMapping[model] = view;
    }

    void IListObserver<TModel>.Remove(TModel model)
    {
      if (!ModelViewMapping.ContainsKey(model))
        return;

      TView view = ModelViewMapping[model];
      ModelViewMapping.Remove(model);

      DeleteView(model, view);
    }

    public IEnumerator<TView> GetEnumerator()
    {
      return ModelViewMapping.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  public sealed class ListView<TModel, TView> : CollectionView<TModel, TView>
  {
    public ListView(Func<TModel, TView> createView, Action<TModel, TView> deleteView) : base(createView, deleteView)
    {
    }
  }

  public class OrderedListView<TModel, TView> : CollectionView<TModel, TView>, IOrderedListObserver<TModel>
  {
    private readonly Action<TView, int> _moveView;

    public OrderedListView(Func<TModel, TView> createView, Action<TModel, TView> deleteView, Action<TView, int> moveView
    )
        : base(createView, deleteView)
    {
      _moveView = moveView;
    }

    public void Insert(TModel model, int index)
    {
      if (ModelViewMapping.ContainsKey(model))
        return;

      TView view = CreateView(model);
      ModelViewMapping[model] = view;
      _moveView(view, index);
    }

    public void Move(TModel model, int index)
    {
      if (!ModelViewMapping.ContainsKey(model))
        return;

      TView view = ModelViewMapping[model];
      _moveView(view, index);
    }
  }
}