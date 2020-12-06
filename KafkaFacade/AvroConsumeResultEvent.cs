using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Avro.Generic;
using System.Threading.Tasks;

namespace KafkaFacade
{
    public class AvroConsumeResultEvent
    {
        ConsumeResult<string, GenericRecord> _consumeResult;
        ISchemaRegistryClient _schemaRegistryClient;

        public AvroConsumeResultEvent(ISchemaRegistryClient schemaRegistryClient, ConsumeResult<string, GenericRecord> consumeResult){
            _consumeResult = consumeResult;
            _schemaRegistryClient = schemaRegistryClient;
        }

        public ConsumeResult<string, GenericRecord> ConsumeResult => _consumeResult;
        public ISchemaRegistryClient SchemaRegistryClient => _schemaRegistryClient;
        
        public async Task<T> ToSpecificAsync<T>()
        {
            return await _consumeResult.Message.Value.ToSpecificAsync<T>(_schemaRegistryClient);
        }
        
    }
}