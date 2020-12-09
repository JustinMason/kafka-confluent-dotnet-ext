using System;
using Xunit;
using KafkaFacade;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.IO;
using System.Linq;
using Confluent.Kafka.Examples.AvroSpecific;

namespace KafkaFacade.Tests
{
    public class IntegrationTests
    {
        
        string _topic = "users";
        string _bootstrapServers = ".gcp.confluent.cloud:9092";
        string _cloudAccessKey = "";
        string _cloudAccessSecret = "";
        string _schemaRegistryUrl = "https://.gcp.confluent.cloud";
        string _schemaRegistryUserPassword = "key:secret";

        private ProducerConfig ProducerConfig ()
        { 
            return new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = _cloudAccessKey,
                SaslPassword = _cloudAccessSecret,
                SslCaLocation = "/usr/local/etc/openssl@1.1/cert.pem"
            };
        }

        private ConsumerConfig ConsumerConfig(){
            return new ConsumerConfig(){
                    BootstrapServers = _bootstrapServers,
                    SaslMechanism = SaslMechanism.Plain,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslUsername = _cloudAccessKey,
                    SaslPassword = _cloudAccessSecret,
                    SslCaLocation = "/usr/local/etc/openssl@1.1/cert.pem",
                    ClientId = "UnitTest-ClientId",
                    GroupId = "UnitTest-ClientId-GroupId",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };
        }
        private SchemaRegistryConfig SchemaRegistryConfig()
        {
            return new SchemaRegistryConfig {
                    Url = _schemaRegistryUrl,
                    BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = _schemaRegistryUserPassword
                };
        }

        private AvroSerializerConfig AvroSerializerConfig(){
            return new AvroSerializerConfig{
                    BufferBytes = 100,                    
                    //SubjectNameStrategy = SubjectNameStrategy.TopicRecord,
                    //AutoRegisterSchemas = true
                };
        }

        [Fact]
        public void CanProduceAndConsumeFromCloudInstance()
        {
            using(var producer = new AvroProducerClient<User>(
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
            
            using(var consumer = new AvroConsumerClient(ConsumerConfig(), 
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
            using(var producerUser = new AvroProducerClient<User>(
                ProducerConfig(),
                SchemaRegistryConfig(),
                AvroSerializerConfig()

            ))
            using(var producerHoursBilled = new AvroProducerClient<HourBilled>(
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
                    rate_billed = Math.Round( Avro.AvroDecimal.ToDecimal( user.hourly_rate) * (decimal)0.1,2)
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
            
            using(var consumer = new AvroConsumerClient(ConsumerConfig(), 
                SchemaRegistryConfig(),
                new AvroConsumeResultTestHandler(2)))
            {
                var t = consumer.Open(_topic);
                Assert.True(t.Wait(10000), "Done Waiting, Consumed Something");
            }
        }
    }
}
