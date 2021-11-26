
using System;
using System.Collections.Generic;
using System.Reflection;

internal class Method : Element
{
    public Type Type { get => MethodInfo.ReturnType; }
    public List<Element> ParameterObjects { get; protected set; }
    public MethodInfo MethodInfo { get; protected set; }

    public Method(MethodInfo method, IEnumerable<Element> parameterObjs)
    {
        MethodInfo = method;
        ParameterObjects = new List<Element>(parameterObjs);
        if (method.IsDefined(typeof(DataTargetAttribute)))
            DataId = method.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public override IEnumerable<Element> Children { get => ParameterObjects; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
