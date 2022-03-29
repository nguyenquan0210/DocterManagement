using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.EF
{
    public class DoctorManageDbContextFactory : IDesignTimeDbContextFactory<DoctorManageDbContext>
    {
        public DoctorManageDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DoctorManagementDb");

            var optionsBuilder = new DbContextOptionsBuilder<DoctorManageDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DoctorManageDbContext(optionsBuilder.Options);
        }
    }
}
