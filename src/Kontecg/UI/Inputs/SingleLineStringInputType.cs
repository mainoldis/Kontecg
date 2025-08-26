using System;
using Kontecg.Runtime.Validation;

namespace Kontecg.UI.Inputs
{
    [Serializable]
    [InputType("SINGLE_LINE_STRING")]
    public class SingleLineStringInputType : InputTypeBase
    {
        public SingleLineStringInputType()
        {
        }

        public SingleLineStringInputType(IValueValidator validator)
            : base(validator)
        {
        }
    }
}
