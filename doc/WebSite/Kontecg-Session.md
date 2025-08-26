### Introducción

Kontecg provée una interfaz **IKontecgSession** para obtener el usuario y empresa activa **sin** usar la Session de ASP.NET. IKontecgSession está completamente integrado y usado por otras estructuras en Kontecg como el [sistema de configuración](Setting-Management.md) y [autorización](Authorization.md).

### Inyectando la sesión

IKontecgSession es generalmente **[inyectado a través de propiedades](/Pages/Documents/Dependency-Injection#property-injection-pattern)** en las clases a menos que no sea posible trabajar sin la información de la sesión. Si usamos la inyección de propiedades, podemos usar **NullKontecgSession.Instance** como valor de inicialización prederterminado, como se muestra debajo:

    public class MiClase : ITransientDependency
    {
        public IKontecgSession KontecgSession { get; set; }

        public MiClase()
        {
            KontecgSession = KontecgSession = NullKontecgSession.Instance;
        }

        public void MiMetodo()
        {
            var currentUserId = KontecgSession.UserId;
            //...
        }
    }

Desde que la autenticación/autorización es una tarea de la capa de aplicación, es aconsejable **usar IKontecgSession en la capa de aplicación y capas superiores**. 
Esto no es generalmente usado en la capa de dominio. **ApplicationService**, 
**KontecgForm,** **KontecgRibbonForm,** **KontecgUserControl,** y otras clases bases tienen inyectado **KontecgSession**, por lo que puedes, por instancia, usar la propiedad KontecgSession directamente en un método de algún servicio de la aplicación.

### Propiedades de la sesión

KontecgSession define algunas propiedades claves:

-   **UserId**: Id del usuario actual o null si no existe ninguno. No puede ser null si el código de llamada está autorizado.
-   **CompanyId**: Id de la empresa activa o null si no hay una empresa activa (en caso de que el usuario no se ha logueado o es un usuario host).
-   **MultiCompanySide**: Puede ser Host o Company.

UserId y CompanyId son del tipo **nullable** que pueden ser nulos. Contiene los métodos **GetUserId()** y **GetCompanyId()** que no retornan valores nulos. Si está seguro que el usuario está logueado, puede llamar el método GetUserId(). Si el usuario actual es un valor nulo, este método lanza una excepción. El método GetCompanyId() trabaja de la misma manera.

**ClaimsKontecgSession**

ClaimsKontecgSession es la **implementación por defecto** de la interfaz IKontecgSession. Obtiene las propiedades de la sesión (exceptuando MultiCompanySide, que es calculada) de los atributos del objeto principal de usuario. Para una autenticación basada en cookies, las propiedades son actualizadas desde la cookie y está completamente integrado al mecanismo de autenticación de ASP.NET.

### Sobreescribiendo Valores de Sesión Actuales

En algunos casos, se necesita cambiar/sobreescribir los valores de sesión para un ambiente limitado. En estos casos puede usar el método IKontecgSession.Use como se muestra debajo:

    public class MiServicio
    {
        private readonly IKontecgSession _session;

        public MiServicio(IKontecgSession session)
        {
            _session = session;
        }

        public void Metodo()
        {
            using (_session.Use(42, null))
            {
                var companyId = _session.CompanyId; //42
                var userId = _session.UserId; //null
            }
        }
    }

El método Use retorna una instancia IDisposable y tiene que **ser destruido** (**disposed** en inglés). Una vez que valor devuelto es destruido, los valores de sesión son **restaurados automáticamente** a los anteriores.

#### Advertencia!

Siempre usa el método IKontecgSession.Use en un bloque **using** como se mostró anteriormente, de otra manera, se pueden obtener valores inesperados. Usted puede anidar bloques de código usando el método Use y trabajará como se espera.

### Identificador de usuario

Ud puede usar el método de extensión **.ToUserIdentifier()** para crear una instancia del tipo UserIdentifier desde IKontecgSession. Desde que UserIdentifier es usado en muchas interfaces programables de aplicación (APIs en inglés), este método simplifica la creación de un objeto UserIdentifier para el usuario actual.
