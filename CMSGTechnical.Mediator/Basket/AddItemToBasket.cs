using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Basket
{
    public record AddItemToBasket(int BasketId, int MenuItemId) : IRequest<BasketDto>;

    public class AddItemToBasketHandler : IRequestHandler<AddItemToBasket, BasketDto>
    {
        private IRepo<Domain.Models.Basket> Baskets { get; }
        private IRepo<MenuItem> MenuItems { get; }

        public AddItemToBasketHandler(IRepo<Domain.Models.Basket> baskets, IRepo<MenuItem> menuItems)
        {
            Baskets = baskets;
            MenuItems = menuItems;
        }

        public async Task<BasketDto> Handle(AddItemToBasket request, CancellationToken cancellationToken)
        {
            var basket = await Baskets.GetAll()
                .Include(b => b.Items)
                .ThenInclude(i => i.MenuItem)
                .SingleAsync(b => b.Id == request.BasketId, cancellationToken);

            var menuItem = await MenuItems.Get(request.MenuItemId, cancellationToken);
            if (menuItem is null)
                throw new InvalidOperationException($"Menu item {request.MenuItemId} was not found.");

            var basketItem = basket.Items.SingleOrDefault(item => item.MenuItemId == request.MenuItemId);
            if (basketItem is null)
            {
                basket.Items.Add(new BasketItem
                {
                    MenuItemId = menuItem.Id,
                    MenuItem = menuItem,
                    Quantity = 1
                });
            }
            else
            {
                basketItem.Quantity += 1;
            }
            await Baskets.Update(basket, cancellationToken);

            return basket.ToDto();
        }
    }
}
