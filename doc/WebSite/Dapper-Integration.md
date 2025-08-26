### Introduction

[Dapper](https://github.com/StackExchange/Dapper) is an
object-relational mapper (ORM) for .NET.
The [Kontecg.Dapper](https://www.nuget.org/packages/Kontecg.Dapper) package simply
integrates Dapper to Kontecg platform. It works as a secondary ORM
provider along with EF Core or NHibernate.

### Installation

Before you start, you need to install
[Kontecg.Dapper](https://www.nuget.org/packages/Kontecg.Dapper) and either EF
Core or the NHibernate ORM NuGet packages in to the project you want
to use.

#### Module Registration

First you need to add the **DependsOn** attribute for the **KontecgDapperModule** on
your module where you register it:

    [DependsOn(
         typeof(KontecgEntityFrameworkCoreModule),
         typeof(KontecgDapperModule)
    )]
    public class MyModule : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SampleApplicationModule).GetAssembly());
        }
    }

**Note** that the KontecgDapperModule dependency should be added later than the EF
Core dependency.

#### Entity to Table Mapping

You can configure mappings. For example, the **Person** class maps to the
**Persons** table in the following example:

    public class PersonMapper : ClassMapper<Person>
    {
        public PersonMapper()
        {
            Table("Persons");
            Map(x => x.Roles).Ignore();
            AutoMap();
        }
    }

You should set the assemblies that contain mapper classes. Example:

    [DependsOn(
         typeof(KontecgEntityFrameworkModule),
         typeof(KontecgDapperModule)
    )]
    public class MyModule : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SampleApplicationModule).GetAssembly());
            DapperExtensions.SetMappingAssemblies(new List<Assembly> { typeof(MyModule).GetAssembly() });
        }
    }
                
### Usage

After registering **KontecgDapperModule**, you can use the Generic
IDapperRepository interface (instead of standard IRepository) to inject
dapper repositories.

    public class SomeApplicationService : ITransientDependency
    {
        private readonly IDapperRepository<Person> _personDapperRepository;
        private readonly IRepository<Person> _personRepository;

        public SomeApplicationService(
            IRepository<Person> personRepository,
            IDapperRepository<Person> personDapperRepository)
        {
            _personRepository = personRepository;
            _personDapperRepository = personDapperRepository;
        }

        public void DoSomeStuff()
        {
            var people = _personDapperRepository.Query("select * from Persons");
        }
    }

You can use both EF and Dapper repositories at the same
time and in the same transaction!
