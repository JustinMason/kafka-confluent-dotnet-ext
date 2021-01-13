using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFacade.Tests
{
    public class BaseIntegrationTests
    {
      string _bootstrapServers = ".gcp.confluent.cloud:9092";
        string _cloudAccessKey = "";
        string _cloudAccessSecret = "";
        string _schemaRegistryUrl = "https://.gcp.confluent.cloud";
        string _schemaRegistryUserPassword = "key:secret";

        protected ProducerConfig ProducerConfig ()
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

        protected ConsumerConfig ConsumerConfig(){
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
        protected SchemaRegistryConfig SchemaRegistryConfig()
        {
            return new SchemaRegistryConfig {
                    Url = _schemaRegistryUrl,
                    BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = _schemaRegistryUserPassword
                };
        }

        protected AvroSerializerConfig AvroSerializerConfig(){
            return new AvroSerializerConfig{
                    BufferBytes = 100,                    
                    //SubjectNameStrategy = SubjectNameStrategy.TopicRecord,
                    //AutoRegisterSchemas = true
                };
        }
    }
}