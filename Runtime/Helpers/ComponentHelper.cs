using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HGrandry.Observables
{
  internal static class ComponentHelper
  {
    public static T GetOrCreateComponent<T>(this GameObject go) where T : Component
    {
      var comp = go.GetComponent<T>();
      if (comp == null)
      {
        comp = go.AddComponent<T>();
      }
      return comp;
    }
  }
}