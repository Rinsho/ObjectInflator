
using System;

[AttributeUsage(
	AttributeTargets.Property | 
	AttributeTargets.Field, 
	AllowMultiple = false, 
	Inherited = false
)]
public class NoInitAttribute : Attribute
{

}