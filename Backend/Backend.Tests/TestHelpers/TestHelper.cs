using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Tests.TestHelpers
{
    public static class TestHelper
    {
        public static NeonTechDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<NeonTechDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Cada prueba tiene su BD única
                .Options;

            var context = new NeonTechDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }


}
