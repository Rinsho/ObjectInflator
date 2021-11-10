
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class Method : IMemberCreator
{
    private List<IObjectCreator> _params;
    private MethodInfo _info;

    public Method(MethodInfo method, IEnumerable<IObjectCreator> parameters)
    {
        _info = method;
        _params = new List<IObjectCreator>(parameters);
    }

    public Func<ParameterExpression, Expression> Construct()
    {
        List<Expression> arguments = new List<Expression>();
        foreach (IObjectCreator param in _params)
            arguments.Add(param.Construct());
        return obj => Expression.Call(obj, _info, arguments);
    }
}
