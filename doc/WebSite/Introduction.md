### ¿Qué es Kontecg?

**Kontecg** es un framework de **[código abierto]( http://172.22.16.134:8080/tfs/CSIRC/Kontecg/)** bien documentado para el desarrollo de aplicaciones webs modernas y escritorio para la empresa Cmdte. Ernesto Che Guevara. No solo es un framework, también provée un fuerte **[modelo arquitectónico](NLayer-Architecture.md)** basado en el patrón de diseño **Domain Driven Design**, con las **mejores prácticas de desarrollo** en mente.

Kontecg trabaja con las últimas versiones de **.NET Core** & **EF Core** pero también soporta versiones anteriores.

### Un ejemplo rápido

Investiguemos una simple clase para ver los beneficios de Kontecg:

    public class TareaAppService : ApplicationService, ITareaAppService
    {
        private readonly IRepository<Tarea> _tareaRepository;

        public TareaAppService(IRepository<Tarea> tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        [KontecgAuthorize(MisPermisos.ActualizarTareas)]
        public async Task ActualizarTarea(ActualizarTareaEntrada entrada)
        {
            Logger.Info("Actualizando una tarea para la entrada: " + entrada);

            var tarea = await _tareaRepository.FirstOrDefaultAsync(entrada.TareaId);
            if (tarea == null)
            {
                throw new UserFriendlyException(L("NoSePudoEncontrarTareaMensaje"));
            }

            ObjectMapper.MapTo(entrada, tarea);
        }
    }

Aquí observamos un ejemplo de [Servicio de Aplicación](Application-Services.md). Un servicio de aplicación, en DDD, es usado directamente en la capa de presentación para implementar los **casos de usos** de la aplicación. Piensa que **ActualizarTarea** es un método que es invocado mediante alguna API implementada.

Veamos algunos beneficios del framework aquí::

-   **[Inyección de Dependencias](Dependency-Injection.md)**: Kontecg usa e implementa una infraestructura convencional de inyección de dependencias. Desde que esta clase es un servicio de aplicación que implementa la clase base ApplicationService, es registrada de forma convencional en el contenedor de dependencias como transiente (creada por pedido). Puede simplemente inyectar cualquier dependencia (como el repositorio de tareas IRepository&lt;Tarea&gt; en este ejemplo).
-   **[Repositorios](Repositories.md)**: Kontecg puede crear un repositorio predeterminado para cada entidad (como IRepository&lt;Tarea&gt; en este ejemplo). El repositorio por defecto tiene muchos métodos útiles como el caso de FirstOrDefault usado en este ejemplo. Se puede extender la funcionalidad de acuerdo a nuestras necesidades. Los Repositorios abstraen el uso de DBMS y ORMs simplificando la lógica de acceso a datos.
-   **[Autorización](Authorization.md)**: Kontecg puede chequear permisos de manera declarativa. Previene el acceso al método ActualizarTarea si el usuario actual no tiene permiso a "actualizar tareas" o no está autenticado. Kontecg no solo usa atributos declarativos, si no también tiene otras vías para autorizar el uso de algún recurso.
-   **[Validación](Validating-Data-Transfer-Objects.md)**: Kontecg chequea automáticamente si la entrada es nula, también valida todas las propiedades del objeto de entrada basado en el standard de anotaciones mediante atributos y reglas de validación personalizadas. Si la petición no es válida, se lanza una excepción de validación y se controla del lado del cliente o capa de presentación.
-   **[Auditoría](Audit-Logging.md)** : Usuario, cliente, dirección IP, servicio de llamada, método, parámetros, tiempo de llamada, duración y otras informaciones son automáticamente guardadas para cada petición o llamada basada en convencionalismos y configuraciones.
-   **[Unidad de Trabajo](Unit-Of-Work.md)** : En Kontecg, cada método de un servicio de aplicación es considerado una unidad de trabajo por defecto. Automáticamente crea una conexión e inicia una transacción al principio del método. Si la ejecución del método es satisfactoria sin ninguna excepción, la transacción es completada y la conexión destruida. Incluso si este método usa diferentes repositorios o métodos, la ejecución de todos ellos será atómica (transaccional). Todos los cambios a entidades son guardados cuando la transacción es completada. No se necesita inclusive llamar el método \_repository.Update(tarea).
-   **[Manejo de Excepciones](Handling-Exceptions.md)**: Generalmente no se necesita manejar las excepciones manualmente en una aplicación web, pero en las aplicaciones de escritorio hay que implementar código adicional de acuerdo a la capa de presentación. Todas las excepciones son manejadas automáticamente por defecto, si una excepción ocurre, Kontecg registra una traza y retorna un resultado adecuado. Oculta todas las excepciones retornando que ha ocurrido un error a menos que sea una excepción de tipo UserFriendlyException, como se muestra en el ejemplo.
-   **[Trazas](Logging.md)**: Como puedes observar, podemos escribir logs o trazas usando el objeto Logger definido en la clase base. Se usa Log4Net por defecto, pero se puede cambiar y configurar.
-   **[Localización](Localization.md)**: ¿Se percata que usamos el método 'L' mientras se lanza la excepción?, de esta manera, se localizan los mensajes automáticamente de acuerdo al idioma o cultura del usuario. Véase el documento  sobre [localización](Localization.md) para más información.
-   **[Automapeo de Objetos a Objetos](Data-Transfer-Objects.md)**: En la última línea, mapeamos "entrada" usando el método MapTo de la interfaz IObjectMapper que proporciona Kontecg, propiedades a propiedades de entidades. Se usa la librería AutoMapper para realizar el mapeo. Nosotros podemos mapear fácilmente propiedades de un objeto a otro basado en convenciones de nombres.

Podemos apreciar los beneficios de Kontecg en esta simple clase. Todas estas tareas normalmente toman mucho tiempo implementarlas, pero son gestionadas automáticamente por el framework.

Además de este ejemplo, Kontecg proporciona una fuerte infraestructura y modelo de desarrollo para [modularidad](Module-System.md), [softwares como servicio (SaaS en inglés)](Multi-Company.md), [cacheo](Caching.md), [trabajos en segundo plano](Background-Jobs-And-Workers.md), [filtro de datos](Data-Filters.md), [gestión de configuración](Setting-Management.md), [eventos de dominio](EventBus-Domain-Events.md), pruebas unitarias y de integración y otras... Usted se puede enfocar en el código de su negocio y no repertirse ud mismo!