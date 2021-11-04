namespace ScraperService
{
      using System.Runtime.Serialization;

      [Serializable]
      internal class FailedWebDriverLogin : Exception
      {
            public FailedWebDriverLogin()
            {
            }

            public FailedWebDriverLogin(string? message) : base(message)
            {
            }

            public FailedWebDriverLogin(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected FailedWebDriverLogin(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
      }
}