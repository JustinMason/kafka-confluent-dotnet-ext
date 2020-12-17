using System;
using Xunit;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Avro;

namespace KafkaFacade.Tests
{
    public class AvroIntegrationTests : BaseIntegrationTests
    {
        
        string _topic = "users";


        [Fact]
        public void CanProduceAndConsumeFromCloudInstance()
        {
            using(var producer = new Avro.AvroProducerClient<User>(
                ProducerConfig(),
                SchemaRegistryConfig(),
                AvroSerializerConfig()
            ))
            {
                
                var user = new User(){
                    name = "Unit Test Name",
                    favorite_color = "green",
                    favorite_number = 18,
                    hourly_rate = 2.22
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
            
            using(var consumer = new Avro.AvroConsumerClient(ConsumerConfig(), 
                SchemaRegistryConfig(),
                new AvroConsumeResultTestHandler()))
            {
                var t = consumer.Open(_topic);
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }

        [Fact]
        public void CanProduceUserAndHourBilledAndConsumeBothFromCloudInstance()
        {
            using(var producerUser = new Avro.AvroProducerClient<User>(
                ProducerConfig(),
                SchemaRegistryConfig(),
                AvroSerializerConfig()

            ))
            using(var producerHoursBilled = new Avro.AvroProducerClient<HourBilled>(
                producerUser,
                AvroSerializerConfig()
            ))
            {

                var user = new User(){
                    name = "Unit Test Name",
                    favorite_color = "green",
                    favorite_number = 18,
                    hourly_rate = 2.22
                };

                var hourBilled = new HourBilled(){
                    name = "Unit Test Hour 10% Bonus",
                    rate_billed = Math.Round( AvroDecimal.ToDecimal( user.hourly_rate) * (decimal)0.1,2)
                };

                producerUser.Produce(_topic, "test1", user, (deliveryReport) =>
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

                producerHoursBilled.Produce(_topic, "test1", hourBilled, (deliveryReport) =>
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
            
            using(var consumer = new Avro.AvroConsumerClient(ConsumerConfig(), 
                SchemaRegistryConfig(),
                new AvroConsumeResultTestHandler(2)))
            {
                var t = consumer.Open(_topic);
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }
    }
}