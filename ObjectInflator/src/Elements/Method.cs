
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class Method : IElement
{
    public Type Type { get => MethodInfo.ReturnType; }
    public List<IElement> ParameterObjects { get; protected set; }
    public MethodInfo MethodInfo { get; protected set; }

    public Method(MethodInfo method, IEnumerable<IElement> parameterObjs)
    {
        MethodInfo = method;
        ParameterObjects = new List<IElement>(parameterObjs);
        if (method.IsDefined(typeof(DataTargetAttribute)))
            DataId = method.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => ParameterObjects; }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
