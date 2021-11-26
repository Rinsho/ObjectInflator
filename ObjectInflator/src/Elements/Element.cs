
using System;
using System.Collections.Generic;

internal abstract class Element
{
    public bool HasDataScope { get => !string.IsNullOrWhiteSpace(DataId); }
    public string DataId { get; protected set; }
    public abstract IEnumerable<Element> Children { get; }
    public abstract void Accept(IVisitor visitor);
}