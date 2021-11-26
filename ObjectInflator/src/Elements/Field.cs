
using System;
using System.Reflection;
using System.Collections.Generic;

internal class Field : Element
{
    public Type Type { get => FieldInfo.FieldType; }
    public FieldInfo FieldInfo { get; protected set; }
    public Element FieldObject { get; protected set; }

    public Field(FieldInfo field, Element fieldObj)
    {
        if (field.IsInitOnly || field.IsLiteral)
        {
            throw new MemberNotWriteableException(
                String.Join('.', field.DeclaringType.FullName, field.Name)
            );
        }
        FieldInfo = field;
        FieldObject = fieldObj;
        if (field.IsDefined(typeof(DataTargetAttribute)))
            DataId = field.GetCustomAttribute<DataTargetAttribute>().DataId;
    }
    
    //IElement interface
    public override IEnumerable<Element> Children { get => new[] { FieldObject }; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
