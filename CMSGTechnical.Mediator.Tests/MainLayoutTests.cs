using Bunit;
using CMSGTechnical.Components.Layout;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using CMSGTechnical.Mediator.Tests.TestHelpers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CMSGTechnical.Mediator.Tests;

public class MainLayoutTests : TestContext
{
    [Fact]
    public void MainLayoutTogglesBasketDrawer()
    {
        var basket = new BasketDto { Id = 1 };
        var mediator = new TestMediator((request, _) =>
        {
            return request switch
            {
                GetBasket => Task.FromResult<object?>(basket),
                _ => Task.FromResult<object?>(basket)
            };
        });

        Services.AddSingleton<IMediator>(mediator);

        var cut = RenderComponent<MainLayout>();

        var toggle = cut.Find("button.basket-toggle");
        toggle.Click();
        Assert.Contains("is-open", cut.Find("aside.basket-drawer").ClassList);

        cut.Find("button.basket-drawer__scrim").Click();
        Assert.DoesNotContain("is-open", cut.Find("aside.basket-drawer").ClassList);

        cut.Dispose();
    }

    [Fact]
    public async Task MainLayoutRespondsToBasketChanges()
    {
        var menuItem = new MenuItemDto { Id = 1, Name = "Item", Price = 2.00m };
        var initial = new BasketDto { Id = 1 };

        var mediator = new TestMediator((request, _) =>
        {
            return request switch
            {
                GetBasket => Task.FromResult<object?>(initial),
                AddItemToBasket => Task.FromResult<object?>(new BasketDto
                {
                    Id = 1,
                    MenuItems = new List<MenuItemDto> { menuItem }
                }),
                _ => Task.FromResult<object?>(initial)
            };
        });

        Services.AddSingleton<IMediator>(mediator);

        var cut = RenderComponent<MainLayout>();

        var basketServiceProperty = typeof(MainLayout).GetProperty("BasketService", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(basketServiceProperty);

        var basketService = basketServiceProperty!.GetValue(cut.Instance) as CMSGTechnical.Code.BasketService;
        Assert.NotNull(basketService);

        var expectedTotal = 4.00m.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

        await cut.InvokeAsync(() => basketService!.Add(menuItem));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Basket (1)", cut.Markup);
            Assert.Contains(expectedTotal, cut.Markup);
        });
    }
}
