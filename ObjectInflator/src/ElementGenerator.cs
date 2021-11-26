using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("ObjectInflator.Tests")]

namespace ExpressionGen
{
    internal class ElementGenerator
    {
        private static readonly BindingFlags MEMBER_FLAGS = 
            BindingFlags.Public | 
            BindingFlags.NonPublic | 
            BindingFlags.Instance;

        protected static IEnumerable<FieldInfo> GetFieldTargetsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType
                .GetFields(MEMBER_FLAGS)
                .Where(field => field.IsDefined(typeof(DataTargetAttribute)));
        }

        protected static IEnumerable<PropertyInfo> GetPropertyTargetsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType
                .GetProperties(MEMBER_FLAGS)
                .Where(property => property.IsDefined(typeof(DataTargetAttribute)));
        }

        protected static IEnumerable<MethodInfo> GetMethodTargetsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            return targetType
                .GetMethods(MEMBER_FLAGS)
                .Where(method => {
                    ParameterInfo[] parameters = method.GetParameters();
                    return (
                        parameters.Length > 0 &&
                        parameters.All(parameter => 
                            parameter.IsOptional ||
                            parameter.IsDefined(typeof(DataTargetAttribute))
                        )
                    );
                });
        }

        protected static ConstructorInfo GetConstructorTargetsOf(Type targetType)
        {
            Debug.Assert(targetType != null);

            //Grab a constructor with DataTargets or the parameterless constructor.
            //Only a single constructor should be a DataTarget, otherwise it would be
            //arbitrary which one gets picked.
            //If no DataTargets and no parameterless constructor, then Single() will throw.
            return targetType
                .GetConstructors(MEMBER_FLAGS)
                .Where(constructor => {
                    ParameterInfo[] parameters = constructor.GetParameters();

                    return 
                        parameters.Length > 0 &&
                        parameters.All(parameter => 
                            parameter.IsOptional ||
                            parameter.IsDefined(typeof(DataTargetAttribute))
                        );
                }).DefaultIfEmpty(null)
                .Single();
        }

        protected static IEnumerable<Field> CreateFieldElementsFrom(IEnumerable<FieldInfo> fields)
        {
            Debug.Assert(fields != null);

            return fields.Select(field => 
                new Field(
                    field, 
                    CreateType(field.FieldType)
                )
            );
        }

        protected static IEnumerable<Property> CreatePropertyElementsFrom(IEnumerable<PropertyInfo> properties)
        {
            Debug.Assert(properties != null);

            return properties.Select(property => {
                if (property.GetIndexParameters().Length > 0)
                {
                    return new PropertyIndex(
                        property,
                        new NumericIterator(),
                        CreateType(property.PropertyType)
                    );
                }
                return new Property(
                    property, 
                    CreateType(property.PropertyType)
                );
            });
        }

        protected static IEnumerable<Method> CreateMethodElementsFrom(IEnumerable<MethodInfo> methods)
        {
            Debug.Assert(methods != null);

            return methods.Select(method => 
                new Method(
                    method,
                    method
                        .GetParameters()
                        .Where(parameter => parameter.IsDefined(typeof(DataTargetAttribute)))
                        .Select(parameter => CreateType(parameter.ParameterType))
                )
            );
        }

        protected static Constructor CreateConstructorElementFrom(ConstructorInfo constructor)
        {
            Debug.Assert(constructor != null);

            return new Constructor(
                constructor,
                constructor
                    .GetParameters()
                    .Where(parameter => parameter.IsDefined(typeof(DataTargetAttribute)))
                    .Select(parameter => CreateType(parameter.ParameterType))
            );
        }

        public static Element CreateType(Type type)
        {
            Debug.Assert(type != null);

            //Check for arrays of type [][] and [,] which are not supported.
            if (type.IsVariableBoundArray ||
                (type.IsSZArray && type.GetElementType().IsArray)
            )
            {
                throw new TypeNotSupportedException("Multi-dimensional arrays not supported.");
            } 

            //Handle single-dimensional arrays
            if (type.IsArray)
                return new Array(type, new NumericIterator(), CreateType(type.GetElementType()));
            
            //Get any targeted members
            IEnumerable<Element> members =
                CreateFieldElementsFrom(GetFieldTargetsOf(type))
                .Concat<Element>(CreatePropertyElementsFrom(GetPropertyTargetsOf(type)))
                .Concat<Element>(CreateMethodElementsFrom(GetMethodTargetsOf(type)));

            //Get targeted constructor if it exists.
            ConstructorInfo constructor = GetConstructorTargetsOf(type);

            //If there're no targets, then this is a data element.
            if (members.Count() < 1 && constructor == null)
                return new Data(type);
            
            //Otherwise the type has targets, create the object container
            //using the parameterless constructor if there was no targeted constructor.
            return new Object(
                CreateConstructorElementFrom(
                    constructor ?? 
                    type.GetConstructor(Type.EmptyTypes) ??
                    throw new TypeNotSupportedException("No valid targeted or parameterless constructor found.")), 
                members);
        }

        public static Element CreateType<T>() =>
            CreateType(typeof(T));
    }
}
