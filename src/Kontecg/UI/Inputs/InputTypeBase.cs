using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontecg.Collections.Extensions;
using Kontecg.Runtime.Validation;

namespace Kontecg.UI.Inputs
{
    [Serializable]
    public abstract class InputTypeBase : IInputType
    {
        protected InputTypeBase()
            : this(new AlwaysValidValueValidator())
        {
        }

        protected InputTypeBase(IValueValidator validator)
        {
            Attributes = new Dictionary<string, object>();
            Validator = validator;
        }

        public virtual string Name
        {
            get
            {
                TypeInfo type = GetType().GetTypeInfo();
                return type.IsDefined(typeof(InputTypeAttribute))
                    ? type.GetCustomAttributes(typeof(InputTypeAttribute)).Cast<InputTypeAttribute>().First().Name
                    : type.Name;
            }
        }

        /// <summary>
        ///     Gets/sets arbitrary objects related to this object.
        ///     Gets null if given key does not exists.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get => Attributes.GetOrDefault(key);
            set => Attributes[key] = value;
        }

        /// <summary>
        ///     Arbitrary objects related to this object.
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; }

        public IValueValidator Validator { get; set; }

        public static string GetName<TInputType>() where TInputType : IInputType
        {
            return ((IInputType) Activator.CreateInstance(typeof(TInputType))).Name;
        }
    }
}
