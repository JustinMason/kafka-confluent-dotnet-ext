# kafka-confluent-dotnet-ext
.Net Confluent Kafka Facade to simplify integration with Kafka in .Net 

## AvroConsumerClient
The Avro Client encapsulates the Producer and Consumer initialization for Serialization.  It has a Commit Window, default 1000 MIlliseconds, that controls the period of time before each commit as Events are recieved.  The AvroConsumerClient provides the Initialization of a Consumer Task on Open.  The IHandleAvroConsumeResultEvent supplied as a constructor is intended to encapsulate Consumer Logic.  This Handler will be invoked after the Consumer is Opened.

### IHandleAvroConsumeResultEvent - Base Implementation
The AvroConsumeResultEventHandler provides a standard implementation for Consumers.  This implementation supports 'AutoCommit False' and will Commit handled Events on Partitions Revoked.

The AvroConsumeResultEvent provides a method to convert from GenericRecord to ISpecific.  This is intended to support Topics that have more than one type of Event/Avro Schema.

See: https://martin.kleppmann.com/2018/01/18/event-types-in-kafka-topic.html

## ProtobufConsumerClient

Follows the AvroConsumerClient's IHandle Pattern.  Confluent's Protobuf Serializer does not have a non-generic IMesssage implementation, and can not support topics that have more than one type of Event (Protobuf Schema).

This is a work in progress and is built to ultimately support Dependency Injection and Dynamic Event Handlers.
