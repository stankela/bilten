using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Exceptions
{
    /// <summary>
    /// This exception is used to mark (fatal) failures in infrastructure and system code.
    /// </summary>
    [Serializable]
    public class InfrastructureException : ApplicationException
    {
        public InfrastructureException()
        {
        }


        public InfrastructureException(string message)
            : base(message)
        {
        }


        public InfrastructureException(string message, Exception cause)
            : base(message, cause)
        {
        }


        public InfrastructureException(Exception cause)
            : base("Failure in infrastructure/system", cause)
        {
        }
    }
}
