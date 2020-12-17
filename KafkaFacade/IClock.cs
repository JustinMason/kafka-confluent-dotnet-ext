using System;

namespace KafkaFacade
{
    public interface IClock
    {
         DateTime Now {get;}
    }
}