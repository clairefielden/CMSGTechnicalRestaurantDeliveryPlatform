using Bunit;
using CMSGTechnical.Code;
using CMSGTechnical.Components.Shared;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using CMSGTechnical.Mediator.Tests.TestHelpers;
using Microsoft.AspNetCore.Components;

namespace CMSGTechnical.Mediator.Tests;

public class BasketDisplayTests : TestContext
{
    [Fact]
    public void BasketDisplayRendersTotals()
    {
        var item = new MenuItemDto { Id = 1, Name = "Item", Price = 4.00m };
        var basket = new BasketDto { Id = 1, MenuItems = new List<MenuItemDto> { item } };
        var mediator = new TestMediator((_, _) => Task.FromResult<object?>(basket));
        var basketService = new BasketService(mediator, basket);

        var cut = RenderComponent<BasketDisplay>(parameters => parameters
            .AddCascadingValue(basketService));

        var subtotal = 4.00m.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        var total = 6.00m.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

        Assert.Contains("Subtotal", cut.Markup);
        Assert.Contains(subtotal, cut.Markup);
        Assert.Contains(total, cut.Markup);
    }

    [Fact]
    public void BasketDisplayUpdatesWhenAddingAndRemoving()
    {
        var item = new MenuItemDto { Id = 1, Name = "Item", Price = 2.00m };
        var basket = new BasketDto { Id = 1, MenuItems = new List<MenuItemDto> { item } };

        var mediator = new TestMediator((request, _) =>
        {
            return request switch
            {
                AddItemToBasket => Task.FromResult<object?>(new BasketDto
                {
                    Id = 1,
                    MenuItems = new List<MenuItemDto> { item, item }
                }),
                RemoveItemFromBasket => Task.FromResult<object?>(new BasketDto { Id = 1, MenuItems = new List<MenuItemDto>() }),
                _ => Task.FromResult<object?>(basket)
            };
        });

        var basketService = new BasketService(mediator, basket);

        var cut = RenderComponent<BasketDisplay>(parameters => parameters
            .AddCascadingValue(basketService));

        cut.Find("button[aria-label='Add Item']").Click();
        cut.WaitForAssertion(() =>
        {
            var quantity = cut.Find(".basket-item__quantity").TextContent;
            Assert.Equal("2", quantity);
        });

        cut.Find("button[aria-label='Remove Item']").Click();
        cut.WaitForAssertion(() =>
        {
            var quantities = cut.FindAll(".basket-item__quantity");
            Assert.Empty(quantities);
        });
    }
}
