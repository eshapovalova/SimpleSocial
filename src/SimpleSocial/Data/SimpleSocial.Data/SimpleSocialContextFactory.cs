﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;


namespace SimpleSocial.Data
{
    public class SimpleSocialContextFactory : IDesignTimeDbContextFactory<SimpleSocialContext>
    {
        public SimpleSocialContext CreateDbContext(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#else
                .AddJsonFile("appsettings.Production.json", optional: false, reloadOnChange: true)
#endif
                .Build();

            var builder = new DbContextOptionsBuilder<SimpleSocialContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            // Stop client query evaluation
            builder.ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning));

            return new SimpleSocialContext(builder.Options);
        }

    }
}