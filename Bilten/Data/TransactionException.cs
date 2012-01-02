using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Data
{
    public class TransactionException : ApplicationException
    {
        public TransactionException()
        {
        }

        public TransactionException(string message) : base(message)
        {
        }

        public TransactionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
