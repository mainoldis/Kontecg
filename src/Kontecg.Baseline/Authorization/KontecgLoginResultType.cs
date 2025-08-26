namespace Kontecg.Authorization
{
    public enum KontecgLoginResultType : byte
    {
        Success = 1,

        InvalidUserNameOrEmailAddress,

        InvalidPassword,

        UserIsNotActive,

        InvalidCompanyName,

        CompanyIsNotActive,

        UserEmailIsNotConfirmed,

        UnknownExternalLogin,

        LockedOut,

        UserPhoneNumberIsNotConfirmed,

        FailedForOtherReason
    }
}
