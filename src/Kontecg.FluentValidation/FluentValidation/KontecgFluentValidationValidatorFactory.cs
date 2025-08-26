using System;
using FluentValidation;
using Kontecg.Dependency;

namespace Kontecg.FluentValidation
{
    public class KontecgFluentValidationValidatorFactory : ValidatorFactoryBase
    {
        private readonly IIocResolver _iocResolver;

        public KontecgFluentValidationValidatorFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            if (_iocResolver.IsRegistered(validatorType))
            {
                return _iocResolver.Resolve(validatorType) as IValidator;
            }

            return null;
        }
    }
}
