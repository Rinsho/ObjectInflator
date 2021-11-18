
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class NumericIterator : IIterator
{
    public ParameterExpression InnerIterator { get; protected set; }

    public NumericIterator()
    {
        InnerIterator = Expression.Variable(typeof(int));
    }

    public Expression Create(Expression data, Expression target)
    {
        LabelTarget exit = Expression.Label();
        return Expression.Block(
                new[] { InnerIterator },
                Expression.Assign(InnerIterator, Expression.Constant(0)),
                Expression.Loop(
                    Expression.Block(
                        //Exit condition
                        Expression.IfThen(
                            //loopVar >= outerData.Length
                            Expression.GreaterThanOrEqual(
                                InnerIterator, 
                                Expression.MakeMemberAccess(
                                    data,
                                    data.Type.GetMember(nameof(System.Array.Length))[0]
                                )
                            ),
                            Expression.Break(exit)
                        ),
                        //Body start
                        target,
                        //Body end
                        Expression.Increment(InnerIterator)
                    ),
                    exit
                )
            );
    }
}