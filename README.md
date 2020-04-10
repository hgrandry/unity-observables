# Observables

Minimalist observable library for Unity.

[Examples](https://github.com/hgrandry/unity-examples/tree/master/Assets/Observables)

```csharp
public class MyModel : MonoBehaviour, IInjectable
{
  // Simple value
  public Observable<int> Count = new Observable(1);

  // List
  public ObservableList<int> MyList = new ObservableList(new [] { 1, 2, 3, 4, 5});

}

public class MyComponent : MonoBehaviour
{
  [SerializedField] private MyModel _model;

  private void Awake()
  {
      _model.Count.Changed += HandleNewValue;

      // or to both observe the Changed event and immediatly act on the current value, i.e call HandleNewValue
      _model.Count.Subscrible(HandleNewValue);

      _model.Count.Set(2);
  }

  private void OnDestroy()
  {
      _model.Count.Changed -= HandleNewValue;
      // or
      _model.Count.Unscubscribe(HandleNewValue);
  }

  private void HandleNewValue(int value) { /*...*/ }
}
```
