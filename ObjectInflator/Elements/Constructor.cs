using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

//Only handles parameterless constructor atm
internal class Constructor : IObjectCreator
{
    private ConstructorInfo _constructor;
    private List<IObjectCreator> _params;

    public Constructor(ConstructorInfo constructor)
    {
        _constructor = constructor;
    }

    public Expression Construct()
    {
        return Expression.New(_constructor);
    }
}