using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.SyncOverAsync;
using Google.Protobuf;

namespace KafkaFacade.Protobuf
{
    public class ProtobufConsumerClient<T> : IDisposable
        where T : class, IMessage<T>, new()
    {
        private readonly object _sync = new object();
        private readonly ConsumerConfig _consumerConfig;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;

        private readonly IHandleProtobufConsumeResultEvent<T> _handleProtobufConsumeResultEvent;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public ProtobufConsumerClient(ConsumerConfig consumerConfig,
            SchemaRegistryConfig schemaRegistryConfig,
            IHandleProtobufConsumeResultEvent<T> handleProtobufConsumeResultEvent)
        {
            _consumerConfig = consumerConfig;
            _schemaRegistryConfig = schemaRegistryConfig;
            _handleProtobufConsumeResultEvent = handleProtobufConsumeResultEvent;
        }

        public Task Open(string topic)
        {
            return Task.Run(()=> {
                System.Threading.Thread.CurrentThread.Name = $"Task:{DateTime.Now.Ticks}";

                using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                    .SetValueDeserializer(new ProtobufDeserializer<T>().AsSyncOverAsync())
                    //.SetErrorHandler (_handleProtobufConsumeResultEvent.ErrorHandler)
                    //.SetPartitionsRevokedHandler(_handleProtobufConsumeResultEvent.PartitionsRevokedHandleAction)
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
                                    var protobufConsumeResultEvent = new ProtobufConsumeResultEvent<T>(schemaRegistry, consumeResult);
                                    _handleProtobufConsumeResultEvent?.Handle(this, protobufConsumeResultEvent);
                                }
                            }
                        }
                        catch(OperationCanceledException){

                        }
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