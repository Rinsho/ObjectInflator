
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

internal class Object : IElement
{
    public Type Type { get => Constructor.Type; }
    public Constructor Constructor { get; protected set; }
    public List<IElement> Members { get; protected set; } 

    public Object(Constructor constructor, IEnumerable<IElement> members)
    {
        Constructor = constructor;
        Members = new List<IElement>(members);
        if (Type.IsDefined(typeof(DataTargetAttribute)))
            DataId = Type.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => Members.Prepend(Constructor); }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);

}
