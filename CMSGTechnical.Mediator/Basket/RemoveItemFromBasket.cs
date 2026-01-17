using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Basket
{
    public record RemoveItemFromBasket(int BasketId, int MenuItemId) : IRequest<BasketDto>;

    public class RemoveItemFromBasketHandler : IRequestHandler<RemoveItemFromBasket, BasketDto>
    {
        private IRepo<Domain.Models.Basket> Baskets { get; }

        public RemoveItemFromBasketHandler(IRepo<Domain.Models.Basket> baskets)
        {
            Baskets = baskets;
        }

        public async Task<BasketDto> Handle(RemoveItemFromBasket request, CancellationToken cancellationToken)
        {
            var basket = await Baskets.GetAll()
                .Include(b => b.Items)
                .ThenInclude(i => i.MenuItem)
                .SingleAsync(b => b.Id == request.BasketId, cancellationToken);

            var basketItem = basket.Items.SingleOrDefault(item => item.MenuItemId == request.MenuItemId);
            if (basketItem is null)
                return basket.ToDto();

            if (basketItem.Quantity > 1)
            {
                basketItem.Quantity -= 1;
            }
            else
            {
                basket.Items.Remove(basketItem);
            }
            await Baskets.Update(basket, cancellationToken);

            return basket.ToDto();
        }
    }
}
