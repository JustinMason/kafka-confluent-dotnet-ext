using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Avro.Specific;
using Confluent.SchemaRegistry.Serdes;
using System.Threading.Tasks;
using System.Net;
using Confluent.Kafka.SyncOverAsync;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KafkaFacade.Avro
{
    public class AvroProducerClient : IAvroProducer, IDisposable
    {
        private readonly IProducer<string, byte[]> _producer;
        private readonly CachedSchemaRegistryClient _schemaRegistryClient;
        private readonly AvroSerializerConfig _avroSerialzerConfig;
        private readonly Dictionary<Type, object> _serializers = new Dictionary<Type, object>(); 
        public AvroProducerClient(ProducerConfig producerConfig, 
            SchemaRegistryConfig schemaRegistryConfig, 
            AvroSerializerConfig avroSerialzerConfig )
        {
            _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            _avroSerialzerConfig = avroSerialzerConfig;

            _producer = new ProducerBuilder<string, byte[]>(producerConfig)
                //.SetValueSerializer(new AvroSerializer<T>(_schemaRegistryClient, avroSerialzerConfig).AsSyncOverAsync())
                .SetKeySerializer(new AvroSerializer<String>(_schemaRegistryClient, avroSerialzerConfig).AsSyncOverAsync())
                .Build();
        }

        public async Task<DeliveryResult<string, byte[]>> ProduceAsync<T>(string topic, string key, T item)
            where T : ISpecificRecord
        {
            var serializer = GetAvroSerializer<T>();
            var bytes = await serializer.SerializeAsync(item, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
        
            return await _producer.ProduceAsync(topic, new Message<string, byte[]> {Key=key, Value = bytes});
        }

        public void Produce<T>(string topic, string key, T item, Action<DeliveryReport<string, byte[]>> produceReportAction)
        {
            var serializer = GetAvroSerializer<T>();
            var bytes =  serializer.AsSyncOverAsync<T>().Serialize(item, 
                new Confluent.Kafka.SerializationContext(Confluent.Kafka.MessageComponentType.Value, topic));
        
            _producer.Produce(topic, new Message<string, byte[]> {Key=key, Value = bytes}, produceReportAction);
        }

        private AvroSerializer<T> GetAvroSerializer<T>()
        {
            if(!_serializers.ContainsKey(typeof(T)))
            {
                var serializer = new AvroSerializer<T>(_schemaRegistryClient, _avroSerialzerConfig);
                _serializers[typeof(T)]  = serializer;
            }

            return _serializers[typeof(T)] as AvroSerializer<T>;
        } 

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
