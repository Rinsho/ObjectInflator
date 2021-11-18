using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

internal class Constructor : IElement
{
    public Type Type { get => ConstructorInfo.ReflectedType; }
    public ConstructorInfo ConstructorInfo { get; protected set; }
    public List<IElement> ParameterObjects { get; protected set; }
    
    public Constructor(ConstructorInfo constructor, IEnumerable<IElement> parameterObjs)
    {
        ConstructorInfo = constructor;
        ParameterObjects = new List<IElement>(parameterObjs);
        if (constructor.IsDefined(typeof(DataTargetAttribute)))
            DataId = constructor.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => ParameterObjects; }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}