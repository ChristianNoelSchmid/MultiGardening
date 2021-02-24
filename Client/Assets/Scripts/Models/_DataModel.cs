using System;

namespace Server.Models
{
    /* Serves as a wrapper for JSON objects being sent to and from server.  */
    public class DataModel
    {
        public string Secret { get; set; }
        public Guid CallerId { get; set; }
    }
    public class DataModel<T>
    {
        public string Secret { get; set; }
        public Guid CallerId { get; set; }
        public T Value { get; set; }
    }
}