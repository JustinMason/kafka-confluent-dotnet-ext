using System.Threading.Tasks;
using Avro.Generic;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFacade.Avro
{
    public static class Extensions
    {
        public static async Task<T> ToSpecificAsync<T>(this GenericRecord record,
            ISchemaRegistryClient schemaRegistry,
            string topic,
            AvroSerializerConfig serializerConfig,
            AvroDeserializerConfig deserializerConfig = null)
        {
            var serializer = new AvroSerializer<GenericRecord>(schemaRegistry, serializerConfig);
            var deserializer = new AvroDeserializer<T>(schemaRegistry, deserializerConfig);
            var bytes = await serializer.SerializeAsync(record, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
            return await deserializer.DeserializeAsync(bytes, false, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
        }
    }
}