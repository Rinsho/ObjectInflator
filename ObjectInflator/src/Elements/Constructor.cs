using System;
using System.Reflection;
using System.Collections.Generic;

internal class Constructor : Element
{
    public Type Type { get => ConstructorInfo.ReflectedType; }
    public ConstructorInfo ConstructorInfo { get; protected set; }
    public List<Element> ParameterObjects { get; protected set; }
    
    public Constructor(ConstructorInfo constructor, IEnumerable<Element> parameterObjs)
    {
        ConstructorInfo = constructor;
        ParameterObjects = new List<Element>(parameterObjs);
        if (constructor.IsDefined(typeof(DataTargetAttribute)))
            DataId = constructor.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public override IEnumerable<Element> Children { get => ParameterObjects; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}