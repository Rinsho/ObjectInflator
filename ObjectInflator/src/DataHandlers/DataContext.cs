
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

    public DataContext()
    {
        _dictionaryIndexer = typeof(Dictionary<string, object>).GetProperty("Item");
        _arrayIndexer = typeof(Array).GetProperty("Item");
        _dataContext = new Stack<Expression>();
        _baseParameter = Expression.Parameter(typeof(Dictionary<string, object>));
        _dataContext.Push(_baseParameter);
    }

    public void AddContextUsing(IElement element)
    {
        _dataContext.Push(
            Expression.MakeIndex(
                _dataContext.Peek(),
                _dictionaryIndexer,
                new[] { Expression.Constant(element.DataId) }
            )
        );
    }

    public void AddContextUsing(NumericIterator iterator)
    {
        _dataContext.Push(
            Expression.MakeIndex(
                _dataContext.Peek(), 
                _arrayIndexer, 
                new[] { iterator.InnerIterator }
            )
        );
    }

    public Expression GetCurrent() => 
        _dataContext.Peek();

    public Expression RemoveCurrent() =>
        _dataContext.Pop();

    public ParameterExpression GetBaseParameter() =>
        _baseParameter;
}