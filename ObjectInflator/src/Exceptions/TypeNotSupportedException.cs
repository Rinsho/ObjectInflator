
using System;

public class TypeNotSupportedException : Exception
{
    public TypeNotSupportedException(string message): base(message) {}
}