using CMSGTechnical.Repository;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Tests.TestHelpers;

public static class TestDbFactory
{
    public static ApplicationDbContext CreateContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
