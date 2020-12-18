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
    }
}