using System;
namespace Discord.Commands
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true
    )]
    public class CategoryAttribute : Attribute
    {
        public string Category { get; }
        public CategoryAttribute(string cat)
        {
            Category = cat;
        }
    }
}