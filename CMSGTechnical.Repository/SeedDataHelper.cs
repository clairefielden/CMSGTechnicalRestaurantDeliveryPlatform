using CMSGTechnical.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Repository;

internal static class SeedDataHelper
{
    public static ModelBuilder SeedData(this ModelBuilder builder)
    {
        builder.SeedBasket();
        builder.SeedMenu();

        return builder;
    }


    private static void SeedBasket(this ModelBuilder builder)
    {
        builder.Entity<Basket>().HasData(new Basket()
        {
            Id = 1,
        });
    }


    private static void SeedMenu(this ModelBuilder builder)
    {
        var id = 1;
        builder.Entity<MenuItem>().HasData(
            new
            {
                Id = id++, Order = 0,
                Name = "Margherita Pizza",
                Description = "Classic pizza with fresh tomatoes, mozzarella cheese, and basil.",
                Price = 12.99m,
                Category = MenuItemCategory.Vegetarian
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Caesar Salad",
                Description = "Crisp romaine lettuce with Caesar dressing, croutons, and parmesan cheese.",
                Price = 8.99m,
                Category = MenuItemCategory.Poultry
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Add Chicken to Caesar Salad",
                Description = "Add-on: grilled chicken for Caesar Salad.",
                Price = 2.50m,
                Category = MenuItemCategory.Poultry
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Grilled Salmon",
                Description = "Grilled salmon fillet served with a side of roasted vegetables.",
                Price = 18.99m,
                Category = MenuItemCategory.Seafood
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Spaghetti Carbonara",
                Description = "Pasta in a creamy sauce with pancetta, parmesan cheese, and black pepper.",
                Price = 14.99m,
                Category = MenuItemCategory.Beef
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Add Beef Mince to Carbonara",
                Description = "Add-on: beef mince for Spaghetti Carbonara.",
                Price = 3.00m,
                Category = MenuItemCategory.Beef
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Vegetable Stir Fry",
                Description = "Seasonal vegetables stir-fried with garlic, ginger, and a light soy glaze.",
                Price = 11.99m,
                Category = MenuItemCategory.Vegetarian
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Chicken Parmesan",
                Description =
                    "Breaded chicken breast topped with marinara sauce and mozzarella cheese, served with pasta.",
                Price = 16.99m,
                Category = MenuItemCategory.Poultry
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Beef Tacos",
                Description = "Three soft tacos filled with seasoned beef, lettuce, cheese, and salsa.",
                Price = 10.99m,
                Category = MenuItemCategory.Beef
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Clam Chowder",
                Description = "Creamy chowder with clams, potatoes, and celery.",
                Price = 7.99m,
                Category = MenuItemCategory.Seafood
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Veggie Burger",
                Description = "Plant-based burger patty with lettuce, tomato, and onion on a whole wheat bun.",
                Price = 9.99m,
                Category = MenuItemCategory.Vegetarian
            },
            new
            {
                Id = id++, Order = 0,
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake with a smooth ganache and a dusting of cocoa.",
                Price = 6.99m,
                Category = MenuItemCategory.Vegetarian
            }
        );
    }
}
