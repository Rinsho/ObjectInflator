
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class Object : IObjectCreator
{
    private Constructor _constructor;
    private List<IMemberCreator> _members;
    public Type Type { get => _constructor.Type; }

    public Object(Constructor constructor, params IMemberCreator[] members)
        : this(constructor, (IEnumerable<IMemberCreator>) members)
    {}

    public Object(Constructor constructor, IEnumerable<IMemberCreator> members)
    {
        _constructor = constructor;
        _members = new List<IMemberCreator>(members);
    }

    public Expression Construct()
    {
        List<Expression> block = new List<Expression>();  
        ParameterExpression objVar = Expression.Variable(this.Type);
        
        //Construct this object and assign to variable
        block.Add(Expression.Assign(objVar, _constructor.Construct()));

        //Construct members
        foreach (IMemberCreator member in _members)
            block.Add(member.Construct()(objVar));

        //Set variable as return value of the block
        block.Add(objVar);

        return Expression.Block(this.Type, new[] { objVar }, block);
    }

}
