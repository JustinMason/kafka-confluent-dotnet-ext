using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFacade.Tests
{
    public class BaseIntegrationTests
    {
       protected string _bootstrapServers = "pkc-43n10.us-central1.gcp.confluent.cloud:9092";
        protected string _cloudAccessKey = "YD2MUJMM7MQ7FEPJ";
        protected string _cloudAccessSecret = "gS0Sj0LiqE/SAyjloa3LpTj0o/JxeNxibvbfNZj0Nws/D6xVbU40ENk9xZL9tfig";
        protected string _schemaRegistryUrl = "https://psrc-4nrnd.us-central1.gcp.confluent.cloud";
        protected string _schemaRegistryUserPassword = "VP3RJO2MI6PF2LAE:haMScw6UyGAhVqLaTpZvavJfcq6p/GnOCcoxM3TZJLO4jdHFzChOflR5FOa2HptX";


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