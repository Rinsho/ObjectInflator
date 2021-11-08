
using System;
using System.Linq.Expressions;
using System.Reflection;

internal class Field : IObjectBindable
{
    private IObjectCreator _obj;
    private FieldInfo _info;

    public Field(FieldInfo field, IObjectCreator creator)
    {
        _info = field;
        _obj = creator;
    }

    public Func<ParameterExpression, Expression> Construct()
    {
        if (_info.IsInitOnly || _info.IsLiteral)
        {
            throw new MemberNotWriteableException(
                String.Join('.', _info.DeclaringType.FullName, _info.Name)
            );
        }
        Expression targetObj = _obj.Construct();
        return obj => Expression.Assign(Expression.MakeMemberAccess(obj, _info), targetObj);
    }
}
