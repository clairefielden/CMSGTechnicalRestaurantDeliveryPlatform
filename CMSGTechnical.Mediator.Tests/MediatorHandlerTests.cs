using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Menu;
using CMSGTechnical.Mediator.Tests.TestHelpers;
using CMSGTechnical.Repository;
using Microsoft.EntityFrameworkCore;
using BasketModel = CMSGTechnical.Domain.Models.Basket;

namespace CMSGTechnical.Mediator.Tests;

public class MediatorHandlerTests
{
    [Fact]
    public async Task GetMenuItemsReturnsDtos()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);
        await repo.Add(new MenuItem { Name = "Item A", Price = 3.25m });

        var handler = new GetMenuItemsHandler(repo);
        var result = await handler.Handle(new GetMenuItems(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Item A", result.First().Name);
    }

    [Fact]
    public async Task GetMenuItemReturnsNullWhenMissing()
    {
        using var context = TestDbFactory.CreateContext();
        var repo = new Repo<MenuItem>(context);

        var handler = new GetMenuItemHandler(repo);
        var result = await handler.Handle(new GetMenuItem(99), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBasketReturnsBasketWithItems()
    {
        using var context = TestDbFactory.CreateContext();
        var basket = new BasketModel { UserId = 1 };
        var menuItem = new MenuItem { Id = 1, Name = "Item A", Price = 2.00m };
        basket.Items.Add(new BasketItem { MenuItem = menuItem, MenuItemId = menuItem.Id, Quantity = 1 });
        context.AddRange(basket, menuItem);
        await context.SaveChangesAsync();

        var repo = new Repo<BasketModel>(context);
        var handler = new GetBasketHandler(repo);
        var result = await handler.Handle(new GetBasket(basket.Id), CancellationToken.None);

        Assert.Single(result.MenuItems);
    }

    [Fact]
    public async Task AddItemToBasketAddsMenuItem()
    {
        using var context = TestDbFactory.CreateContext();
        var basket = new BasketModel { UserId = 1 };
        var menuItem = new MenuItem { Name = "Item A", Price = 2.00m };
        context.AddRange(basket, menuItem);
        await context.SaveChangesAsync();

        var basketRepo = new Repo<BasketModel>(context);
        var menuRepo = new Repo<MenuItem>(context);
        var handler = new AddItemToBasketHandler(basketRepo, menuRepo);

        var result = await handler.Handle(new AddItemToBasket(basket.Id, menuItem.Id), CancellationToken.None);

        Assert.Single(result.MenuItems);
        Assert.Equal(menuItem.Id, result.MenuItems.First().Id);
    }

    [Fact]
    public async Task AddItemToBasketThrowsWhenMissingMenuItem()
    {
        using var context = TestDbFactory.CreateContext();
        var basket = new BasketModel { UserId = 1 };
        context.Add(basket);
        await context.SaveChangesAsync();

        var basketRepo = new Repo<BasketModel>(context);
        var menuRepo = new Repo<MenuItem>(context);
        var handler = new AddItemToBasketHandler(basketRepo, menuRepo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await handler.Handle(new AddItemToBasket(basket.Id, 123), CancellationToken.None));

        Assert.Contains("Menu item 123 was not found.", ex.Message);
    }

    [Fact]
    public async Task RemoveItemFromBasketRemovesMenuItem()
    {
        using var context = TestDbFactory.CreateContext();
        var menuItem = new MenuItem { Id = 1, Name = "Item A", Price = 2.00m };
        var basket = new BasketModel { UserId = 1 };
        basket.Items.Add(new BasketItem { MenuItem = menuItem, MenuItemId = menuItem.Id, Quantity = 1 });
        context.AddRange(basket, menuItem);
        await context.SaveChangesAsync();

        var basketRepo = new Repo<BasketModel>(context);
        var handler = new RemoveItemFromBasketHandler(basketRepo);

        var result = await handler.Handle(new RemoveItemFromBasket(basket.Id, menuItem.Id), CancellationToken.None);

        Assert.Empty(result.MenuItems);
    }

    [Fact]
    public async Task RemoveItemFromBasketReturnsBasketWhenMissingMenuItem()
    {
        using var context = TestDbFactory.CreateContext();
        var basket = new BasketModel { UserId = 1 };
        context.Add(basket);
        await context.SaveChangesAsync();

        var basketRepo = new Repo<BasketModel>(context);
        var handler = new RemoveItemFromBasketHandler(basketRepo);

        var result = await handler.Handle(new RemoveItemFromBasket(basket.Id, 456), CancellationToken.None);

        Assert.Empty(result.MenuItems);
    }
}
