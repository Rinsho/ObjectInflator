using System;

[AttributeUsage(
	AttributeTargets.Property | 
	AttributeTargets.Field |
	AttributeTargets.Parameter,
	AllowMultiple = false,
	Inherited = true
)]
public class ParserDataAttribute : Attribute
{
	public string DataID { get; set; } = string.Empty;
}

[AttributeUsage(
	AttributeTargets.Property |
	AttributeTargets.Field |
	AttributeTargets.Parameter, 
	AllowMultiple = true, 
	Inherited = true
)]
public class OverrideParserDataAttribute : Attribute
{
	public string DataID { get; set; } = string.Empty;
	public string TargetName { get; }
	public string ParentMethodName { get; }

	public OverrideParserDataAttribute(string targetMemberName) =>
		TargetName = targetMemberName ?? string.Empty;

	public OverrideParserDataAttribute(string targetParameterName, string parentMethodName)
		: this(targetParameterName) =>
		ParentMethodName = parentMethodName ?? string.Empty;
}

[AttributeUsage(
	AttributeTargets.Property | 
	AttributeTargets.Field, 
	AllowMultiple = false, 
	Inherited = false
)]
public class NoInitAttribute : Attribute
{

}