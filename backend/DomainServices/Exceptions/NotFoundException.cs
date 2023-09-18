using System;

namespace DomainServices.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string errorMessage) : base(errorMessage) { }
}