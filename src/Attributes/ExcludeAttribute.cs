using System;

namespace CssInCSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExcludeAttribute<T1> : Attribute
    {
        public string[] Keys { get; }

        public ExcludeAttribute(params string[] keys)
        {
            Keys = keys;
        }
    }
}
