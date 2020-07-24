using System;

namespace MoyskleyTech.Mathematics
{
    internal class SizeMismatchException : Exception
    {
        public SizeMismatchException():base("Size Mismatch")
        {
        }

        public SizeMismatchException(string message) : base(message)
        {
        }

        public SizeMismatchException(string message , Exception innerException) : base(message , innerException)
        {
        }
    }
}