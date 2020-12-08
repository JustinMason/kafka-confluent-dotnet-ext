# kafka-confluent-dotnet-ext
.Net Confluent Kafka Facade to simplify integration with Kafka in .Net 

The Avro Clients encapsulate the Producer and Consumer initialization for Serialization. 

The AvroConsumerClient provides the Initialization of a Consumer Task on Open.  The IHandleAvroConsumeResultEvent supplied as a constructor is intended to encapsulate Consumer Logic.  This Handler will be invoked after the Consumer is Opened.
The AvroConsumeResultEventHandler provides a standard implementation for Consumers.  This implementation supports 'AutoCommit False' and will Commit handled Events on Partitions Revoked.
The AvroConsumeResultEvent provides a method to convert from GenericRecord to ISpecific.  This is intended to support Topics that have more than one type of Event/Avro Schema.

See: https://martin.kleppmann.com/2018/01/18/event-types-in-kafka-topic.html

This is a work in progress and is built to ultimately support Dependency Injection and Dynamic Event Handlers.
