
using System;
using System.Reflection;
using System.Collections.Generic;

internal class Parameter : Element
{
    public Type Type { get => ParameterInfo.ParameterType; }
    public ParameterInfo ParameterInfo { get; protected set; }
    public Element ParameterObject { get; protected set; }

    public Parameter(ParameterInfo parameter, Element parameterObject)
    {
        ParameterInfo = parameter;
        ParameterObject = parameterObject;
        if (parameter.IsDefined(typeof(DataTargetAttribute)))
            DataId = parameter.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //Element
    public override IEnumerable<Element> Children { get => new Element[] { ParameterObject }; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}