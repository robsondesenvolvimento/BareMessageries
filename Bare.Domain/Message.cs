using System;

namespace Bare.Domain
{
    public class Message
    {
        public string MessageText { get; init; }
        public string ServiceId { get; init; }
        public DateTime TimeStamp { get; init; }
    }
}
