using System;
using System.Linq.Expressions;

internal interface IObjectBindable
{
    Func<ParameterExpression, Expression> Construct();
}
