using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Basket
{
    public record RemoveItemFromBasket(int BasketId, int MenuItemId) : IRequest<BasketDto>;

    public class RemoveItemFromBasketHandler : IRequestHandler<RemoveItemFromBasket, BasketDto>
    {
        private IRepo<Domain.Models.Basket> Baskets { get; }
        private IRepo<MenuItem> MenuItems { get; }

        public RemoveItemFromBasketHandler(IRepo<Domain.Models.Basket> baskets, IRepo<MenuItem> menuItems)
        {
            Baskets = baskets;
            MenuItems = menuItems;
        }

        public async Task<BasketDto> Handle(RemoveItemFromBasket request, CancellationToken cancellationToken)
        {
            var basket = await Baskets.GetAll()
                .Include(b => b.MenuItems)
                .SingleAsync(b => b.Id == request.BasketId, cancellationToken);

            var menuItem = await MenuItems.Get(request.MenuItemId, cancellationToken);
            if (menuItem is null)
                throw new InvalidOperationException($"Menu item {request.MenuItemId} was not found.");

            basket.MenuItems.Remove(menuItem);
            await Baskets.Update(basket, cancellationToken);

            return basket.ToDto();
        }
    }
}
