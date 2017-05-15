using System;

namespace PostgreEventStore.Domain
{
    public class Event
    {
        public string aggregateid { get; set; }
        public byte[] data { get; set; }
        public int version { get; set; }             
        public DateTime date { get; set; }
    }
}
