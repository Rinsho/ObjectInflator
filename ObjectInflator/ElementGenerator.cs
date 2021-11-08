using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

namespace ExpressionGen
{
    public class ElementGenerator
    {
        //Get ParserData fields.
        public static IEnumerable<FieldInfo> GetFieldsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetRuntimeFields().Where(
                field => field.IsDefined(typeof(ParserDataAttribute))
            );
        }

        //Get ParserData properties.
        public static IEnumerable<PropertyInfo> GetPropertiesOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetRuntimeProperties().Where(
                property => property.IsDefined(typeof(ParserDataAttribute))
            );
        }

        //Get ParserData methods. All args must be ParserData args or optional.
        public static IEnumerable<MethodInfo> GetMethodsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetRuntimeMethods().Where(
                method => {
                    ParameterInfo[] parameters = method.GetParameters();
                    return 
                        parameters.Length > 0 &&
                        parameters.All(parameter => 
                            parameter.IsOptional ||
                            parameter.IsDefined(typeof(ParserDataAttribute))
                        );
                }
            );
        }

    }
}
