using Bunit;
using CMSGTechnical.Components.Shared;
using CMSGTechnical.Mediator.Dtos;
using Microsoft.AspNetCore.Components;

namespace CMSGTechnical.Mediator.Tests;

public class MenuItemDisplayTests : TestContext
{
    [Theory]
    [InlineData("Margherita Pizza", "/images/margherita.png")]
    [InlineData("Caesar Salad", "/images/caeser_salad.png")]
    [InlineData("Add Chicken to Caesar Salad", "/images/caeser_salad.png")]
    [InlineData("Grilled Salmon", "/images/salmon.png")]
    [InlineData("Spaghetti Carbonara", "/images/spaghetti_carbonara.png")]
    [InlineData("Add Beef Mince to Carbonara", "/images/spaghetti_carbonara.png")]
    [InlineData("Vegetable Stir Fry", "/images/veg_stirfry.png")]
    [InlineData("Chicken Parmesan", "/images/chicken_parmesan.png")]
    [InlineData("Beef Tacos", "/images/beef_taco.png")]
    [InlineData("Clam Chowder", "/images/clam_chowder.png")]
    [InlineData("Veggie Burger", "/images/veggie_burger.png")]
    [InlineData("Chocolate Cake", "/images/choc_cake.png")]
    [InlineData("Unknown", "/images/trolley.png")]
    public void MenuItemDisplayUsesExpectedImage(string name, string expectedSrc)
    {
        var item = new MenuItemDto { Id = 1, Name = name, Price = 1.00m };

        var cut = RenderComponent<MenuItemDisplay>(parameters => parameters
            .Add(p => p.Item, item));

        var image = cut.Find("img");
        Assert.Equal(expectedSrc, image.GetAttribute("src"));
    }

    [Fact]
    public void MenuItemDisplayInvokesAddAndAddOnCallbacks()
    {
        var item = new MenuItemDto { Id = 1, Name = "Caesar Salad", Price = 8.99m };
        var addOn = new MenuItemDto { Id = 2, Name = "Add Chicken to Caesar Salad", Price = 2.50m };
        var added = false;
        var addOnAdded = false;
        var removed = false;

        var cut = RenderComponent<MenuItemDisplay>(parameters => parameters
            .Add(p => p.Item, item)
            .Add(p => p.OnAdd, EventCallback.Factory.Create<MenuItemDto>(this, _ => added = true))
            .Add(p => p.AddOnItem, addOn)
            .Add(p => p.OnAddOn, EventCallback.Factory.Create<MenuItemDto?>(this, _ => addOnAdded = true))
            .Add(p => p.OnRemove, EventCallback.Factory.Create<MenuItemDto>(this, _ => removed = true)));

        cut.Find("button[aria-label='Add item to basket']").Click();
        cut.FindAll("button").Single(button => button.TextContent.Contains("Add Chicken")).Click();
        cut.FindAll("button").Single(button => button.TextContent == "-").Click();

        Assert.True(added);
        Assert.True(addOnAdded);
        Assert.True(removed);
    }

    [Fact]
    public void MenuItemDisplayRendersAddOnPrice()
    {
        var item = new MenuItemDto { Id = 1, Name = "Spaghetti Carbonara", Price = 14.99m };
        var addOn = new MenuItemDto { Id = 2, Name = "Add Beef Mince to Carbonara", Price = 3.00m };

        var cut = RenderComponent<MenuItemDisplay>(parameters => parameters
            .Add(p => p.Item, item)
            .Add(p => p.AddOnItem, addOn)
            .Add(p => p.OnAddOn, EventCallback.Factory.Create<MenuItemDto?>(this, _ => { })));

        var expectedPrice = 3.00m.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

        Assert.Contains("Add Beef Mince", cut.Markup);
        Assert.Contains(expectedPrice, cut.Markup);
    }

    [Fact]
    public void MenuItemDisplayUsesDefaultAddOnLabel()
    {
        var item = new MenuItemDto { Id = 1, Name = "Vegetable Stir Fry", Price = 11.99m };
        var addOn = new MenuItemDto { Id = 2, Name = "Extra Sauce", Price = 1.00m };

        var cut = RenderComponent<MenuItemDisplay>(parameters => parameters
            .Add(p => p.Item, item)
            .Add(p => p.AddOnItem, addOn)
            .Add(p => p.OnAddOn, EventCallback.Factory.Create<MenuItemDto?>(this, _ => { })));

        Assert.Contains("Extra Sauce", cut.Markup);
    }
}
