using System;

namespace CssInCSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExtendsAttribute<T1> : Attribute
    {
        public string[] Keys { get; }

        public ExtendsAttribute(params string[] keys)
        {
            Keys = keys;
        }
    }
}
