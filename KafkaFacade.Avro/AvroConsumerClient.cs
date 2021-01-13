using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.SyncOverAsync;
using Microsoft.Extensions.Logging;

namespace KafkaFacade.Avro
{
    public class AvroConsumerClient : IDisposable
    {
        private readonly ILogger _logger;

        private readonly ConsumerConfig _consumerConfig;

        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public AvroConsumerClient(ConsumerConfig consumerConfig,
            SchemaRegistryConfig schemaRegistryConfig,
            ILogger<AvroConsumerClient> logger,
            int commitWindowMilliseconds = 1000)
        {
            _consumerConfig = consumerConfig;
            _schemaRegistryConfig = schemaRegistryConfig;
            _logger = logger;

            _consumerConfig.EnableAutoOffsetStore = false;
            _consumerConfig.AutoCommitIntervalMs = commitWindowMilliseconds;
            _consumerConfig.EnableAutoCommit = true;
        }

        public Task Open(string topic, IHandleAvroConsumeResultEvent handler)
        {
            return Task.Run(()=> {
                System.Threading.Thread.CurrentThread.Name = $"Task:{DateTime.Now.Ticks}";

                using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, GenericRecord>(_consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<GenericRecord>(schemaRegistry).AsSyncOverAsync())
                    .SetErrorHandler (handler.ErrorHandler)
                    .SetPartitionsRevokedHandler(handler.PartitionsRevokedHandleAction)
                    .Build())
                    {
                        consumer.Subscribe(topic);

                        try
                        {
                            while(!_cancellationTokenSource.IsCancellationRequested)
                            {
                                var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                                if(consumeResult != null && consumeResult.Message != null)
                                {
                                    var avroConsumeResultEvent = new AvroConsumeResultEvent(schemaRegistry, consumeResult);
                                    handler?.Handle(this, avroConsumeResultEvent);
                                    consumer.StoreOffset(consumeResult);   
                                }
                            }
                        }
                        catch(OperationCanceledException){}
                        finally{
                            consumer.Close();
                        }
                    }
               }
            );
        }

        public void Close(){
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}