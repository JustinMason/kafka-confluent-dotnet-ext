using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.SyncOverAsync;

namespace KafkaFacade.Avro
{
    public class AvroConsumerClient : IDisposable
    {
        private readonly ConsumerConfig _consumerConfig;

        private readonly SchemaRegistryConfig _schemaRegistryConfig;

        private readonly IHandleAvroConsumeResultEvent _handleAvroConsumeResultEvent;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly CommitWindow _commitWindow = new CommitWindow();
        
        public AvroConsumerClient(ConsumerConfig consumerConfig,
            SchemaRegistryConfig schemaRegistryConfig,
            IHandleAvroConsumeResultEvent handleAvroConsumeResultEvent,
            int commitWindowMilliseconds = 1000)
        {
            _consumerConfig = consumerConfig;
            _schemaRegistryConfig = schemaRegistryConfig;
            _handleAvroConsumeResultEvent = handleAvroConsumeResultEvent;
            _commitWindow.WindowMilliseconds = commitWindowMilliseconds;
        }

        public Task Open(string topic)
        {
            return Task.Run(()=> {
                System.Threading.Thread.CurrentThread.Name = $"Task:{DateTime.Now.Ticks}";

                using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, GenericRecord>(_consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<GenericRecord>(schemaRegistry).AsSyncOverAsync())
                    .SetErrorHandler (_handleAvroConsumeResultEvent.ErrorHandler)
                    .SetPartitionsRevokedHandler(_handleAvroConsumeResultEvent.PartitionsRevokedHandleAction)
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
                                    _commitWindow.Recieved();
                                    var avroConsumeResultEvent = new AvroConsumeResultEvent(schemaRegistry, consumeResult);
                                    _handleAvroConsumeResultEvent?.Handle(this, avroConsumeResultEvent);

                                    if(_commitWindow.Elasped)
                                    {
                                        try
                                        {
                                            consumer.Commit();
                                        }
                                        catch (System.Exception err)
                                        {
                                            System.Diagnostics.Debug.Write (err.ToString());
                                        }

                                        _commitWindow.Reset();
                                    }
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