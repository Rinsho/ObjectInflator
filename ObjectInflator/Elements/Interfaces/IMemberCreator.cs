using System;
using System.Linq.Expressions;

internal interface IMemberCreator
{
    Func<ParameterExpression, Expression> Construct();
}
