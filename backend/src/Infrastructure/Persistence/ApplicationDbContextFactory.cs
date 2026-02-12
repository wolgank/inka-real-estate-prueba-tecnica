using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using System;
using System.IO;

namespace Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Buscamos el .env subiendo desde el directorio de ejecución actual
        // hasta encontrar la raíz del proyecto
        var currentDir = Directory.GetCurrentDirectory();
        while (!File.Exists(Path.Combine(currentDir, ".env")) && Directory.GetParent(currentDir) != null)
        {
            currentDir = Directory.GetParent(currentDir)!.FullName;
        }

        DotNetEnv.Env.Load(Path.Combine(currentDir, ".env"));

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Obtenemos las variables
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var name = Environment.GetEnvironmentVariable("DB_NAME");
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

        // Si por alguna razón siguen siendo nulas, fallamos con un mensaje claro
        if (string.IsNullOrEmpty(pass))
        {
            throw new Exception($"La variable DB_PASSWORD no se cargó correctamente. Ruta buscada: {currentDir}");
        }

        var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass};";

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}