
using System;
using System.Reflection;
using System.Collections.Generic;

internal class Data : Element
{
    public Type Type { get; protected set; }
    
    public Data(Type type)
    {
        Type = type;
        if (type.IsDefined(typeof(DataTargetAttribute)))
            DataId = type.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //Element
    public override IEnumerable<Element> Children { get => new Element[] {}; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}