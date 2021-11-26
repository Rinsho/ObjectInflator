
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

internal class Property : Element
{
    public Type Type { get => PropertyInfo.PropertyType; }
    public Element PropertyObject { get; protected set; }
    public PropertyInfo PropertyInfo { get; protected set; }
    
    public Property(PropertyInfo property, Element propertyObj)
    {
        if (!property.CanWrite)
        {
            throw new MemberNotWriteableException(
                String.Join('.', property.DeclaringType.FullName, property.Name)
            );
        }
        PropertyInfo = property;
        PropertyObject = propertyObj;
        if (property.IsDefined(typeof(DataTargetAttribute)))
            DataId = property.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public override IEnumerable<Element> Children { get => new[] { PropertyObject }; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
