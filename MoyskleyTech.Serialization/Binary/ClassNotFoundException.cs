using System;

namespace MoyskleyTech.Serialization.Binary
{
    internal class ClassNotFoundException : Exception
    {
        public ClassNotFoundException()
        {
        }

        public ClassNotFoundException(string message) : base(message)
        {
        }

        public ClassNotFoundException(string message , Exception innerException) : base(message , innerException)
        {
        }
    }
}