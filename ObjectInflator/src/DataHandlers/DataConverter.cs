
using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

internal class DataConverter
{
    protected BindingFlags _operatorFlags = BindingFlags.Public | BindingFlags.Static;
    protected string _implicitOperatorName = "op_Implicit";
    protected string _explicitOperatorName = "op_Explicit";

    public virtual LambdaExpression GetConverterFor(Type targetType)
    {
        ParameterExpression data = Expression.Parameter(typeof(object));
        return Expression.Lambda(
            CreateCompileTimeConversionsFor(targetType, data), 
            data
        );
    }

    protected Expression CreateCompileTimeConversionsFor(
        Type targetType, 
        ParameterExpression data
    ) 
    {
        //Get all implicit and explicit conversion operators on the
        //targetType that convert to the targetType using a single parameter.
        //Create switch cases that test the given data object's run-time type
        //against these parameter types, converting the data into a compatible
        //parameter type (unboxes too) before calling the conversion operator.
        
        IEnumerable<SwitchCase> cases = 
            targetType.GetMember(_implicitOperatorName, _operatorFlags)
            .Concat(targetType.GetMember(_explicitOperatorName, _operatorFlags))
            .Cast<MethodInfo>()
            .Where(converter => converter.ReturnType == targetType)
            .Where(converter => converter.GetParameters().Length == 1)
            .Select(converter => {
                Type parameterType = converter.GetParameters()[0].ParameterType;
                return Expression.SwitchCase(
                    Expression.Call(null, converter, Expression.Convert(data, parameterType)),
                    Expression.Constant(parameterType)
                );
            });
        SwitchExpression switchExpression = Expression.Switch(
            targetType,
            Expression.Call(
                data,
                data.Type.GetMethod(nameof(object.GetType))
            ),
            Expression.Convert(data, targetType),
            null,
            cases
        );
        return switchExpression;
    } 
}