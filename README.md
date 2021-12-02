# ObjectInflator
Generates and inflates objects provided tagged data.

Work in progress.

Current (unoptimized) benchmarks:
- 767 ns/obj for full generation of a type+object (element tree, expression compilation, and object creation).
- 220 ns/obj for object creation only.

If you want to test it or play around with it, the current details:

.NET Core 3.1 required

Should be able to run "dotnet test" to execute the nUnit tests from command line.
`ElementGenerator.Create<ObjectType>()` is the main entry-point for creating the Element tree.
`ConstructionVisitor` is passed to the Element tree to create the Expression tree.
`ConstructionVisitor.GetResult<DataType, ObjectType>` to retrieve the compiled lambda.

These are the core internal classes the public API will run on.

Currently supports:
- Data formatted as an `IDictionary<string, object>` where the strings are the data ids.
  - `object`s can be nested dictionaries.
- Unboxing for value-types is supported.
- Implicit and explicit conversion operators on the target type are supported. The run-time
  type of the data object must match the parameter of a conversion operator, otherwise
  a raw cast is used as the default. This means assigning a boxed value-type to another type
  other than it's unboxed type will fail if an implicit/explicit conversion operator does not
  exist for the conversion.
  - Note: This may be extended to support basic value-types without using implicit/explicit operators
    in the future, but general support is impossible without an additional layer of reflection
    during execution of the lambda which will significantly slow everything down.
- Nested types are supported, but not optimized at the moment.

***All of this is subject to change as the API is tweaked and improved.***
