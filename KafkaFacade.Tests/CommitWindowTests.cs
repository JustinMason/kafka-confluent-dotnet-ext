using Moq;
using Xunit;

namespace KafkaFacade.Tests
{
    public class CommitWindowTests
    {

        [Fact]
        public void When_Initialized_Default_Time_Is_Elapsed(){

            var clockMock = new Mock<IClock>();

            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
            Assert.Equal(new System.DateTime(1999,12,31,1,1,1,1), clockMock.Object.Now);

            var commitWindow = new CommitWindow(clockMock.Object);

            Assert.True(commitWindow.Elasped); 
        }

        [Fact]
        public void When_Initialized_After_Recieved_Time_Not_Elapsed(){

            var clockMock = new Mock<IClock>();

            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
            Assert.Equal(new System.DateTime(1999,12,31,1,1,1,1), clockMock.Object.Now);

            var commitWindow = new CommitWindow(clockMock.Object);
            commitWindow.WindowMilliseconds = 10;
            commitWindow.Recieved();

            Assert.False(commitWindow.Elasped); 
        }

        [Fact]
        public void When_Initialized_After_Recieved_And_Time_Window_Is_Elapsed(){

            var clockMock = new Mock<IClock>();
            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
      
            var commitWindow = new CommitWindow(clockMock.Object);
            commitWindow.WindowMilliseconds = 10;
            commitWindow.Recieved();

            //Initial Recieved does not elaspe the window
            Assert.False(commitWindow.Elasped); 
            
            clockMock.Reset();
            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,10));

            //Second Recieved does not elapses at 9 milliseconds
            commitWindow.Recieved();
            Assert.False(commitWindow.Elasped); 

            clockMock.Reset();
            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,12));

            //Third Recieved After Reset Does Elasp Window 11 milliseconds
            commitWindow.Recieved();
            Assert.True(commitWindow.Elasped); 

            clockMock.Reset();
            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,13));

            //Forth Reset of Window Does not Elasp Window 1 milliseconds from Reset()
            commitWindow.Reset();
            Assert.False(commitWindow.Elasped); 

        }

        [Fact]
        public void When_Initialized_Elapsed_Event_Fires(){
            var clockMock = new Mock<IClock>();
            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
            var count = 0;
      
            var commitWindow = new CommitWindow(clockMock.Object);
            commitWindow.WindowElapsed += (e,a) =>{
                count++;
            };
            commitWindow.WindowMilliseconds = 200;
            Assert.Equal (count, 0);
            System.Threading.Thread.Sleep(200);
            Assert.Equal (count, 1);             
        }

        [Fact]
        public void When_Initialized_And_EventCount_Threshold_Met_Elasped(){

            var clockMock = new Mock<IClock>();

            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
            Assert.Equal(new System.DateTime(1999,12,31,1,1,1,1), clockMock.Object.Now);

            var commitWindow = new CommitWindow(clockMock.Object);
            commitWindow.WindowMilliseconds = 10;
            commitWindow.EventCountThreshold = 100;
            commitWindow.Recieved();

            for (int i = 0; i < 98; i++)
            {            
                commitWindow.Recieved();
            }
            Assert.False(commitWindow.Elasped);       

            commitWindow.Recieved();
            
            Assert.True(commitWindow.Elasped);       
        }

        

        public void When_EventCount_Threshol_Met_Elasped_Fired()
        {
            var clockMock = new Mock<IClock>();
            var count = 0;

            clockMock.Setup(x=> x.Now ).Returns(new System.DateTime(1999,12,31,1,1,1,1));
            Assert.Equal(new System.DateTime(1999,12,31,1,1,1,1), clockMock.Object.Now);

            var commitWindow = new CommitWindow(clockMock.Object);

            commitWindow.WindowElapsed += (e,a) =>{
                count++;
            };

            //Defaults raised elasped when initialized
            Assert.Equal(1, count);

            for (int i = 0; i < 98; i++)
            {            
                commitWindow.Recieved();
            }

            //Not raised untill 100 -  Default
            Assert.Equal(1, count);
            commitWindow.Recieved();
            
            Assert.Equal(2, count);
            

        }
    }
}