
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

internal class Object : Element
{
    public Type Type { get => Constructor.Type; }
    public Constructor Constructor { get; protected set; }
    public List<Element> Members { get; protected set; } 

    public Object(Constructor constructor, IEnumerable<Element> members)
    {
        Constructor = constructor;
        Members = new List<Element>(members);
        if (Type.IsDefined(typeof(DataTargetAttribute)))
            DataId = Type.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public override IEnumerable<Element> Children { get => Members.Prepend(Constructor); }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);

}
