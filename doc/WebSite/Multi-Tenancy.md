### ¿Qué es Multi-Company?

"*Software* ***Multitenancy*** *refers to a software* ***architecture***
*in which a* ***single instance*** *of a software runs on a server and
serves ***multiple companys***. A company is a group of users who share
a common access with specific privileges to the software instance. With
a multicompany architecture, a software application is designed to
provide every company a ***dedicated share of the instance including its
data***, configuration, user management, company individual
functionality and non-functional properties. Multitenancy contrasts with
multi-instance architectures, where separate software instances operate
on behalf of different companys*"
([Wikipedia](https://en.wikipedia.org/wiki/Multitenancy))

In short, multi-company is a technique that is used to create **SaaS**
(Software as-a Service) applications.

#### Database & Deployment Architectures

There are some different multi-company database & deployment approaches:

##### Multiple Deployment - Multiple Database

This is **not multi-company** actually, but if we run **one instance**
of the application **for each** customer (company) with a **separated
database**, we can serve **multiple companys** on a single server. We
just have to make sure that multiple instances of the application don't
**conflict** with each other on the same server environment.

This can also be possible for an **existing application** which is not
designed as multi-company. It's easier to create such an application since
the application is not aware of multitenancy. There are, however, setup,
utilization and maintenance problems in this approach.

##### Single Deployment - Multiple Database

ln this approach, we run a **single instance** of the application on
a server. We have a **master** (host) database to store company metadata
(like company name and subdomain) and a **separate database** for each
company. Once we identify the **current company** (for example; from
subdomain or from a user login form), then we can **switch** to that
company's database to perform operations.

In this approach, the application should be designed as multi-company at some
level, but most of the application can remain independent from it.

We create and maintain a **separate database** for each company,
this includes **database migrations**. If we have many customers with
dedicated databases, it may take a long time to migrate the database schema during
an application update. Since we have a separated database for each company, we
can **backup** its database separately from other companys. We can also
**move** the company database to a stronger server if that company needs
it.

##### Single Deployment - Single Database

This is the **most ideal multi-company** architecture: We only deploy a
**single instance** of the application with a **single database** on to a
**single server**. We have a **CompanyId** (or similar) field in each
table (for a RDBMS) which is used to isolate a company's data from
others.

This type of application is easy to setup and maintain, but harder to create.
This is because we must prevent a Company from reading or writing to other
company data. We may add a **CompanyId filter** for each database read
(select) operation. We may also check it every time we write, to see if this entity is
related to the **current company**. This is tedious and error-prone. However,
Kontecg platform helps us here by using **automatic [data
filtering](Data-Filters.md)**.

This approach may have performance problems if we have many companys with
large data sets. We can use table partitioning or other database features to
overcome this problem.

##### Single Deployment - Hybrid Databases

We may want to store companys in a single databases normally, but may want to
create a separate database for desired companys. For example, we can
store companys with big data in their own databases, but store all other
companys in a single database.

##### Multiple Deployment - Single/Multiple/Hybrid Database

Finally, we may want to deploy our application to more than one server
(like web farms) for better application performance, high
availability, and/or scalability. This is independent from the database
approach.

### Multi-Company in Kontecg platform

Kontecg platform can work with all the scenarios described above.

#### Enabling Multi-Company

Multi-tenancy is disabled by default for Framework level. We can enable it in PreInitialize method
of our module as shown below:

    Configuration.MultiCompany.IsEnabled = true; 

**Note:** Multi-company is enabled in Kontecg baseline platform.

#### Ignore Feature Check For Host Users

There is another configuration to ignore feature check for host users. We can enable it in PreInitialize method
of our module as shown below:

    Configuration.MultiCompany.IgnoreFeatureCheckForHostUsers = true; 

**Note:** `IgnoreFeatureCheckForHostUsers` default value is `false`;

#### Host vs Company

We define two terms used in a multi-company system:

-   **Company**: A customer which has its own users, roles,
    permissions, settings... and uses the application completely
    isolated from other companys. A multi-company application will have
    one or more companys. If this is a CRM application, different companys
    also have their own accounts, contacts, products and orders. So
    when we say a '**company user**', we mean a user owned by a company.
-   **Host**: The Host is singleton (there is a single host). The Host is
    responsible for creating and managing companys. A '**host user**' is at a
    higher level and independent from all companys and can control them.

#### Session

Kontecg platform defines the **IKontecgSession** interface to obtain the current
**user** and **company** ids. This interface is used in multi-company to
get the current company's id by default. Thus, it can filter data based on the
current company's id. Here are the rules:

-   If both the UserId and the CompanyId is null, then the current user is **not
    logged in** to the system. We can not know if it's a host user
    or company user. In this case, the user can not access
    [authorized](/Pages/Documents/Authorization) content.
-   If the UserId is not null and the CompanyId is null, then we know that
    the current user is a **host user**.
-   If the UserId is not null and the CompanyId is not null, we know
    that the current user is a **company user**.
-   If the UserId is null but the CompanyId is not null, that means we know
    the current company, but the current request is not authorized (user did
    not login). See the next section to understand how the current company is
    determined.

See the [session documentation](/Pages/Documents/Kontecg-Session) for more
information.

#### Determining Current Company

Since all company users use the same application, we should have a way of
distinguishing the company of the current request. The default session
implementation (ClaimsKontecgSession) uses different approaches to find the
company related to the current request in this given order:

1.  If the user is logged in, it gets the CompanyId from current claims. Claim
    name is *http://www.aspnetboilerplate.com/identity/claims/companyId*
    and should contain an integer value. If it's not found in claims
    then the user is assumed to be a *host* user.
2.  If the user has not logged in, then it tries to resolve the CompanyId from the
    *company resolve contributor*s. There are 3 pre-defined company
    contributors and are run in a given order (first successful resolver 'wins'):
    1.  **DomainCompanyResolveContributor**: Tries to resolve tenancy
        name from an url, generally from a domain or subdomain. You can
        configure the domain format in the PreInitialize method of your module
        (like
        Configuration.Modules.KontecgWebCommon().MultiCompany.DomainFormat =
        "{0}.mydomain.com";). If the domain format is "{0}.mydomain.com" and
        the current host of the request is ***acme*.mydomain.com**, then the
        tenancy name is resolved as "acme". The next step is to query
        ICompanyStore to find the CompanyId by the given tenancy name. If a
        company is found, then it's resolved as the current CompanyId.
    2.  **HttpHeaderCompanyResolveContributor**: Tries to resolve
        CompanyId from an "Kontecg.CompanyId" header value, if present. This is a
        constant defined in
        Kontecg.MultiCompany.MultiCompanyConsts.CompanyIdResolveKey.
    3.  **HttpCookieCompanyResolveContributor**: Tries to resolve
        the CompanyId from an "Kontecg.CompanyId" cookie value, if present. This uses the
        same constant explained above.

By default, Kontecg platform uses "Kontecg.CompanyId" to find CompanyId from Cookie or Request Headers. You can change it using multi-company configuration:

````c#
Configuration.MultiCompany.CompanyIdResolveKey = "Kontecg-CompanyId";
````

You also need to configure it on the client side:

````js
kontecg.multiCompany.companyIdCookieName = 'Kontecg-CompanyId';
````

If none of these attempts can resolve a CompanyId, then the current requester
is considered to be the host. Company resolvers are extensible. You can add
resolvers to the **Configuration.MultiCompany.Resolvers** collection, or
remove an existing resolver.

One last thing on resolvers: The resolved company id is cached during
the same request for performance reasons. Resolvers are executed
once in a request, and only if the current user has not already logged in.

##### Company Store

The **DomainCompanyResolveContributor** uses ICompanyStore to find the company id
by tenancy name. The default implementation of **ICompanyStore** is
**NullCompanyStore** which does not contain any company and returns null
for queries. You can implement and replace it to query companys from any
data source. [Module Zero](Zero/Overall.md) properly implements it by
getting it from its [company manager](Zero/Company-Management.md). So if you
are using Module Zero, you don't need to worry about the company store.

#### Data Filters

For the **multi-company single database** approach, we must add a
**CompanyId** filter to only get the current company's entities when
retrieving [entities](/Pages/Documents/Entities) from the database. ASP.NET
Boilerplate automatically does it when you implement one of the two
interfaces for your entity: **IMustHaveCompany** and **IMayHaveCompany**.

##### IMustHaveCompany Interface

This interface is used to distinguish the entities of different companys by
defining a **CompanyId** property. An example entity that implements
IMustHaveCompany:

    public class Product : Entity, IMustHaveCompany
    {
        public int CompanyId { get; set; }

        public string Name { get; set; }

        //...other properties
    }

This way, Kontecg platform knows that this is a company-specific entity
and automatically isolates the entities of a company from other companys.

##### IMayHaveCompany interface

We may need to share an **entity type** between host and companys. As such, an
entity may be owned by a company or the host. The IMayHaveCompany interface
also defines **CompanyId** (similar to IMustHaveCompany), but it is **nullable**
in this case. An example entity that implements IMayHaveCompany:

    public class Role : Entity, IMayHaveCompany
    {
        public int? CompanyId { get; set; }

        public string RoleName { get; set; }

        //...other properties
    }

We may use the same Role class to store Host roles and Company roles. In this
case, the CompanyId property says if this is  host entity or company
entitiy. A **null** value means this is a **host** entity, a
**non-null** value means this entity is owned by a **company** where the Id is
the **CompanyId**.

##### Additional Notes

IMayHaveCompany is not as common as IMustHaveCompany. For example, a Product
class can not be IMayHaveCompany since a Product is related to the actual
application functionality, and not related to managing companys. So use
the IMayHaveCompany interface carefully since it's harder to maintain code
shared by host and companys.

When you define an entity type as IMustHaveCompany or IMayHaveCompany,
**always set the CompanyId** when you create a new entity (While ASP.NET
Boilerplate tries to set it from current CompanyId, it may not be
possible in some cases, especially for IMayHaveCompany entities). Most of
the time, this will be the only point you deal with the CompanyId properties.
You don't need to explicitly write the CompanyId filter in Where conditions
while writing LINQ, since it is automatically filtered.

#### Switching Between Host and Companys

While working on a multi-company application database, we can get the
**current company**. By default, it's obtained from the
[IKontecgSession](Kontecg-Session.md) (as described before). We can change
this behavior and switch to another company's database. Example:

    public class ProductService : ITransientDependency
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProductService(IRepository<Product> productRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _productRepository = productRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual List<Product> GetProducts(int companyId)
        {
            using (_unitOfWorkManager.Current.SetCompanyId(companyId))
            {
                return _productRepository.GetAllList();
            }
        }
    }

SetCompanyId ensures that we are working on a given company's data,
independent from the database architecture:

-   If the given company has a dedicated database, it switches to that
    database and gets products from it.
-   If the given company does not have a dedicated database (the single database
    approach, for example), it adds the automatic CompanyId filter to query
    only that company's products.

If we don't use SetCompanyId, it gets the companyId from the
[session](Kontecg-Session.md). There are some guidelines and
best practices here:

-   Use **SetCompanyId(null)** to switch to the host.
-   Use SetCompanyId within a **using** block (as in the example) if there
    is not a special case. This way, it automatically restores the companyId at
    the end of the using block and the code calling the GetProducts method
    works as before.
-   You can use SetCompanyId in **nested blocks** if it's needed.
-   Since **\_unitOfWorkManager.Current** is only available in a [unit of
    work](Unit-Of-Work.md), be sure that your code runs in a UOW.
