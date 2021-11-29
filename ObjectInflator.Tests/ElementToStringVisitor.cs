
using System;
using System.Text;

internal class ElementToStringVisitor : IVisitor
{
    private StringBuilder _result = new StringBuilder();
    public string Result { get => _result.ToString(); }

    public void VisitChildren(Element element)
    {
        foreach (Element child in element.Children)
            child.Accept(this);
    }

    public void Visit(Data element)
    {
        _result.Append("|Data");
    }

    public void Visit(Field element)
    {
        _result.Append("|Field");
        VisitChildren(element);
    }

    public void Visit(Property element)
    {
        _result.Append("|Property");
        VisitChildren(element);
    }

    public void Visit(Method element)
    {
        _result.Append("|Method");
        VisitChildren(element);
    }

    public void Visit(PropertyIndex element)
    {
        _result.Append("|PropertyIndex");
        VisitChildren(element);
    }

    public void Visit(Object element)
    {
        _result.Append("|Object");
        VisitChildren(element);
    }

    public void Visit(Array element)
    {
        _result.Append("|Array");
        VisitChildren(element);
    }

    public void Visit(Constructor element)
    {
        _result.Append("|Constructor");
        VisitChildren(element);
    }

    public void Visit(Parameter element)
    {
        _result.Append("|Parameter");
        VisitChildren(element);
    }
}