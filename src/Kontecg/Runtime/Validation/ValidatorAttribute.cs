using System;

namespace Kontecg.Runtime.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatorAttribute : Attribute
    {
        public ValidatorAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
