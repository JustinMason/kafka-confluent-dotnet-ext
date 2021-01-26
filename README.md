# kafka-confluent-dotnet-ext
.Net Confluent Kafka Facade to simplify integration with Kafka in .Net 

## AvroConsumerClient
The Avro Client encapsulates the Producer and Consumer initialization for Serialization.  The AvroConsumerClient provides the Initialization of a Consumer Task on Open.  The IHandleAvroConsumeResultEvent supplied as an argument on `Open` is intended to encapsulate Consumer Logic.  This Handler will be invoked after the Consumer is Opened and messages are consumed.

A note, the Consumer uses StoreOffset rather than simple auto commit.  The following settings disable the 'Auto OffsetStore' and only Store successfully 'Handled' messages.  This requires the following settings:

'''c#
    _consumerConfig.EnableAutoOffsetStore = false;
    _consumerConfig.AutoCommitIntervalMs = commitWindowMilliseconds;
    _consumerConfig.EnableAutoCommit = true;
'''

### IHandleAvroConsumeResultEvent - Base Implementation
The AvroConsumeResultEventHandler provides a standard implementation for Consumers.  This implementation will Commit handled Events on Partitions Revoked.

The AvroConsumeResultEvent provides a method to convert from GenericRecord to ISpecific.  This is intended to support Topics that have more than one type of Event/Avro Schema.

See: https://martin.kleppmann.com/2018/01/18/event-types-in-kafka-topic.html

## ProtobufConsumerClient

Follows the AvroConsumerClient's IHandle Pattern.  Confluent's Protobuf Serializer does not have a non-generic IMesssage implementation, and cannot support Consuming topics that have more than one type of Event (Protobuf Schema).

*This is a work in progress*
