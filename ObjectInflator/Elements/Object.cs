
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class Object : IObjectCreator
{
    private Constructor _constructor;
    private List<IObjectBindable> _members;
    private Type _thisType;

    public Object(Type objectType, Constructor constructor, IObjectBindable[] members)
    {
        _thisType = objectType;
        _constructor = constructor;
        _members = new List<IObjectBindable>(members);
    }

    /* Construct():
        1) Create object - obj = Constructor.Construct
        2) Create fields - foreach fe = Field.Construct, fe(obj)
        3) Create properties - foreach pe = Property.Construct, pe(obj)
        4) Create methods - foreach me = Method.Construct, me(obj)
        5) Add obj to end of block
        6) Return block
    */
    public Expression Construct()
    {
        List<Expression> block = new List<Expression>();  
        ParameterExpression objVar = Expression.Variable(_thisType);
        
        //Construct this object and assign to variable
        block.Add(Expression.Assign(objVar, _constructor.Construct()));

        //Construct members
        foreach (IObjectBindable member in _members)
            block.Add(member.Construct()(objVar));

        //Set variable as return value of the block
        block.Add(objVar);

        return Expression.Block(_thisType, new[] { objVar }, block);
    }

}
