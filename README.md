System.Core+
==============
A library of simple enhancements to System and System.Core. This single dll has no other dependencies, making it a lightweight addition to any project.

System.EventArgs<T>
--------------
An EventArgs class that adds a generic property T Argument, eliminating the need to create specialized subclasses that would only contain a single property.

System.StringExtensions
--------------
FormatString(): Adds the convenience of calling string.Format() like an instance method of string.

TrimNullSafe(): An extension method that calls Trim(), but can be used even when the string is null.

TrimWhitespace(): In addition to removing leading and trailing whitespace, replaces consecutive whitespaces with a single space. Has the ability to consider or ignore line breaks.

Truncate(): The counter to PadLeft(), will truncate a string at a given length.

System.IO.CsvReader/CsvWriter
--------------
A lightweight and efficient implementation that will correctly deal with escaped qualifiers and line breaks in fields.

System.Reflection.TypeName
--------------
Parses and reconstitutes assembly qualified type names.



