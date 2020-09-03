namespace HGrandry.Observables.Components
{
    public interface IObservableComponent<T>
    {
        Observable<T> Value { get; }
    }
}