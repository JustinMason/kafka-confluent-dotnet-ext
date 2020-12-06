using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace KafkaFacade
{
    public interface IProducerClient
    {
         Handle Handle {get;}
         CachedSchemaRegistryClient SchemaRegistryClient {get;}

    }
}