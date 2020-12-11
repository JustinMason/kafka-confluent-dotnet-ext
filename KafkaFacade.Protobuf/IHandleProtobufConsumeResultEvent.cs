using System.Collections.Generic;
using Confluent.Kafka;
using Google.Protobuf;

namespace KafkaFacade.Protobuf
{
    public interface IHandleProtobufConsumeResultEvent<T>
        where T : class, IMessage<T>, new()
    {
        void Handle(ProtobufConsumerClient<T> ProtobufConsumerClient, ProtobufConsumeResultEvent<T> ProtobufConsumeResultEvent);

        void ErrorHandler(IConsumer<string, T> consumer, Error error);

        void PartitionsRevokedHandleAction(IConsumer<string, T> consumer, List<TopicPartitionOffset> topicPartitionOffsets);
    }
}