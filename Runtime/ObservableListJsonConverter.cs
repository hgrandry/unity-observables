using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HGrandry.Observables
{
    public class ObservableListJsonConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is ObservableList<T> obs))
                return;
        
            writer.WriteStartArray();
        
            foreach (T t in obs)
            {
                writer.WriteValue(t);        
            }
        
            writer.WriteEndArray();
        }
    
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obs = new ObservableList<T>();

            JArray array = JArray.Load(reader);

            foreach (JToken child in array.Children())
            {
                var item = child.ToObject<T>();
                obs.Add(item);
            }
            
            return obs;
        }
    
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}