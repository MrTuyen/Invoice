using System;

namespace SPA_Invoice.Infrastructure.Exception
{
    public class SabEmailSendingException : RankException
    {
        public SabEmailSendingException()
        {
        }

        public SabEmailSendingException(string message)
            : base(message)
        {
        }

        public SabEmailSendingException(string message, RankException innerException)
            : base(message, innerException)
        {
        }
    }
}