
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class Object : IObjectCreator
{
    private Constructor _constructor;
    private List<IMemberCreator> _members;
    private Type _thisType;

    public Object(Type objectType, Constructor constructor, IMemberCreator[] members)
    {
        _thisType = objectType;
        _constructor = constructor;
        _members = new List<IMemberCreator>(members);
    }

    public Expression Construct()
    {
        List<Expression> block = new List<Expression>();  
        ParameterExpression objVar = Expression.Variable(_thisType);
        
        //Construct this object and assign to variable
        block.Add(Expression.Assign(objVar, _constructor.Construct()));

        //Construct members
        foreach (IMemberCreator member in _members)
            block.Add(member.Construct()(objVar));

        //Set variable as return value of the block
        block.Add(objVar);

        return Expression.Block(_thisType, new[] { objVar }, block);
    }

}
