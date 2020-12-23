using System;
using System.Threading;

namespace KafkaFacade
{
    public class CommitWindow
    {

        private readonly IClock _clock;
        private DateTime _lastCommit;
        private DateTime _lastMessageRecieved = DateTime.MinValue;
        private int _eventsRecieved = 0;

        private readonly Timer _timer;

        public event EventHandler WindowElapsed;

        public CommitWindow(IClock clock){
            _clock = clock;

            this.EventCountThreshold = 100;
            this.WindowMilliseconds = 1000;
            Reset();
            _timer = new Timer(this.Timer_Elapsed, null, 0, Timeout.Infinite);
        }

        public CommitWindow():this(new DefaulClock())
        {
        }

        public int WindowMilliseconds {get; set;}
        public int EventCountThreshold {get; set;}

        public void Recieved()
        {
            _lastMessageRecieved = _clock.Now;  
            _eventsRecieved ++;                                 
            
            if(Elasped)
                WindowElapsed?.Invoke(this, EventArgs.Empty);
        }

        public void Reset(){
            _lastCommit = _clock.Now;
            _eventsRecieved = 0;
        }


        private void Timer_Elapsed(object state)
        {
            WindowElapsed?.Invoke(this, EventArgs.Empty);
            _timer.Change(WindowMilliseconds, Timeout.Infinite);
        }

        public bool Elasped {
            get {
                return (_lastMessageRecieved == DateTime.MinValue || 
                    (_lastMessageRecieved - _lastCommit).Milliseconds > WindowMilliseconds) ||
                     _eventsRecieved >= EventCountThreshold;  
            }
        }
    }
}