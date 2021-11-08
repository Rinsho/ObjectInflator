using System.Linq.Expressions;

internal interface IObjectCreator
{
    Expression Construct();
}
