
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

internal class ConstructionVisitor : IVisitor
{
    private DataContext _dataContext;
    private Stack<Expression> _constructedElements;
    private Stack<ParameterExpression> _parentReferences;

    public ConstructionVisitor(DataContext dataContext)
    {
        _constructedElements = new Stack<Expression>();
        _parentReferences = new Stack<ParameterExpression>();    
        _dataContext = dataContext;
    }
    
    public Func<Dictionary<string, object>, object> GetResult()
    {
        return Expression.Lambda<Func<Dictionary<string, object>, object>>(
            _constructedElements.Pop(),
            false,          //Tail call optimization not applicable.
            _dataContext.GetBaseParameter()
        ).Compile(false);   //No interpretation if we can fully compile.
    }

    //IVisitor interface
    public void Visit(Data element)
    {
        _constructedElements.Push(
            Expression.ConvertChecked(_dataContext.GetCurrent(), element.Type)
        );
    }

    public void Visit(Array element)
    {
        //Get current data context and let this element filter it
        Expression data = _dataContext.AddContext(element);
        if (!data.Type.IsArray)
            throw new InvalidDataException("Array member requires array-type data.");

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
                        data,
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
        //Remove data context for this element
        _dataContext.RemoveCurrent();
    }
    
    public void Visit(Field element)
    {
        Expression data = _dataContext.AddContext(element);

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

        _dataContext.RemoveCurrent();
    }
    
    public void Visit(Property element)
    {
        Expression data = _dataContext.AddContext(element);

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

        _dataContext.RemoveCurrent();
    }
    
    public void Visit(Method element)
    {
        Expression data = _dataContext.AddContext(element);

        //Visit method's children
        List<Expression> parameters = new List<Expression>();
        foreach (IElement child in element.ParameterObjects)
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

        _dataContext.RemoveCurrent();
    }

    public void Visit(Object element)
    {
        Expression data = _dataContext.AddContext(element);

        //Create parent reference
        ParameterExpression objectReference = Expression.Parameter(element.Type);
        _parentReferences.Push(objectReference);

        List<Expression> members = new List<Expression>();

        //Visit object's constructor
        element.Constructor.Accept(this);
        members.Add(_constructedElements.Pop());

        //Visit object's members 
        foreach (IElement child in element.Members)
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

        _dataContext.RemoveCurrent();
        _parentReferences.Pop();
    }

    public void Visit(PropertyIndex element)
    {
        Expression data = _dataContext.AddContext(element);

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

        _dataContext.RemoveCurrent();
    }

    public void Visit(Constructor element)
    {
        Expression data = _dataContext.AddContext(element);

        //Visit constructor's children
        List<Expression> parameters = new List<Expression>();
        foreach (IElement child in element.ParameterObjects)
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

        _dataContext.RemoveCurrent();
    }
}