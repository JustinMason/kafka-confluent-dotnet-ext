using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace KafkaFacade
{
    public class AvroConsumerClient
    {
        public ConsumerConfig _consumerConfig;
        SchemaRegistryConfig _schemaRegistryConfig;
        public AvroConsumerClient(ConsumerConfig consumerConfig, SchemaRegistryConfig schemaRegistryConfig)
        {
            _consumerConfig = consumerConfig;
            _schemaRegistryConfig = schemaRegistryConfig;
        }

    }
}