using System;
using Xunit;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Confluent.Kafka;
using Confluent.Kafka.Examples.Protobuf;
using KafkaFacade.Protobuf;

namespace KafkaFacade.Tests
{
    public class ProtobufAvroIntegrationTests : BaseIntegrationTests
    {
        
        string _topic = "usersProto";


        [Fact]
        public void CanProduceAndConsumeFromCloudInstance()
        {
            using(var producer = new ProtobufProducerClient<User>(
                ProducerConfig(),
                SchemaRegistryConfig(),
                new ProtobufSerializerConfig()
            ))
            {
                
                var user = new User(){
                    Name = "Unit Test Name Proto",
                    FavoriteColor ="Proto",
                    FavoriteNumber = 9
                };

                producer.Produce(_topic,  "test1", user, (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        Assert.True(false, $"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                    }
                });
            }
            
            using(var consumer = new ProtobufConsumerClient<User>(ConsumerConfig(), 
                SchemaRegistryConfig(),
                new ProtobufConsumeResultTestHandler<User>()))
            {
                var t = consumer.Open(_topic);
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }

        
    }
}
