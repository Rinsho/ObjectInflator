
using System;
using System.Linq.Expressions;

internal interface IIteratorGenerator
{
    ParameterExpression Iterator { get; }
}


//Not fully implemented; need to work some issues.
internal class Array : IObjectCreator, IIteratorGenerator
{
    private IObjectCreator _obj;
    private Data _outerData;
    public ParameterExpression Iterator { get; }

    public Array()
    {
        throw new NotImplementedException();
    }

    public Expression Construct()
    {
        if (!_outerData.Parameter.Type.IsArray)
            throw new InvalidDataException("Iteration requires array-type data.");
        
        LabelTarget exit = Expression.Label();
        ParameterExpression loopVar = Expression.Variable(typeof(int));
        return Expression.Block(
            new[] { loopVar },
            Expression.Assign(loopVar, Expression.Constant(0)),
            Expression.Loop(
                Expression.Block(
                    Expression.IfThen(
                        Expression.GreaterThanOrEqual(
                            loopVar, 
                            Expression.MakeMemberAccess(
                                _outerData.Parameter,
                                _outerData.Parameter.Type.GetMember("Length")[0]
                            )
                        ),
                        Expression.Break(exit)
                    ),
                    Expression.Assign(Iterator, loopVar),
                    //STUFF WITH ITERATOR HERE
                    Expression.Increment(loopVar)
                ),
                exit
            )
        );
    }
}
