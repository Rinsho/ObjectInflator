
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class DataContext
{
    private Stack<Expression> _dataContext;
    private readonly PropertyInfo _dictionaryIndexer;
    private readonly PropertyInfo _arrayIndexer;
    private readonly ParameterExpression _baseParameter;

    public DataContext(ParameterExpression baseParameter)
    {
        _dictionaryIndexer = typeof(Dictionary<string, object>).GetProperty("Item");
        _arrayIndexer = typeof(Array).GetProperty("Item");
        _dataContext = new Stack<Expression>();
        _baseParameter = baseParameter;
        _dataContext.Push(_baseParameter);
    }

    public Expression AddContext(IElement element)
    {
        if (element.HasDataScope)
        {
            _dataContext.Push(
                Expression.MakeIndex(
                    _dataContext.Peek(),
                    _dictionaryIndexer,
                    new[] { Expression.Constant(element.DataId) }
                )
            );
        }
        return _dataContext.Peek();
    }

    public Expression AddContext(NumericIterator iterator)
    {
        _dataContext.Push(
            Expression.MakeIndex(
                _dataContext.Peek(), 
                _arrayIndexer, 
                new[] { iterator.InnerIterator }
            )
        );
        return _dataContext.Peek();
    }

    public Expression GetCurrent() => 
        _dataContext.Peek();

    public Expression RemoveCurrent() =>
        _dataContext.Pop();

    public ParameterExpression GetBaseParameter() =>
        _baseParameter;
}