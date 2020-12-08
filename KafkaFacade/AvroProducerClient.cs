using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Avro.Specific;
using Confluent.SchemaRegistry.Serdes;
using System.Threading.Tasks;
using System.Net;
using Confluent.Kafka.SyncOverAsync;

namespace KafkaFacade
{
    public class AvroProducerClient<T> : IProducerClient, IDisposable
        where T : ISpecificRecord
    {
        private readonly IProducer<string, T> _producer;
        private readonly CachedSchemaRegistryClient _schemaRegistryClient;
        public AvroProducerClient(ProducerConfig producerConfig, 
            SchemaRegistryConfig schemaRegistryConfig, 
            AvroSerializerConfig avroSerialzerConfig )
        {
            _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

            _producer = new ProducerBuilder<string, T>(producerConfig)
                .SetValueSerializer(new AvroSerializer<T>(_schemaRegistryClient, avroSerialzerConfig).AsSyncOverAsync())
                .SetKeySerializer(new AvroSerializer<String>(_schemaRegistryClient, avroSerialzerConfig).AsSyncOverAsync())
                .Build();
        }

        public AvroProducerClient(IProducerClient producerClient, 
            SchemaRegistryConfig schemaRegistryConfig, 
            AvroSerializerConfig avroSerialzerConfig )
        {
            _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

            _producer = new DependentProducerBuilder<string, T>(producerClient.Handle)
                .SetValueSerializer(new AvroSerializer<T>(_schemaRegistryClient, avroSerialzerConfig))
                .SetKeySerializer(new AvroSerializer<String>(_schemaRegistryClient, avroSerialzerConfig))
                .Build();
        }

        public async Task<DeliveryResult<string, T>> ProduceAsync(string topic, string key, T item)
        {
            return await _producer.ProduceAsync(topic, new Message<string, T> {Key=key, Value = item});
        }

        public void Produce(string topic, string key, T item, Action<DeliveryReport<string, T>> produceReportAction)
        {
            _producer.Produce(topic, new Message<string, T> {Key=key, Value = item}, produceReportAction);
        }

        public Handle Handle => _producer.Handle;

        public CachedSchemaRegistryClient SchemaRegistryClient => _schemaRegistryClient;

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
