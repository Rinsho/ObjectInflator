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
	//Index value for data
	public string DataId { get; set; } = string.Empty;
	//Ignore sub-targets and force data assignment here.
	public bool TruncateScope { get; set; } = false;
}

