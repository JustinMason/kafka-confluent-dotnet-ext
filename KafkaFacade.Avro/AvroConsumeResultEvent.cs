using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Avro.Generic;
using System.Threading.Tasks;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFacade.Avro
{
    public class AvroConsumeResultEvent
    {
        readonly ConsumeResult<string, GenericRecord> _consumeResult;
        readonly ISchemaRegistryClient _schemaRegistryClient;

        readonly AvroSerializerConfig _avroSerializerConfig;

        public AvroConsumeResultEvent(ISchemaRegistryClient schemaRegistryClient, 
            ConsumeResult<string, GenericRecord> consumeResult,
            AvroSerializerConfig avroSerializerConfig)
        {
            _consumeResult = consumeResult;
            _schemaRegistryClient = schemaRegistryClient;
            _avroSerializerConfig = avroSerializerConfig;
        }

        public ConsumeResult<string, GenericRecord> ConsumeResult => _consumeResult;
        public ISchemaRegistryClient SchemaRegistryClient => _schemaRegistryClient;
        
        public async Task<T> ToSpecificAsync<T>()
        {
            return await _consumeResult.Message.Value.ToSpecificAsync<T>(
                _schemaRegistryClient,
                _consumeResult.Topic,
                _avroSerializerConfig);
        }
        
    }
}