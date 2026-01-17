using CMSGTechnical.Code;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using CMSGTechnical.Mediator.Tests.TestHelpers;

namespace CMSGTechnical.Mediator.Tests;

public class BasketServiceTests
{
    [Fact]
    public async Task AddUpdatesBasketAndRaisesChange()
    {
        var menuItem = new MenuItemDto { Id = 11, Name = "Item", Price = 1.00m };
        var initial = new BasketDto { Id = 1 };

        var mediator = new TestMediator((request, _) =>
        {
            if (request is AddItemToBasket)
            {
                return Task.FromResult<object?>(new BasketDto
                {
                    Id = 1,
                    MenuItems = new List<MenuItemDto> { menuItem }
                });
            }

            throw new InvalidOperationException("Unexpected request.");
        });

        var service = new BasketService(mediator, initial);
        var changes = 0;
        service.OnChange += (_, args) =>
        {
            changes++;
            Assert.Single(args.Basket.MenuItems);
        };

        await service.Add(menuItem);

        Assert.Equal(1, changes);
        Assert.Single(service.Basket.MenuItems);
    }

    [Fact]
    public async Task RemoveUpdatesBasketAndRaisesChange()
    {
        var menuItem = new MenuItemDto { Id = 12, Name = "Item", Price = 1.00m };
        var initial = new BasketDto { Id = 2, MenuItems = new List<MenuItemDto> { menuItem } };

        var mediator = new TestMediator((request, _) =>
        {
            if (request is RemoveItemFromBasket)
            {
                return Task.FromResult<object?>(new BasketDto { Id = 2, MenuItems = new List<MenuItemDto>() });
            }

            throw new InvalidOperationException("Unexpected request.");
        });

        var service = new BasketService(mediator, initial);
        var changes = 0;
        service.OnChange += (_, args) =>
        {
            changes++;
            Assert.Empty(args.Basket.MenuItems);
        };

        await service.Remove(menuItem);

        Assert.Equal(1, changes);
        Assert.Empty(service.Basket.MenuItems);
    }
}
