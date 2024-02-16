using Newtonsoft.Json;
namespace Stockage.Converters
{
    /// <summary>
    /// A JSON converter for dictionaries of generic types
    /// </summary>
    /// <typeparam name="TDictionary">The dictionary type</typeparam>
    /// <typeparam name="TItem">The item type</typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
    public class ConcreteDictionnaryTypeConverter<TDictionary, TItem, TKey, TValue> : JsonConverter where TDictionary : IDictionary<TKey, TValue>, new() where TItem : TValue
    {
        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var collection = new TDictionary();
            var items = serializer.Deserialize<Dictionary<TKey, TItem>>(reader);
            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item.Key, item.Value);
                }
            }
            return collection;
        }
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<TKey, TValue>).IsAssignableFrom(objectType);
        }
    }
}
