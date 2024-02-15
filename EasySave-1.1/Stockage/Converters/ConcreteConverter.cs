using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Stockage.Converters
{
    /// <summary>
    /// This convert can be used on any interface definition to instruct the JSON
    /// serializer to use a specific concrete class when deserializing the instance.
    /// The type specified by TConcrete must implement the interface specified by
    /// TInterface.
    /// </summary>
    /// <typeparam name="TInterface">The Type that was serialized into the JSON text.</typeparam>
    /// <typeparam name="TConcrete">The Type that specifies the class that will be created.</typeparam>
    public class ConcreteConverter<TInterface, TConcrete> : JsonConverter where TConcrete : TInterface, new()
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>Returns true if this instance can convert the specified object type, false otherwise.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TInterface);
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can write
        /// JSON.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            object returnValue = null;
            if (objectType == typeof(TInterface))
            {
                // ***
                // *** Deserialize the object to a temporary instance
                // ***
                JToken jsonToken = JToken.Load(reader);
                if (jsonToken is JObject || jsonToken is JArray)
                {
                    // ***
                    // *** Create the concrete type
                    // ***
                    returnValue = new TConcrete();
                    using JsonReader serializerReader = jsonToken.CreateReader();
                    // ***
                    // *** Populate the object
                    // ***
                    serializer.Populate(serializerReader, returnValue);
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
