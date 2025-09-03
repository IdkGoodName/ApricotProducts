using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Npgsql;

namespace ApricotProducts.Database;

// TODO: Database
public class ApricotDatabaseConnection : IAsyncDisposable
{
    public NpgsqlDataSource Source { get; }

    public string ConnectionString { get; }

    public ApricotDatabaseConnection(string connectionString) =>
        (ConnectionString, Source) = (connectionString, NpgsqlDataSource.Create(connectionString));

    public ApricotDatabaseConnection(string host, string username, string password, string database) : this(CreateConnectionString(host, username, password, database))
    { }

    public ApricotDatabaseConnection(string host, ushort port, string username, string password, string database) : this(CreateConnectionString(host, port, username, password, database))
    { }

    public ValueTask DisposeAsync() =>
        Source.DisposeAsync();

    private static string CreateConnectionString(string host, string username, string password, string database) =>
        $"Host={host};Username={username};Password={password};Database={database}";

    private static string CreateConnectionString(string host, ushort port, string username, string password, string database) =>
        $"Host={host};Port={port};Username={username};Password={password};Database={database}";
}