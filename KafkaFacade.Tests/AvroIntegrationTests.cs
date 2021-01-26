using System;
using Xunit;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Avro;
using Microsoft.Extensions.Logging;
using KafkaFacade.Avro;

namespace KafkaFacade.Tests
{
    public class AvroIntegrationTests : BaseIntegrationTests
    {
        
        string _topic = "users";


        [Fact]
        public void CanProduceAndConsumeFromCloudInstance()
        {
            using(var producer = new Avro.AvroProducerClient(
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
                AvroSerializerConfig(),
                new LoggerFactory().CreateLogger<AvroConsumerClient>()))
            {
                var t = consumer.Open(_topic, new AvroConsumeResultTestHandler());
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }

        [Fact]
        public void CanProduceUserAndHourBilledAndConsumeBothFromCloudInstance()
        {
            using(var producer = new Avro.AvroProducerClient(
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

                var hourBilled = new HourBilled(){
                    name = "Unit Test Hour 10% Bonus",
                    rate_billed = Math.Round( AvroDecimal.ToDecimal( user.hourly_rate) * (decimal)0.1,2)
                };

                producer.Produce<User>(_topic, "test1", user, (deliveryReport) =>
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

                producer.Produce<HourBilled>(_topic, "test1", hourBilled, (deliveryReport) =>
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
                AvroSerializerConfig(),
                new LoggerFactory().CreateLogger<AvroConsumerClient>()))
            {
                var t = consumer.Open(_topic, new AvroConsumeResultTestHandler(2));
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }
    }
}
