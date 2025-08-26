using Kontecg.Events.Bus;

namespace Kontecg.Tests.Events.Bus
{
    public class MySimpleEventData : EventData
    {
        public int Value { get; set; }

        public MySimpleEventData(int value)
        {
            Value = value;
        }
    }
}