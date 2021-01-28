using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Google.Protobuf;

namespace KafkaFacade.Protobuf
{
    public interface IProtobufProducer
    {
         Task<DeliveryResult<string, byte[]>> ProduceAsync<T>(string topic, string key, T item)
            where T : IMessage<T>, new();
        
        void Produce<T>(string topic, string key, T item, Action<DeliveryReport<string, byte[]>> produceReportAction)
            where T : IMessage<T>, new();
    }
}