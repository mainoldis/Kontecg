using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Caching.Configuration;
using Kontecg.Runtime.Caching.Memory;
using Kontecg.Runtime.Remoting;
using Kontecg.Runtime.Session;
using Kontecg.TestBase.Runtime.Session;
using Kontecg.Tests.MultiCompany;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Kontecg.Tests.Configuration
{
    public class SettingManager_Tests : TestBaseWithLocalIocManager
    {
        private enum MyEnumSettingType
        {
            Setting1 = 0,
            Setting2 = 1,
        }

        private const string MyAppLevelSetting = "MyAppLevelSetting";
        private const string MyAllLevelsSetting = "MyAllLevelsSetting";
        private const string MyNotInheritedSetting = "MyNotInheritedSetting";
        private const string MyEnumTypeSetting = "MyEnumTypeSetting";
        private const string MyEncryptedSetting = "MyEncryptedSetting";

        private SettingManager CreateSettingManager(bool multiCompanyIsEnabled = true)
        {
            return new SettingManager(
                CreateMockSettingDefinitionManager(),
                new KontecgMemoryCacheManager(
                    new CachingConfiguration(Substitute.For<IKontecgStartupConfiguration>())
                ),
                new MultiCompanyConfig
                {
                    IsEnabled = multiCompanyIsEnabled
                }, new TestCompanyStore(),
                new SettingEncryptionService(new SettingsConfiguration()),
                Substitute.For<IUnitOfWorkManager>());
        }

        [Fact]
        public async Task Should_Get_Default_Values_With_No_Store_And_No_Session()
        {
            var settingManager = CreateSettingManager();

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(42);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level default value");
        }

        [Fact]
        public async Task Should_Get_Stored_Application_Value_With_No_Session()
        {
            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(48);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level stored value");
        }

        [Fact]
        public async Task Should_Get_Correct_Values()
        {
            var session = CreateTestKontecgSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            session.CompanyId = 1;

            //Inherited setting

            session.UserId = 1;
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 stored value");

            session.UserId = 2;
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 2 stored value");

            session.UserId = 3;
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting))
                .ShouldBe("company 1 stored value"); //Because no user value in the store

            session.CompanyId = 3;
            session.UserId = 3;
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting))
                .ShouldBe("application level stored value"); //Because no user and company value in the store

            //Not inherited setting

            session.CompanyId = 1;
            session.UserId = 1;

            (await settingManager.GetSettingValueForApplicationAsync(MyNotInheritedSetting)).ShouldBe(
                "application value");
            (await settingManager.GetSettingValueForCompanyAsync(MyNotInheritedSetting, session.CompanyId.Value))
                .ShouldBe("default-value");
            (await settingManager.GetSettingValueAsync(MyNotInheritedSetting)).ShouldBe("default-value");

            (await settingManager.GetSettingValueAsync<MyEnumSettingType>(MyEnumTypeSetting)).ShouldBe(MyEnumSettingType
                .Setting1);
        }

        [Fact]
        public async Task Should_Get_All_Values()
        {
            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();

            (await settingManager.GetAllSettingValuesAsync()).Count.ShouldBe(5);

            (await settingManager.GetAllSettingValuesForApplicationAsync()).Count.ShouldBe(4);

            (await settingManager.GetAllSettingValuesForCompanyAsync(1)).Count.ShouldBe(2);
            (await settingManager.GetAllSettingValuesForCompanyAsync(1)).Count.ShouldBe(2);
            (await settingManager.GetAllSettingValuesForCompanyAsync(2)).Count.ShouldBe(0);
            (await settingManager.GetAllSettingValuesForCompanyAsync(3)).Count.ShouldBe(0);

            (await settingManager.GetAllSettingValuesForUserAsync(new UserIdentifier(1, 1))).Count.ShouldBe(1);
            (await settingManager.GetAllSettingValuesForUserAsync(new UserIdentifier(1, 2))).Count.ShouldBe(2);
            (await settingManager.GetAllSettingValuesForUserAsync(new UserIdentifier(1, 3))).Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Change_Setting_Values()
        {
            var session = CreateTestKontecgSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            //Application level changes

            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "53");
            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "54");
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting,
                "application level changed value");

            (await settingManager.SettingStore.GetSettingOrNullAsync(null, null, MyAppLevelSetting)).Value
                .ShouldBe("54");

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(54);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level changed value");

            //Company level changes

            session.CompanyId = 1;
            await settingManager.ChangeSettingForCompanyAsync(1, MyAllLevelsSetting, "company 1 changed value");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("company 1 changed value");

            //User level changes

            session.UserId = 1;
            await settingManager.ChangeSettingForUserAsync(1, MyAllLevelsSetting, "user 1 changed value");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 changed value");
        }

        [Fact]
        public async Task Should_Delete_Setting_Values_On_Default_Value()
        {
            var session = CreateTestKontecgSession();
            var store = new MemorySettingStore();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = store;
            settingManager.KontecgSession = session;

            session.CompanyId = 1;
            session.UserId = 1;

            //We can get user's personal stored value
            (await store.GetSettingOrNullAsync(1, 1, MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 stored value");

            //This will delete setting for the user since it's same as company's setting value
            await settingManager.ChangeSettingForUserAsync(1, MyAllLevelsSetting, "company 1 stored value");
            (await store.GetSettingOrNullAsync(1, 1, MyAllLevelsSetting)).ShouldBe(null);

            //We can get company's setting value
            (await store.GetSettingOrNullAsync(1, null, MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("company 1 stored value");

            //This will delete setting for company since it's same as application's setting value
            await settingManager.ChangeSettingForCompanyAsync(1, MyAllLevelsSetting, "application level stored value");
            (await store.GetSettingOrNullAsync(1, 1, MyAllLevelsSetting)).ShouldBe(null);

            //We can get application's value
            (await store.GetSettingOrNullAsync(null, null, MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level stored value");

            //This will delete setting for application since it's same as the default value of the setting
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting,
                "application level default value");
            (await store.GetSettingOrNullAsync(null, null, MyAllLevelsSetting)).ShouldBe(null);

            //Now, there is no setting value, default value should return
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level default value");
        }

        [Fact]
        public async Task Should_Save_Application_Level_Setting_As_Company_Setting_When_Multi_Company_Is_Disabled()
        {
            // Arrange
            var session = CreateTestKontecgSession(multiCompanyIsEnabled: false);

            var settingManager = CreateSettingManager(multiCompanyIsEnabled: false);
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            // Act
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting, "53");

            // Assert
            var value = await settingManager.GetSettingValueAsync(MyAllLevelsSetting);
            value.ShouldBe("53");
        }

        [Fact]
        public async Task Should_Get_Company_Setting_For_Application_Level_Setting_When_Multi_Company_Is_Disabled()
        {
            // Arrange
            var session = CreateTestKontecgSession(multiCompanyIsEnabled: false);

            var settingManager = CreateSettingManager(multiCompanyIsEnabled: false);
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            // Act
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting, "53");

            // Assert
            var value = await settingManager.GetSettingValueForApplicationAsync(MyAllLevelsSetting);
            value.ShouldBe("53");
        }

        [Fact]
        public async Task Should_Change_Setting_Value_When_Multi_Company_Is_Disabled()
        {
            // Arrange
            var session = CreateTestKontecgSession(multiCompanyIsEnabled: false);

            var settingManager = CreateSettingManager(multiCompanyIsEnabled: false);
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            //change setting value with "B"
            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "B");

            // it's ok
            (await settingManager.GetSettingValueForApplicationAsync(MyAppLevelSetting)).ShouldBe("B");

            //change setting with same value "B" again,
            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "B");

            //but was "A" ,that's wrong
            (await settingManager.GetSettingValueForApplicationAsync(MyAppLevelSetting)).ShouldBe("B");
        }

        [Fact]
        public async Task Should_Get_Encrypted_Setting_Value()
        {
            var session = CreateTestKontecgSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            session.CompanyId = 1;

            // User setting
            session.UserId = 2;
            (await settingManager.GetSettingValueAsync(MyEncryptedSetting)).ShouldBe("user_setting");

            // Company setting
            session.UserId = null;
            (await settingManager.GetSettingValueAsync(MyEncryptedSetting)).ShouldBe("company_setting");

            // App setting
            session.CompanyId = null;
            (await settingManager.GetSettingValueAsync(MyEncryptedSetting)).ShouldBe("app_setting");
        }

        [Fact]
        public async Task Should_Set_Encrypted_Setting_Value()
        {
            var session = CreateTestKontecgSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            session.CompanyId = 1;

            // User setting
            session.UserId = 2;
            await settingManager.ChangeSettingForUserAsync(session.ToUserIdentifier(), MyEncryptedSetting,
                "user_123qwe");

            var settingValue = await settingManager.SettingStore.GetSettingOrNullAsync(
                session.CompanyId,
                session.UserId,
                MyEncryptedSetting
            );

            settingValue.Value.ShouldBe("oKPqQDCAHhz+AEnl/r0fsw==");

            // Company setting
            session.UserId = null;
            await settingManager.ChangeSettingForCompanyAsync(session.GetCompanyId(), MyEncryptedSetting,
                "company_123qwe");

            settingValue = await settingManager.SettingStore.GetSettingOrNullAsync(
                session.CompanyId,
                session.UserId,
                MyEncryptedSetting
            );

            settingValue.Value.ShouldBe("1iEFHk/LJ7mq68phI/dWEA==");

            // App setting
            session.CompanyId = null;
            await settingManager.ChangeSettingForApplicationAsync(MyEncryptedSetting, "app_123qwe");

            settingValue = await settingManager.SettingStore.GetSettingOrNullAsync(
                session.CompanyId,
                session.UserId,
                MyEncryptedSetting
            );

            settingValue.Value.ShouldBe("EOi2wcQt1pi1K4qYycBBbg==");
        }

        [Fact]
        public async Task Should_Get_Changed_Encrypted_Setting_Value()
        {
            var session = CreateTestKontecgSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.KontecgSession = session;

            session.CompanyId = 1;

            // User setting
            session.UserId = 2;

            await settingManager.ChangeSettingForUserAsync(
                session.ToUserIdentifier(),
                MyEncryptedSetting,
                "new_user_setting"
            );

            var settingValue = await settingManager.GetSettingValueAsync(MyEncryptedSetting);
            settingValue.ShouldBe("new_user_setting");

            // Company Setting
            session.UserId = null;

            await settingManager.ChangeSettingForCompanyAsync(
                session.GetCompanyId(),
                MyEncryptedSetting,
                "new_company_setting"
            );

            settingValue = await settingManager.GetSettingValueAsync(MyEncryptedSetting);
            settingValue.ShouldBe("new_company_setting");

            // App Setting
            session.CompanyId = null;

            await settingManager.ChangeSettingForApplicationAsync(
                MyEncryptedSetting,
                "new_app_setting"
            );

            settingValue = await settingManager.GetSettingValueAsync(MyEncryptedSetting);
            settingValue.ShouldBe("new_app_setting");
        }

        private static TestKontecgSession CreateTestKontecgSession(bool multiCompanyIsEnabled = true)
        {
            return new TestKontecgSession(
                new MultiCompanyConfig {IsEnabled = multiCompanyIsEnabled},
                new DataContextAmbientScopeProvider<SessionOverride>(
                    new AsyncLocalAmbientDataContext()
                ),
                Substitute.For<ICompanyResolver>()
            );
        }

        private static ISettingDefinitionManager CreateMockSettingDefinitionManager()
        {
            var settings = new Dictionary<string, SettingDefinition>
            {
                {MyAppLevelSetting, new SettingDefinition(MyAppLevelSetting, "42")},
                {
                    MyAllLevelsSetting,
                    new SettingDefinition(MyAllLevelsSetting, "application level default value",
                        scopes: SettingScopes.Application | SettingScopes.Company | SettingScopes.User)
                },
                {
                    MyNotInheritedSetting,
                    new SettingDefinition(MyNotInheritedSetting, "default-value",
                        scopes: SettingScopes.Application | SettingScopes.Company, isInherited: false)
                },
                {MyEnumTypeSetting, new SettingDefinition(MyEnumTypeSetting, MyEnumSettingType.Setting1.ToString())},
                {
                    MyEncryptedSetting,
                    new SettingDefinition(MyEncryptedSetting, "", isEncrypted: true,
                        scopes: SettingScopes.Application | SettingScopes.Company | SettingScopes.User)
                }
            };

            var definitionManager = Substitute.For<ISettingDefinitionManager>();

            //Implement methods
            definitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(x =>
            {
                if (!settings.TryGetValue(x[0].ToString(), out var settingDefinition))
                {
                    throw new KontecgException("There is no setting defined with name: " + x[0]);
                }

                return settingDefinition;
            });
            definitionManager.GetAllSettingDefinitions().Returns(settings.Values.ToList());

            return definitionManager;
        }

        private class MemorySettingStore : ISettingStore
        {
            private readonly List<SettingInfo> _settings;

            public MemorySettingStore()
            {
                _settings = new List<SettingInfo>
                {
                    new SettingInfo(null, null, MyAppLevelSetting, "48"),
                    new SettingInfo(null, null, MyAllLevelsSetting, "application level stored value"),
                    new SettingInfo(1, null, MyAllLevelsSetting, "company 1 stored value"),
                    new SettingInfo(1, 1, MyAllLevelsSetting, "user 1 stored value"),
                    new SettingInfo(1, 2, MyAllLevelsSetting, "user 2 stored value"),
                    new SettingInfo(1, 2, MyEncryptedSetting,
                        "Bs90qo8Argqw3l4ZfWsRqQ=="), // encrypted setting: user_setting
                    new SettingInfo(1, null, MyEncryptedSetting,
                        "f1dilIUWtfL7DhGextUFKw=="), // encrypted setting: company_setting
                    new SettingInfo(null, null, MyEncryptedSetting,
                        "OsxLBbqIX7jiqOXo3M1DdA=="), // encrypted setting: app_setting
                    new SettingInfo(null, null, MyNotInheritedSetting, "application value"),
                };
            }

            public Task<SettingInfo> GetSettingOrNullAsync(int? companyId, long? userId, string name)
            {
                return Task.FromResult(GetSettingOrNull(companyId, userId, name));
            }

            public SettingInfo GetSettingOrNull(int? companyId, long? userId, string name)
            {
                return _settings.FirstOrDefault(s => s.CompanyId == companyId && s.UserId == userId && s.Name == name);
            }

#pragma warning disable 1998
            public Task DeleteAsync(SettingInfo setting)
            {
                Delete(setting);
                return Task.CompletedTask;
            }

            public void Delete(SettingInfo setting)
            {
                _settings.RemoveAll(s =>
                    s.CompanyId == setting.CompanyId && s.UserId == setting.UserId && s.Name == setting.Name);
            }
#pragma warning restore 1998

#pragma warning disable 1998
            public Task CreateAsync(SettingInfo setting)
            {
                Create(setting);
                return Task.CompletedTask;
            }

            public void Create(SettingInfo setting)
            {
                _settings.Add(setting);
            }
#pragma warning restore 1998

            public Task UpdateAsync(SettingInfo setting)
            {
                Update(setting);
                return Task.CompletedTask;
            }

            public void Update(SettingInfo setting)
            {
                var s = GetSettingOrNull(setting.CompanyId, setting.UserId, setting.Name);
                if (s != null)
                {
                    s.Value = setting.Value;
                }
            }

            public Task<List<SettingInfo>> GetAllListAsync(int? companyId, long? userId)
            {
                return Task.FromResult(GetAllList(companyId, userId));
            }

            public List<SettingInfo> GetAllList(int? companyId, long? userId)
            {
                var allSetting = _settings.Where(s => s.CompanyId == companyId && s.UserId == userId)
                    .Select(s => new SettingInfo(s.CompanyId, s.UserId, s.Name, s.Value)).ToList();

                //Add some undefined settings.
                allSetting.Add(new SettingInfo(null, null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                allSetting.Add(new SettingInfo(1, null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
                allSetting.Add(new SettingInfo(1, 1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

                return allSetting;
            }
        }
    }
}
