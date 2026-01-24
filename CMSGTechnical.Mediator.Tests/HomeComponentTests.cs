using Bunit;
using CMSGTechnical.Code;
using CMSGTechnical.Components.Pages;
using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using CMSGTechnical.Mediator.Menu;
using CMSGTechnical.Mediator.Tests.TestHelpers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CMSGTechnical.Mediator.Tests;

public class HomeComponentTests : TestContext
{
    [Fact]
    public void HomeRendersAddOnButtonsForSpecificItems()
    {
        var menuItems = new List<MenuItemDto>
        {
            new() { Id = 1, Name = "Caesar Salad", Price = 8.99m, Category = MenuItemCategory.Poultry },
            new() { Id = 2, Name = "Add Chicken to Caesar Salad", Price = 2.50m, Category = MenuItemCategory.Poultry },
            new() { Id = 3, Name = "Spaghetti Carbonara", Price = 14.99m, Category = MenuItemCategory.Beef },
            new() { Id = 4, Name = "Add Beef Mince to Carbonara", Price = 3.00m, Category = MenuItemCategory.Beef }
        };

        var basket = new BasketDto { Id = 1 };
        var addCalled = false;
        Task<object?> HandleAdd()
        {
            addCalled = true;
            return Task.FromResult<object?>(basket);
        }
        var mediator = new TestMediator((request, _) =>
        {
            return request switch
            {
                GetMenuItems => Task.FromResult<object?>(menuItems),
                AddItemToBasket => HandleAdd(),
                RemoveItemFromBasket => Task.FromResult<object?>(basket),
                _ => Task.FromResult<object?>(basket)
            };
        });

        Services.AddSingleton<IMediator>(mediator);

        var basketService = new BasketService(mediator, basket);
        var cut = RenderComponent<Home>(parameters => parameters.AddCascadingValue(basketService));

        Assert.Contains("Add Chicken", cut.Markup);
        Assert.Contains("Add Beef Mince", cut.Markup);

        cut.Find("button[aria-label='Add item to basket']").Click();
        Assert.True(addCalled);
    }

    [Fact]
    public async Task HomeAddAddOnIgnoresNull()
    {
        var menuItems = new List<MenuItemDto>();
        var basket = new BasketDto { Id = 1 };
        var mediator = new TestMediator((request, _) =>
        {
            return request switch
            {
                GetMenuItems => Task.FromResult<object?>(menuItems),
                _ => Task.FromResult<object?>(basket)
            };
        });

        Services.AddSingleton<IMediator>(mediator);

        var basketService = new BasketService(mediator, basket);
        var cut = RenderComponent<Home>(parameters => parameters.AddCascadingValue(basketService));

        var method = typeof(Home).GetMethod("AddAddOn", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(method);

        var task = (Task)method!.Invoke(cut.Instance, new object?[] { null })!;
        await task;
    }
}
