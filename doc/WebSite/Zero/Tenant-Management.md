### About Multi-Company

We strongly recommend that you read the [multi-company
documentation](/Pages/Documents/Multi-Company) before this one.

### Enabling Multi-Company

Kontecg platform and Module Zero can run in **multi-company** or
**single-company** modes. Multi-tenancy is disabled by default. We can
enable it in the PreInitialize method of our
[module](/Pages/Documents/Module-System) as shown below:

    [DependsOn(typeof(KontecgZeroCoreModule))]
    public class MyCoreModule : KontecgModule
    {
        public override void PreInitialize()
        {
            Configuration.MultiCompany.IsEnabled = true;    
        }

        ...
    }

Note: Even if our application is not multi-company, we must define a
default company (see Default Company section of this document). 

When we create a project [template](/Templates) based on ASP.NET
Boilerplate and Module Zero, we have the **Company** entity and the
**CompanyManager** domain service.

### Company Entity

The Company entity represents a Company of the application.

    public class Company : KontecgCompany<Company, User>
    {

    }

It's derived from a generic **KontecgCompany** class. Company entities are
stored in the **KontecgCompanys** table in the database. You can add your own custom
properties to the Company class.

The KontecgCompany class defines some base properties, the most important are:

-   **CompanyName**: This is the **unique** name of a company in the
    application. It should not normally be changed. It can be used to
    allocate subdomains to companys like '**mycompany**.mydomain.com'.
    As such, it cannot contain spaces.
    Company.**CompanyNameRegex** constant defines the naming rule.
-   **Name**: An arbitrary, human-readable, long name of the company.
-   **IsActive**: True, if this company can use the application. If it's
    false, no user of this company can login to the application.

The KontecgCompany class inherits **FullAuditedEntity**. This means it
has creation, modification and deletion **audit properties**. It is also
**[Soft-Delete](/Pages/Documents/Data-Filters#isoftdelete)**, so
when we delete a company, it's not deleted from the database, just marked as
deleted.

Finally, the **Id** of KontecgCompany is defined as an **int**.

### Company Manager

The Company Manager is a service to perform the **domain logic** for companys:

    public class CompanyManager : KontecgCompanyManager<Company, Role, User>
    {
        public CompanyManager(IRepository<Company> companyRepository)
            : base(companyRepository)
        {

        }
    }

The CompanyManager is also used to manage the company
[features](/Pages/Documents/Feature-Management). You can add your own
methods here. You can also override any method of the KontecgCompanyManager base
class for your own needs.

### Default Company

Kontecg platform and Module Zero assume that there is a pre-defined
company where the CompanyName is '**Default**' and the Id is **1**. In a
single-company application, this is used as the only company. In a
multi-company application, you can delete it or make it passive.
