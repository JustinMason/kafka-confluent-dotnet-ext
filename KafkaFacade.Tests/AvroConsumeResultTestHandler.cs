using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Xunit;
using KafkaFacade.Avro;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;


namespace KafkaFacade.Tests
{
    /// <summary>
    /// Mock Test Handler to Validate Consumer Interactions
    /// </summary>
    public class AvroConsumeResultTestHandler : IHandleAvroConsumeResultEvent
    {
        protected ILogger _logger;
        
        public void SetUpLogging()
        {
            var loggerFactory = LoggerFactory.Create(builder => {
                    builder.AddFilter("KafkaFacade", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("KafkaFacace.Tests", LogLevel.Debug)
                        .AddConsole();
                }
            );

            _logger = loggerFactory.CreateLogger("KafaFacade");
        }

        int _handled = 0;
        List<long> _offsets = new List<long>();
        public AvroConsumeResultTestHandler(List<long> offsets, int handleCount = 1)
        {
            HandleCount = handleCount;
            _offsets = offsets;
            SetUpLogging();
        }

        public AvroConsumeResultTestHandler(int handleCount = 1)
        {
            HandleCount = handleCount;
            SetUpLogging();
        }

        public int HandleCount { get; }

        public void ErrorHandler(IConsumer<string, GenericRecord> consumer, Error error)
        {
            Assert.True(false, error.Reason);
        }

        public void Handle(AvroConsumerClient avroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent)
        {
            if(avroConsumeResultEvent.ConsumeResult.Message.Value.Schema.Fullname =="Confluent.Kafka.Examples.AvroSpecific.User" )
                _logger.LogDebug ($"CONSUME Proto: {avroConsumeResultEvent.ConsumeResult.Partition} OFFSET: {avroConsumeResultEvent.ConsumeResult.Offset} NAME: {avroConsumeResultEvent.ToSpecificAsync<User>().GetAwaiter().GetResult().name} Standard RATE: {avroConsumeResultEvent.ToSpecificAsync<User>().GetAwaiter().GetResult().hourly_rate }"); 
            else if(avroConsumeResultEvent.ConsumeResult.Message.Value.Schema.Fullname =="Confluent.Kafka.Examples.AvroSpecific.HourBilled" )
                _logger.LogDebug ($"CONSUME Proto: {avroConsumeResultEvent.ConsumeResult.Partition} OFFSET: {avroConsumeResultEvent.ConsumeResult.Offset} NAME: {avroConsumeResultEvent.ToSpecificAsync<HourBilled>().GetAwaiter().GetResult().name} RATE: {avroConsumeResultEvent.ToSpecificAsync<HourBilled>().GetAwaiter().GetResult().rate_billed }"); 
    
            _handled++;

            if(_handled == HandleCount)
                avroConsumerClient.Close();
        }

        public void PartitionsRevokedHandleAction(IConsumer<string, GenericRecord> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
        {
            
        }
    }
}