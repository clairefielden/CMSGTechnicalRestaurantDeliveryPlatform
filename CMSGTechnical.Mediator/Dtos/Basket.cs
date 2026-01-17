using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;

namespace CMSGTechnical.Mediator.Dtos
{
    public class BasketDto
    {
        public int Id { get; set; }
        public ICollection<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();

        public int UserId { get; set; }

        public decimal Subtotal => MenuItems.Sum(item => item.Price);
        public decimal DeliveryFee => 2.00m;
        public decimal Total => Subtotal + DeliveryFee;
    }


    public static class BasketExtensions
    {


        public static IEnumerable<BasketDto> ToDto(this IEnumerable<Domain.Models.Basket> models) =>
            models.Select(i => i.ToDto()).ToArray();

        public static BasketDto ToDto(this Domain.Models.Basket model)
        {
            var menuItems = model.Items
                .Where(item => item.MenuItem != null)
                .SelectMany(item => Enumerable.Repeat(item.MenuItem.ToDto(), item.Quantity))
                .ToList();

            return new BasketDto()
            {
                Id = model.Id,
                MenuItems = menuItems,
                UserId = model.UserId
            };
        }
    }

}
