
using System;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

internal class NumericIterator : IIterator
{
    public ParameterExpression InnerIterator { get; protected set; }

    public NumericIterator()
    {
        InnerIterator = Expression.Parameter(typeof(int));
    }

    public Expression Create(Expression data, Expression body)
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
                                    Expression.Convert(data, typeof(ICollection)),
                                    typeof(ICollection).GetProperty(nameof(ICollection.Count))
                                )
                            ),
                            Expression.Break(exit)
                        ),
                        //Body start
                        body,
                        //Body end
                        Expression.PreIncrementAssign(InnerIterator)
                    ),
                    exit
                )
            );
    }
}