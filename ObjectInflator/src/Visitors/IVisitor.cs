
internal interface IVisitor
{
    void Visit(Data element);
    void Visit(Field element);
    void Visit(Property element);
    void Visit(Method element);
    void Visit(PropertyIndex element);
    void Visit(Object element);
    void Visit(Array element);
    void Visit(Constructor element);
    void Visit(Parameter element);
}