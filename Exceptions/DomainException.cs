using System;

namespace GeorgianRailwayApi.Exceptions
{
    public class DomainException : Exception
    {
        public string ErrorCode { get; }
        public DomainException(string message, string errorCode = "DomainError") : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : DomainException
    {
        public NotFoundException(string message, string errorCode = "NotFound") : base(message, errorCode) { }
    }
}
