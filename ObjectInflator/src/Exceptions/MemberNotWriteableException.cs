using System;

public class MemberNotWriteableException : Exception
{
    private string _memberName;

    public MemberNotWriteableException(string memberName) : base()
    {
        _memberName = memberName;
    }

    public override string ToString() =>
        $"Member {_memberName} is not writeable.";
}
