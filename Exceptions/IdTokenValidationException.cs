using System;

namespace SampleMvcApp.Exceptions;

internal sealed class IdTokenValidationException : Exception
{
    public IdTokenValidationException(string message) : base(message)
    {
    }
}