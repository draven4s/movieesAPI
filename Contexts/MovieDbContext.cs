using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
namespace projektas.Context
{
    public class MovieDbContext : DbContext
    {

        private const string connectionString = "server=localhost;port=3306;database=moviesdb;uid=root;password=LUL";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new System.Version(8, 0, 22)));
        }
        public virtual DbSet<Movies> movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

        }

    }

}
