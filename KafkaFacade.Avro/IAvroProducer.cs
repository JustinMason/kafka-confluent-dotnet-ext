using System;
using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;

namespace KafkaFacade.Avro
{
    public interface IAvroProducer
    {
         Task<DeliveryResult<string, byte[]>> ProduceAsync<T>(string topic, string key, T item)
            where T : ISpecificRecord;

        void Produce<T>(string topic, string key, T item, Action<DeliveryReport<string, byte[]>> produceReportAction);
                
            
    }
}