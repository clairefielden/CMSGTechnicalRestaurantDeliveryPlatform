using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Tests.TestHelpers;
using CMSGTechnical.Repository;
using Microsoft.EntityFrameworkCore;
using BasketModel = CMSGTechnical.Domain.Models.Basket;

namespace CMSGTechnical.Mediator.Tests;

public class RepositoryTests
{
    [Fact]
    public void ApplicationDbContextSeedsMenuItems()
    {
        using var context = TestDbFactory.CreateContext();
        context.Database.EnsureCreated();

        var menuItems = context.Set<MenuItem>().ToList();
        var baskets = context.Set<BasketModel>().ToList();

        Assert.True(menuItems.Count >= 12);
        Assert.True(baskets.Count >= 1);
    }

    [Fact]
    public async Task RepoAddUpdateDeleteWorkflows()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var item = new MenuItem { Name = "Repo Item", Price = 1.00m };
        await repo.Add(item);

        var loaded = await repo.Get(item.Id);
        Assert.NotNull(loaded);

        loaded!.Name = "Updated";
        await repo.Update(loaded);

        var updated = await repo.Get(item.Id);
        Assert.Equal("Updated", updated!.Name);

        await repo.Delete(item.Id);
        var afterDelete = await repo.Get(item.Id);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task RepoModifyHandlesDetachedAndTrackedEntities()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var detached = new MenuItem { Name = "Detached", Price = 2.00m };
        await repo.Modify(detached);

        var tracked = new MenuItem { Name = "Tracked", Price = 3.00m };
        await repo.Add(tracked);
        tracked.Price = 4.00m;
        context.Entry(tracked).State = EntityState.Modified;

        await repo.Modify(tracked);

        var reloaded = await repo.Get(tracked.Id);
        Assert.Equal(4.00m, reloaded!.Price);
    }

    [Fact]
    public async Task RepoModifyEnumerableHandlesAddedAndModifiedStates()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var existing = new MenuItem { Name = "Existing", Price = 1.00m };
        await repo.Add(existing);

        existing.Price = 2.00m;
        context.Entry(existing).State = EntityState.Modified;

        var added = new MenuItem { Name = "Added", Price = 3.00m };
        context.Entry(added).State = EntityState.Added;

        await repo.Modify(new[] { existing, added });

        var allItems = await repo.GetAll().ToListAsync();
        Assert.Contains(allItems, item => item.Name == "Added");
    }

    [Fact]
    public async Task RepoDeleteOverloadsRemoveEntities()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var first = new MenuItem { Name = "First", Price = 1.00m };
        var second = new MenuItem { Name = "Second", Price = 2.00m };
        var third = new MenuItem { Name = "Third", Price = 3.00m };
        await repo.Add(new[] { first, second, third });

        await repo.Delete(second);
        await repo.Delete(new[] { first.Id });
        await repo.Delete(new[] { third });

        var remaining = await repo.GetAll().ToListAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task RepoGetByIdsReturnsExpectedItems()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var first = new MenuItem { Name = "First", Price = 1.00m };
        var second = new MenuItem { Name = "Second", Price = 2.00m };
        await repo.Add(new[] { first, second });

        var byIds = await repo.Get(new[] { first.Id });

        Assert.Single(byIds);
        Assert.Equal("First", byIds.First().Name);
    }

    [Fact]
    public async Task RepoQueryableMembersAreAccessible()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        await repo.Add(new MenuItem { Name = "Queryable", Price = 1.00m });

        _ = repo.ElementType;
        _ = repo.Expression;
        _ = repo.Provider;

        using var enumerator = repo.GetEnumerator();
        Assert.True(enumerator.MoveNext());
    }
}
