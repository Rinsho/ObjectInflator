using System;

[AttributeUsage(
	AttributeTargets.Property | 
	AttributeTargets.Field |
	AttributeTargets.Parameter,
	AllowMultiple = false,
	Inherited = true
)]
public class DataTargetAttribute : Attribute
{
	public string DataId { get; set; } = string.Empty;
	public bool ScopeOnly { get; set; } = false;
}

