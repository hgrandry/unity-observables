using System;
using Newtonsoft.Json;

namespace HGrandry.Observables
{
    public class ObservableJsonConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is Observable<T> obs)
                writer.WriteValue(obs.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object val = reader.Value;
            var obs = new Observable<T>();
            ((IUntypedObservable) obs).UntypedValue = val;
            return obs;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
    
    
}