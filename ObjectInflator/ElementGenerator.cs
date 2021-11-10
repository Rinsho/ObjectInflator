using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

namespace ExpressionGen
{
    internal class ElementGenerator
    {
        //Get ParserData fields.
        protected static IEnumerable<FieldInfo> GetFieldsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetRuntimeFields().Where(
                field => field.IsDefined(typeof(ParserDataAttribute))
            );
        }

        //Get ParserData properties.
        protected static IEnumerable<PropertyInfo> GetPropertiesOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetRuntimeProperties().Where(
                property => property.IsDefined(typeof(ParserDataAttribute))
            );
        }

        //Get ParserData methods. All args must be ParserData args or optional.
        protected static IEnumerable<MethodInfo> GetMethodsOf(Type targetType)
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

        //Get the parameterless constructor.
        protected static ConstructorInfo GetConstructorOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType.GetConstructors()
                .Where(constructor => constructor.GetParameters().Length == 0)
                .First();
        }

        protected static List<Field> CreateFieldElementsFrom(IEnumerable<FieldInfo> fieldInfos)
        {
            List<Field> fieldElements = new List<Field>();
            foreach (FieldInfo field in fieldInfos)
            {
                IObjectCreator fieldObjectElement = Create(field.FieldType);
                Field fieldElement = new Field(field, fieldObjectElement);
                fieldElements.Add(fieldElement);
            }
            return fieldElements;
        }

        protected static List<Property> CreatePropertyElementsFrom(IEnumerable<PropertyInfo> propertyInfos)
        {
            List<Property> propertyElements = new List<Property>();
            foreach (PropertyInfo property in propertyInfos)
            {
                IObjectCreator propertyObjectElement = Create(property.PropertyType);
                Property propertyElement = new Property(property, propertyObjectElement);
                propertyElements.Add(propertyElement);
            }
            return propertyElements;
        }

        protected static List<Method> CreateMethodElementsFrom(IEnumerable<MethodInfo> methodInfos)
        {
            List<Method> methodElements = new List<Method>();
            foreach (MethodInfo method in methodInfos)
            {
                List<IObjectCreator> methodObjectElements = new List<IObjectCreator>();
                IEnumerable<ParameterInfo> methodParams =
                    method.GetParameters().Where(
                        param => param.IsDefined(typeof(ParserDataAttribute))
                    );
                foreach (ParameterInfo parameter in methodParams)
                {
                    IObjectCreator methodObjectElement = Create(parameter.ParameterType);
                    methodObjectElements.Add(methodObjectElement);
                }
                Method methodElement = new Method(method, methodObjectElements);
                methodElements.Add(methodElement);
            }
            return methodElements;
        }

        public static IObjectCreator Create(Type type)
        {
            //Create constructor element
            ConstructorInfo constructor = GetConstructorOf(type);
            Constructor constructorElement = new Constructor(constructor);

            List<IMemberCreator> memberElements = new List<IMemberCreator>();
            //Create field elements
            memberElements.AddRange(CreateFieldElementsFrom(GetFieldsOf(type)));

            //Create property elements
            memberElements.AddRange(CreatePropertyElementsFrom(GetPropertiesOf(type)));

            //Create method elements
            memberElements.AddRange(CreateMethodElementsFrom(GetMethodsOf(type)));

            return new Object(constructorElement, memberElements);
        }
    }
}
