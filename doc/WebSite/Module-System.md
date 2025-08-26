### Introducción

Kontecg proporciona una infraestructura para crear módulos y combinarlos para crear una aplicación. Un módulo puede depender de otro. Generalmente, un ensamblado es considerado un módulo. Si creas una aplicación con más de un módulo, es recomendable crear una definición de módulo para cada uno de ellos.

### Definición de un módulo

Un módulo es definido en una clase derivada de **KontecgModule** que está en el **paquete Kontecg**. Dicho esto estaremos desarrollando un módulo que puede ser usado por varias aplicaciónes. La definición más simple sería como se muestra debajo:

    public class MaestroModule : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

La clase que define el módulo es responsable de registrar las demás clases del ensamblado a través de la [inyección de dependencias](Dependency-Injection.md), si se necesita (puede realizarse de manera convencional como se mostró), además puedes configurar la aplicación y otros módulos, agregar nuevas funcionalidades a la aplicación y mucho más...

### Ciclo de vida

Kontecg invoca algunos métodos específicos cuando una aplicación inicia y termina. Ud puede sobreescribir la implementación de estos métodos para realizar tareas concretas.

Kontecg invoca estos métodos de manera **ordenada por dependencias**. Si un módulo A depende de un módulo B, el módulo B se inicializa primero que el módulo A.

El órden exacto del inicio de estos métodos: PreInitialize-B, PreInitialize-A,
Initialize-B, Initialize-A, PostInitialize-B y PostInitialize-A. Esto se cumple para todo el grafo de dependencias. El método **shutdown** es algo similar, pero en **órden inverso**.

#### PreInitialize

Este método es el primero en invocar, cuando inicia la aplicación. Es un método para [configurar](Startup-Configuration.md) el framework y otros módulos antes de que ellos se inicien.

Ud puede también escribir código específico en este método para ejecutarlo antes de que se registren las dependencias de código. Por ejemplo, si creas una clase para [registro convencional](Dependency-Injection.md), debería usar el método IocManager.AddConventionalRegisterer.

#### Initialize

Este es el lugar donde el [registro de dependencias](Dependency-Injection.md) debe efectuarse. Generalmente se lleva a cabo usando el método **IocManager.RegisterAssemblyByConvention**. Si se desea definir registros de dependencias personalizados, véa la documentación de [inyección de dependencias](Dependency-Injection.md).

#### PostInitialize

Este método es invocado al final del proceso de inicialización, Es seguro resolver alguna dependencia aquí.

#### Shutdown

El método es invocado cuando la aplicación finaliza.

### Dependencias entre módulos

Un módulo puede depender de otro. Se necesita declarar **explicítamente** una dependencia usando el atributo **DependsOn**, como se muestra debajo:

    [DependsOn(typeof(MaestroCoreModule))]
    public class MaestroApplicationModule : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

Aquí, declaramos para el framework que el módulo MaestroApplicationModule depende del módulo MaestroCoreModule y debería inicializarse antes que MaestroApplicationModule.

Kontecg puede resolver las dependencias recursivamente al inicio del módulo principal e inicializarlo en consecuencia. El módulo principal se inicializa como el último módulo.

### Extensiones de la aplicación (PlugIns)

Mientras que los módulos son examinados al inicio del módulo primario a través de sus dependencias, Kontecg puede también cargar módulos **dinámicamente**.
La clase **KontecgBootstrapper** define la propiedad **PlugInSources** la cúal es usada para agregar [extensiones de la aplicación](Plugin.md) para cargarlas dinámicamente. Un proveedor de extensiones o plugins puede ser cualquier clase que implemente la interfaz **IPlugInSource**. La clase **PlugInFolderSource** implementa la carga de plugins desde un directorio.

### Ensamblados adicionales

La implementación por defecto de IAssemblyFinder y ITypeFinder (la cual es usada por Kontecg para investigar clases específicas en la aplicación) solo busca módulos y tipos en esos ensamblados. Podemos sobreescribir el método **GetAdditionalAssemblies** en nuestro módulo para incluir ensamblados adicionales.

### Métodos personalizados

Los módulos pueden declarar otros métodos que pueden ser usados por otros módulos que dependan de este. Asume que Modulo2 depende del Modulo1 y se requiere invocar un método de Modulo1 en el método PreInitialize.

    public class Modulo1 : KontecgModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public void Modulo1Metodo1()
        {
            //Aquí va el código de este método
        }
    }

    [DependsOn(typeof(Modulo1))]
    public class Modulo2 : KontecgModule
    {
        private readonly Modulo1 _modulo1;

        public Modulo2(Modulo1 modulo1)
        {
            _modulo1 = modulo1;
        }

        public override void PreInitialize()
        {
            _modulo1.Modulo1Metodo1(); //Llamada al método del módulo Modulo1
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

Aquí se injecta Modulo1 en el módulo Modulo2 a través de su constructor, lo que permite incovar métodos de Modulo1. Esto es solo posible si Modulo2 depende de Modulo1.

### Configuración del módulo

Mientras que los métodos personalizados declarados en un módulo pueden usarse para configurar el módulo, se sugiere usar el [sistema de configuración inicial](Startup-Configuration.md) para definir y configurar los módulos.

### Ciclo de vida de un módulo

Las clases de módulos son registradas automáticamente como **singleton**.
