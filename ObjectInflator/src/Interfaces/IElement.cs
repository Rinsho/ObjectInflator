
using System;
using System.Collections.Generic;
using System.Reflection;

internal interface IElement
{
    bool HasDataScope { get => !string.IsNullOrWhiteSpace(DataId); }
    string DataId { get; }
    IEnumerable<IElement> Children { get; }
    void Accept(IVisitor visitor);
}