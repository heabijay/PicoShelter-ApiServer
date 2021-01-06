using System;

namespace PicoShelter_ApiServer.BLL.Infrastructure
{

    [Serializable]
    public class HandlingException : Exception
    {
        public ExceptionType Type { get; set; }
        public new object Data { get; set; }
        public HandlingException(ExceptionType exception, object data = null) : base() 
        {
            this.Type = exception;
            this.Data = data;
        }
        protected HandlingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
