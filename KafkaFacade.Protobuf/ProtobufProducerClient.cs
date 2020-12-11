using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System.Threading.Tasks;
using System.Net;
using Confluent.Kafka.SyncOverAsync;
using Google.Protobuf;

namespace KafkaFacade.Protobuf
{
    public class ProtobufProducerClient<T> : IProducerClient, IDisposable
        where T : IMessage<T>, new()
    {
        private readonly IProducer<string, T> _producer;
        private readonly CachedSchemaRegistryClient _schemaRegistryClient;
        public ProtobufProducerClient(ProducerConfig producerConfig, 
            SchemaRegistryConfig schemaRegistryConfig, 
            ProtobufSerializerConfig ProtobufSerialzerConfig )
        {
            _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);

            _producer = new ProducerBuilder<string, T>(producerConfig)
                .SetValueSerializer(new ProtobufSerializer<T>(_schemaRegistryClient, ProtobufSerialzerConfig).AsSyncOverAsync())
                .Build();
        }

        public ProtobufProducerClient(IProducerClient producerClient,  
            ProtobufSerializerConfig ProtobufSerialzerConfig )
        {
            _schemaRegistryClient = producerClient.SchemaRegistryClient;

            _producer = new DependentProducerBuilder<string, T>(producerClient.Handle)
                .SetValueSerializer(new ProtobufSerializer<T>(_schemaRegistryClient, ProtobufSerialzerConfig).AsSyncOverAsync())
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
