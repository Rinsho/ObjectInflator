
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class Data : IElement
{
    public Type Type { get; protected set; }
    
    public Data(Type type)
    {
        Type = type;
        if (type.IsDefined(typeof(DataTargetAttribute)))
            DataId = type.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => new IElement[] {}; }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}