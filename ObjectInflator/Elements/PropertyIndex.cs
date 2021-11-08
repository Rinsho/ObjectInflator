
using System;
using System.Linq.Expressions;
using System.Reflection;

//Only support single-parameter indexers atm
internal class PropertyIndex : Property
{
    protected Data _indexer;

    public PropertyIndex(PropertyInfo property, Data indexer, IObjectCreator creator)
        : base(property, creator)
    {
        _indexer = indexer;
    }

    public override Func<ParameterExpression, Expression> Construct()
    {
        if (!_info.CanWrite)
        {
            throw new MemberNotWriteableException(
                String.Join('.', _info.DeclaringType.FullName, _info.Name)
            );
        }
        return obj => {
            return Expression.Assign(
                Expression.MakeIndex(obj, _info, new[] { _indexer.Construct() }),
                _obj.Construct()
            );
        };
    }
}
