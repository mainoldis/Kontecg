using Kontecg.Authorization;
using Kontecg.Logging;
using Kontecg.Runtime.Validation;
using Kontecg.UI;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Logging
{
    public class LogSeverity_Tests: TestBaseWithLocalIocManager
    {
        [Fact]
        public void AuthorizationException_Default_Log_Severity_Test()
        {
            // change log severity ...
            KontecgAuthorizationException.DefaultLogSeverity = LogSeverity.Warn;

            var exception = new KontecgAuthorizationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void AuthorizationException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            KontecgAuthorizationException.DefaultLogSeverity = LogSeverity.Error;

            var exception = new KontecgAuthorizationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }

        [Fact]
        public void ValidationException_Default_Log_Severity_Test()
        {
            // change log severity ...
            KontecgValidationException.DefaultLogSeverity = LogSeverity.Warn;

            var exception = new KontecgValidationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void ValidationException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            KontecgValidationException.DefaultLogSeverity = LogSeverity.Error;

            var exception = new KontecgValidationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }

        [Fact]
        public void UserFriendlyException_Default_Log_Severity_Test()
        {
            // change log severity ...
            UserFriendlyException.DefaultLogSeverity = LogSeverity.Warn;

            var exception = new UserFriendlyException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void UserFriendlyException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            UserFriendlyException.DefaultLogSeverity = LogSeverity.Error;

            var exception = new UserFriendlyException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }
    }
}
