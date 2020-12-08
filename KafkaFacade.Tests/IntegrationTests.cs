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

        [Fact]
        public void CanProduceAndConsumeFromCloudInstance()
        {
            using(var producer = new AvroProducerClient<User>(
                ProducerConfig(),
                SchemaRegistryConfig(),
                new AvroSerializerConfig{
                    BufferBytes = 100
                }
            ))
            {

                var user = new User(){
                    name = "Unit Test Name",
                    favorite_color = "green",
                    favorite_number = 18,
                    hourly_rate = 2.22
                };

                producer.Produce(_topic, "test1", user, (deliveryReport) =>
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
    }
}
