
using System;
using System.Linq.Expressions;

internal class Data : IObjectCreator
{
    public string Id { get; }
    public ParameterExpression Parameter { get; }

    public Data(Type dataType, string dataId)
    {
        this.Parameter = Expression.Parameter(dataType, dataId);
        this.Id = dataId;
    }

    public Expression Construct() => this.Parameter;
}
