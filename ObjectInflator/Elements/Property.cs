
using System;
using System.Linq.Expressions;
using System.Reflection;

internal class Property : IMemberCreator
{
    protected IObjectCreator _obj;
    protected PropertyInfo _info;

    public Property(PropertyInfo property, IObjectCreator creator)
    {
        _info = property;
        _obj = creator;
    }

    public virtual Func<ParameterExpression, Expression> Construct()
    {
        if (!_info.CanWrite)
        {
            throw new MemberNotWriteableException(
                String.Join('.', _info.DeclaringType.FullName, _info.Name)
            );
        }
        Expression targetObj = _obj.Construct();
        return obj => Expression.Assign(Expression.MakeMemberAccess(obj, _info), targetObj);
    }
}
