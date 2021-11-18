
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

internal class Field : IElement
{
    public Type Type { get => FieldInfo.FieldType; }
    public FieldInfo FieldInfo { get; protected set; }
    public IElement FieldObject { get; protected set; }

    public Field(FieldInfo field, IElement fieldObj)
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
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => new[] { FieldObject }; }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
