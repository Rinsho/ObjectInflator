
using System;
using System.Linq.Expressions;
using System.Collections;
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
        _dictionaryIndexer = typeof(IDictionary<string, object>).GetProperty("Item");
        _arrayIndexer = typeof(IList).GetProperty("Item");
        _dataContext = new Stack<Expression>();
        _baseParameter = Expression.Parameter(typeof(IDictionary<string, object>));
        _dataContext.Push(
            Expression.MakeIndex(
                _baseParameter,
                _dictionaryIndexer,
                new[] { Expression.Constant(string.Empty) }
            )
        );
    }

    public void AddContextUsing(Element element)
    {
        _dataContext.Push(
            Expression.MakeIndex(
                Expression.Convert(
                    _dataContext.Peek(), 
                    typeof(IDictionary<string, object>)
                ),
                _dictionaryIndexer,
                new[] { Expression.Constant(element.DataId) }
            )
        );
    }

    public void AddContextUsing(IIterator iterator)
    {
        _dataContext.Push(
            Expression.MakeIndex(
                Expression.Convert(
                    _dataContext.Peek(),
                    typeof(IList)
                ), 
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