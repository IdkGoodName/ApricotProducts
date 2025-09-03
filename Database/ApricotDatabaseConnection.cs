using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Npgsql;

namespace ApricotProducts.Database;

// TODO: Database
/// <summary>
/// Represents the application's connection to the database.
/// </summary>
public sealed class ApricotDatabaseConnection : IAsyncDisposable
{
    /// <summary>
    /// Gets the connection to the database of the application.
    /// </summary>
    public NpgsqlDataSource Source { get; }

    /// <summary>
    /// Gets the string used to connect to the database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Initializes a new instance of the database with a provided <paramref name="connection">connection string</paramref>.
    /// </summary>
    /// <param name="connectionString">The string used to connect to the database</param>
    public ApricotDatabaseConnection(string connectionString) =>
        (ConnectionString, Source) = (connectionString, NpgsqlDataSource.Create(connectionString));

    /// <summary>
    /// Initializes a new instance of the database with provided parameters, but without a provided port.
    /// </summary>
    /// <param name="host">The host IP of the database</param>
    /// <param name="username">The username to connect to the database</param>
    /// <param name="password">The password to access the database</param>
    /// <param name="database">The name of the database to connect to</param>
    public ApricotDatabaseConnection(string host, string username, string password, string database) : this(CreateConnectionString(host, username, password, database))
    { }

    /// <summary>
    /// Initializes a new instance of the database with provided parameters, but without a provided port.
    /// </summary>
    /// <param name="host">The host IP of the database</param>
    /// <param name="port">The port integer of the host</param>
    /// <param name="username">The username to connect to the database</param>
    /// <param name="password">The password to access the database</param>
    /// <param name="database">The name of the database to connect to</param>
    public ApricotDatabaseConnection(string host, ushort port, string username, string password, string database) : this(CreateConnectionString(host, port, username, password, database))
    { }

    /// <summary>
    /// Disposes the database connection.
    /// </summary>
    /// <returns>Disposing content task</returns>
    public ValueTask DisposeAsync() =>
        Source.DisposeAsync();

    private static string CreateConnectionString(string host, string username, string password, string database) =>
        $"Host={host};Username={username};Password={password};Database={database}";

    private static string CreateConnectionString(string host, ushort port, string username, string password, string database) =>
        $"Host={host};Port={port};Username={username};Password={password};Database={database}";
}