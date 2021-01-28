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
    public class ProtobufProducerClient : IProducerClient, IDisposable
    {
        private readonly IProducer<string, byte[]> _producer;

        private readonly ProtobufSerializerConfig _protobufSerialzerConfig;

        private readonly CachedSchemaRegistryClient _schemaRegistryClient;
        public ProtobufProducerClient(ProducerConfig producerConfig, 
            SchemaRegistryConfig schemaRegistryConfig, 
            ProtobufSerializerConfig protobufSerialzerConfig )
        {
            _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            _protobufSerialzerConfig = protobufSerialzerConfig;

            _producer = new ProducerBuilder<string, byte[]>(producerConfig)
                //.SetValueSerializer(new ProtobufSerializer<T>(_schemaRegistryClient, ProtobufSerialzerConfig).AsSyncOverAsync())
                .Build();
        }

        

        public async Task<DeliveryResult<string, byte[]>> ProduceAsync<T>(string topic, string key, T item)
            where T : IMessage<T>, new()
        {
            var serializer = new ProtobufSerializer<T>(_schemaRegistryClient, _protobufSerialzerConfig);
            var bytes = await serializer.SerializeAsync(item, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
        
            return await _producer.ProduceAsync(topic, new Message<string, byte[]> {Key=key, Value = bytes});
        }

        public void Produce<T>(string topic, string key, T item, Action<DeliveryReport<string, byte[]>> produceReportAction)
            where T : IMessage<T>, new()
        {
            var serializer = new ProtobufSerializer<T>(_schemaRegistryClient, _protobufSerialzerConfig).AsSyncOverAsync<T>();
            var bytes = serializer.Serialize(item, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
        
            _producer.Produce(topic, new Message<string, byte[]> {Key=key, Value = bytes}, produceReportAction);
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
