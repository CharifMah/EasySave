using Newtonsoft.Json;
namespace Stockage.Converters
{
    /// <summary>
    /// Concrete Collection Converter
    /// </summary>
    /// <typeparam name="TCollection">Collection</typeparam>
    /// <typeparam name="TItem">Item de la collection</typeparam>
    /// <typeparam name="TBaseItem">Item de base</typeparam>
    /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
    public class ConcreteCollectionTypeConverter<TCollection, TItem, TBaseItem> : JsonConverter where TCollection : ICollection<TBaseItem>, new() where TItem : TBaseItem
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var collection = new TCollection();
            var items = serializer.Deserialize<IEnumerable<TItem>>(reader);
            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
            return collection;
        }
        /// <summary>
        /// Can convert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(ICollection<TBaseItem>).IsAssignableFrom(objectType);
        }
    }
}
