using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Dtos;
using BasketModel = CMSGTechnical.Domain.Models.Basket;

namespace CMSGTechnical.Mediator.Tests;

public class DomainDtoTests
{
    [Fact]
    public void BasketDtoTotalsReflectMenuItems()
    {
        var basket = new BasketDto
        {
            MenuItems = new List<MenuItemDto>
            {
                new() { Id = 1, Name = "Item A", Price = 4.50m },
                new() { Id = 2, Name = "Item B", Price = 2.25m }
            }
        };

        Assert.Equal(6.75m, basket.Subtotal);
        Assert.Equal(2.00m, basket.DeliveryFee);
        Assert.Equal(8.75m, basket.Total);
    }

    [Fact]
    public void BasketExtensionsMapModelToDto()
    {
        var menuItem = new MenuItem { Id = 3, Name = "Pizza", Price = 9.99m };
        var basket = new BasketModel
        {
            Id = 7,
            UserId = 2,
            Items = new List<BasketItem>
            {
                new() { MenuItem = menuItem, MenuItemId = menuItem.Id, Quantity = 1 }
            }
        };

        var dto = basket.ToDto();

        Assert.Equal(7, dto.Id);
        Assert.Equal(2, dto.UserId);
        Assert.Single(dto.MenuItems);
        Assert.Equal("Pizza", dto.MenuItems.First().Name);
    }

    [Fact]
    public void MenuItemExtensionsMapChildItems()
    {
        var child = new MenuItem { Id = 2, Name = "Add-on", Price = 1.25m };
        var parent = new MenuItem
        {
            Id = 1,
            Name = "Main",
            Price = 5.00m,
            ChildItems = new List<MenuItem> { child }
        };

        var dto = parent.ToDto();

        Assert.Equal("Main", dto.Name);
        Assert.Single(dto.ChildItems);
        Assert.Equal("Add-on", dto.ChildItems.First().Name);
    }
}
