using System;

namespace KafkaFacade
{
    public class CommitWindow
    {

        private readonly IClock _clock;
        private DateTime _lastCommit;
        private DateTime _lastMessageRecieved = DateTime.MinValue;

        public CommitWindow(IClock clock){
            _clock = clock;
            _lastCommit =  _clock.Now;
        }

        public CommitWindow():this(new DefaulClock())
        {
        }

        public int WindowMilliseconds {get; set;}

        public void Recieved()
        {
            _lastMessageRecieved = _clock.Now;                                   
        }

        public void Reset(){
            _lastCommit = _clock.Now;
        }

        public bool Elasped {
            get {return _lastMessageRecieved == DateTime.MinValue || 
                    (_lastMessageRecieved - _lastCommit).Milliseconds > WindowMilliseconds ;}
        }
    }
}