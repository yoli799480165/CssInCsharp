using System;

namespace CssInCSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class UnionAttribute<T1> : Attribute
    {
    }
}
