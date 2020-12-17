using System;

namespace KafkaFacade
{
    public class DefaulClock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}