
using System;
using System.Linq.Expressions;

public interface IIterator
{
    ParameterExpression InnerIterator { get; }
    Expression Create(Expression data, Expression body);
}