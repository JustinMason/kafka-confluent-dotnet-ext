using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Google.Protobuf;
using System.Threading.Tasks;

namespace KafkaFacade.Protobuf
{
    public class ProtobufConsumeResultEvent<T>
        where T : class, IMessage<T>, new()
    {
        ConsumeResult<string, T> _consumeResult;
        ISchemaRegistryClient _schemaRegistryClient;

        public ProtobufConsumeResultEvent(ISchemaRegistryClient schemaRegistryClient, ConsumeResult<string, T> consumeResult){
            _consumeResult = consumeResult;
            _schemaRegistryClient = schemaRegistryClient;
        }

        public ConsumeResult<string, T> ConsumeResult => _consumeResult;
        public ISchemaRegistryClient SchemaRegistryClient => _schemaRegistryClient;
        
    
        
    }
}