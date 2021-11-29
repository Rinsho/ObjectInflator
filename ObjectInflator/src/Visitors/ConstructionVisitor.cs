
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

internal class ConstructionVisitor : IVisitor
{
    private DataContext _dataContext;
    private DataConverter _dataConverter;
    private Stack<Expression> _constructedElements;
    private Stack<ParameterExpression> _parentReferences;

    public ConstructionVisitor(DataContext dataContext, DataConverter converter)
    {
        _constructedElements = new Stack<Expression>();
        _parentReferences = new Stack<ParameterExpression>();    
        _dataContext = dataContext;
        _dataConverter = converter;
    }
    
    public Func<T, R> GetResult<T, R>()
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        BlockExpression body =
            Expression.Block(
                new[] { _dataContext.GetBaseParameter() },
                Expression.Assign(
                    _dataContext.GetBaseParameter(),
                    Expression.ListInit(
                        Expression.New(typeof(Dictionary<string, object>)),
                        Expression.ElementInit(
                            typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary<string, object>.Add)),
                            Expression.Constant(string.Empty),
                            Expression.Convert(parameter, typeof(object))
                        )
                    )
                ),
                _constructedElements.Pop()
            );
        return Expression.Lambda<Func<T, R>>(
            body,
            false,          //Tail call optimization not applicable.
            parameter
        ).Compile(false);   //No interpretation if we can fully compile.
    }

    //IVisitor interface
    private void Visit(Element element, Action<Expression> constructionHandler)
    {
        Action cleanup = () => {};
        if (element.HasDataScope)
        {
            _dataContext.AddContextUsing(element);
            cleanup = () => _dataContext.RemoveCurrent();
        }
        Expression data = _dataContext.GetCurrent();
        constructionHandler(data);
        cleanup();
    }

    public void Visit(Data element)
    {
        Visit(element, (data) => {
            _constructedElements.Push(
                Expression.Invoke(
                    _dataConverter.GetConverterFor(element.Type),
                    data
                )
            );
        });
    }

    public void Visit(Array element)
    {
        Visit(element, (data) => {
            /* Won't work, type is obj
            if (!data.Type.IsArray)
                throw new InvalidDataException("Array member requires array-type data.");
            */
            
            //Visit array's child
            element.ArrayObject.Accept(this);

            //Construct array element
            ParameterExpression array = Expression.Parameter(element.Type);
            Expression arrayElement = Expression.Block(
                array.Type,
                new[] { array },
                Expression.Assign(
                    array, 
                    Expression.NewArrayBounds(
                        array.Type.GetElementType(),
                        Expression.MakeMemberAccess(
                            Expression.Convert(data, typeof(Array)),
                            typeof(Array).GetMember("Length")[0]
                        )
                    )
                ),
                element.Iterator.Create(
                    data,
                    Expression.Assign(
                        Expression.ArrayAccess(
                            array, 
                            new[] { element.Iterator.InnerIterator }
                        ),
                        _constructedElements.Pop()
                    )
                ),
                array
            );

            //Push constructed array element for parent
            _constructedElements.Push(arrayElement);
        });
    }
    
    public void Visit(Field element)
    {
        Visit(element, (data) => {
            //Visit field's child
            element.FieldObject.Accept(this);

            //Construct field
            Expression fieldElement = Expression.Assign(
                Expression.MakeMemberAccess(
                    _parentReferences.Peek(), 
                    element.FieldInfo
                ), 
                _constructedElements.Pop()
            );

            //Push field element for parent
            _constructedElements.Push(fieldElement);
        });
    }
    
    public void Visit(Property element)
    {
        Visit(element, (data) => {
            //Visit property's child
            element.PropertyObject.Accept(this);

            //Construct property
            Expression propertyElement = Expression.Assign(
                Expression.MakeMemberAccess(
                    _parentReferences.Peek(), 
                    element.PropertyInfo), 
                _constructedElements.Pop()
            );

            //Push property element for parent
            _constructedElements.Push(propertyElement);
        });
    }
    
    public void Visit(Method element)
    {
        Visit(element, (data) => {
            //Visit method's children
            List<Expression> parameters = new List<Expression>();
            foreach (Element child in element.ParameterObjects)
            {
                child.Accept(this);
                parameters.Add(_constructedElements.Pop());
            }

            //Construct method
            Expression methodElement = Expression.Call(
                _parentReferences.Peek(), 
                element.MethodInfo,
                parameters
            );

            //Push method element for parent
            _constructedElements.Push(methodElement);
        });
    }

    public void Visit(Object element)
    {
        Visit(element, (data) => {
            //Create parent reference
            ParameterExpression objectReference = Expression.Parameter(element.Type);
            _parentReferences.Push(objectReference);

            List<Expression> members = new List<Expression>();

            //Visit object's constructor
            element.Constructor.Accept(this);
            members.Add(
                Expression.Assign(
                    objectReference,
                    _constructedElements.Pop()
                )
            );

            //Visit object's members 
            foreach (Element child in element.Members)
            {
                child.Accept(this);
                members.Add(_constructedElements.Pop());
            }

            //Add object reference as return
            members.Add(objectReference);

            //Construct object
            Expression objectElement = Expression.Block(
                element.Type,
                new[] { objectReference },
                members
            );

            //Push object element for parent
            _constructedElements.Push(objectElement);
            _parentReferences.Pop();
        });
    }

    public void Visit(PropertyIndex element)
    {
        Visit(element, (data) => {
            //Visit property index's child
            element.PropertyObject.Accept(this);

            //Construct property index
            Expression propertyIndexElement = element.Iterator.Create(
                data,
                Expression.Assign(
                    Expression.MakeIndex(
                        _parentReferences.Peek(), 
                        element.PropertyInfo, 
                        new[] { element.Iterator.InnerIterator }
                    ),
                    _constructedElements.Pop()
                )
            );

            //Push property index element for parent
            _constructedElements.Push(propertyIndexElement);
        });
    }

    public void Visit(Constructor element)
    {
        Visit(element, (data) => {
            //Visit constructor's children
            List<Expression> parameters = new List<Expression>();
            foreach (Element child in element.ParameterObjects)
            {
                child.Accept(this);
                parameters.Add(_constructedElements.Pop());
            }

            //Construct constructor
            Expression constructorElement = Expression.New(
                element.ConstructorInfo, 
                parameters
            );

            //Push constructor element for parent
            _constructedElements.Push(constructorElement);
        });
    }

    public void Visit(Parameter element)
    {
        Visit(element, (data) => {
            //Visit parameter's child
            element.ParameterObject.Accept(this);
        });
    }
}