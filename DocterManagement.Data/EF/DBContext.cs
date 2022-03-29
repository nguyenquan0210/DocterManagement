using DocterManagement.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.EF
{
    public class DBContext : IdentityDbContext<AppUsers, AppRoles, Guid>
    {
    }
}
