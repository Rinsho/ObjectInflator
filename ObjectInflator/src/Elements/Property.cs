
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

internal class Property : IElement
{
    public Type Type { get => PropertyInfo.PropertyType; }
    public IElement PropertyObject { get; protected set; }
    public PropertyInfo PropertyInfo { get; protected set; }
    
    public Property(PropertyInfo property, IElement propertyObj)
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
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => new[] { PropertyObject }; }
    public virtual void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
