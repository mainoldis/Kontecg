using System;
using Medo;

namespace Kontecg
{
    /// <summary>
    /// Implements <see cref="IGuidGenerator"/> using UUID v7 (RFC 4122) for generating sequential, 
    /// time-ordered globally unique identifiers that are optimized for database performance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// UuidGenerator provides a modern implementation of GUID generation using the UUID v7 standard,
    /// which offers better performance characteristics compared to traditional random GUIDs.
    /// UUID v7 combines a timestamp with random data to create identifiers that are:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Time-ordered for better database indexing performance</description></item>
    /// <item><description>Globally unique across distributed systems</description></item>
    /// <item><description>Cryptographically secure with random components</description></item>
    /// <item><description>Compatible with existing GUID/UUID standards</description></item>
    /// </list>
    /// <para>
    /// This implementation uses the Medo library for UUID v7 generation and provides
    /// database-specific optimizations for different database engines.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> This class is the recommended replacement for <see cref="SequentialGuidGenerator"/>
    /// due to its superior performance and compliance with modern UUID standards.
    /// </para>
    /// </remarks>
    public sealed class UuidGenerator : IGuidGenerator
    {
        /// <summary>
        /// Specifies the database type for GUID generation optimization.
        /// </summary>
        /// <remarks>
        /// Different database engines have different GUID storage and indexing characteristics.
        /// This enumeration allows the generator to optimize the GUID format for the target database:
        /// <list type="bullet">
        /// <item><description><strong>SqlServer:</strong> Uses MS SQL Server optimized format for better performance</description></item>
        /// <item><description><strong>Oracle:</strong> Uses standard UUID v7 format</description></item>
        /// <item><description><strong>MySql:</strong> Uses standard UUID v7 format</description></item>
        /// <item><description><strong>PostgresSql:</strong> Uses standard UUID v7 format</description></item>
        /// </list>
        /// </remarks>
        public enum UuidDatabaseType
        {
            /// <summary>
            /// Microsoft SQL Server database type.
            /// Uses MS SQL Server optimized UUID format for improved indexing performance.
            /// </summary>
            SqlServer,

            /// <summary>
            /// Oracle database type.
            /// Uses standard UUID v7 format compatible with Oracle's GUID storage.
            /// </summary>
            Oracle,

            /// <summary>
            /// MySQL database type.
            /// Uses standard UUID v7 format compatible with MySQL's GUID storage.
            /// </summary>
            MySql,

            /// <summary>
            /// PostgreSQL database type.
            /// Uses standard UUID v7 format compatible with PostgreSQL's GUID storage.
            /// </summary>
            PostgresSql
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="UuidGenerator"/> class from being created.
        /// Use <see cref="Instance"/> to access the singleton instance.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the generator with SQL Server as the default database type.
        /// The singleton pattern ensures consistent GUID generation across the application.
        /// </remarks>
        private UuidGenerator()
        {
            DatabaseType = UuidDatabaseType.SqlServer;
        }

        /// <summary>
        /// Gets the singleton <see cref="UuidGenerator"/> instance.
        /// </summary>
        /// <value>
        /// The singleton instance of the UuidGenerator.
        /// </value>
        /// <remarks>
        /// This property provides access to the singleton instance of the UuidGenerator.
        /// The singleton pattern ensures that all GUID generation uses the same configuration
        /// and maintains consistency across the application.
        /// </remarks>
        public static UuidGenerator Instance { get; } = new();

        /// <summary>
        /// Gets or sets the database type for GUID generation optimization.
        /// </summary>
        /// <value>
        /// The database type that determines the GUID format optimization.
        /// Defaults to <see cref="UuidDatabaseType.SqlServer"/>.
        /// </value>
        /// <remarks>
        /// This property controls how GUIDs are generated to optimize performance for the target database.
        /// Changing this property affects all subsequent GUID generation calls.
        /// </remarks>
        public UuidDatabaseType DatabaseType { get; set; }

        /// <summary>
        /// Creates a new GUID using the current database type configuration.
        /// </summary>
        /// <returns>
        /// A new <see cref="Guid"/> that is optimized for the configured database type.
        /// </returns>
        /// <remarks>
        /// This method generates a UUID v7 GUID using the current <see cref="DatabaseType"/> setting.
        /// The generated GUID will be time-ordered and optimized for the specified database engine.
        /// </remarks>
        public Guid Create()
        {
            return Create(DatabaseType);
        }

        /// <summary>
        /// Creates a new GUID optimized for the specified database type.
        /// </summary>
        /// <param name="databaseType">
        /// The database type that determines the GUID format optimization.
        /// </param>
        /// <returns>
        /// A new <see cref="Guid"/> that is optimized for the specified database type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an unsupported database type is specified.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method generates a UUID v7 GUID optimized for the specified database engine:
        /// </para>
        /// <list type="bullet">
        /// <item><description><strong>SqlServer:</strong> Uses <see cref="Uuid7.NewMsSqlUniqueIdentifier()"/> for MS SQL Server optimization</description></item>
        /// <item><description><strong>Other databases:</strong> Uses <see cref="Uuid7.NewUuid7()"/> for standard UUID v7 format</description></item>
        /// </list>
        /// <para>
        /// The generated GUIDs are time-ordered, which provides better performance for database
        /// indexing and clustering operations compared to random GUIDs.
        /// </para>
        /// </remarks>
        public Guid Create(UuidDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case UuidDatabaseType.SqlServer:
                    return Uuid7.NewMsSqlUniqueIdentifier();
                case UuidDatabaseType.Oracle:
                case UuidDatabaseType.MySql:
                case UuidDatabaseType.PostgresSql:
                    return Uuid7.NewUuid7();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
