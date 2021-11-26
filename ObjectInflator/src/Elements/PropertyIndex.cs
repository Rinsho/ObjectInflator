
using System;
using System.Reflection;

internal class PropertyIndex : Property
{
    public IIterator Iterator { get; protected set; }

    public PropertyIndex(PropertyInfo property, IIterator iterator, Element propertyObj)
        : base (property, propertyObj)
    {
        Iterator = iterator;
    }

    //IElement interface
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
