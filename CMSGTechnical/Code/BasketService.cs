using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace CMSGTechnical.Code
{

    public class BasketChangedEventArgs : EventArgs
    {
        public BasketDto Basket { get; set; } = new();
    }


    public class BasketService
    {

        public event EventHandler<BasketChangedEventArgs>? OnChange;

        private IMediator Mediator { get; }

        public BasketDto Basket { get; private set; }

        public BasketService(IMediator mediator, BasketDto basket)
        {
            Mediator = mediator;
            Basket = basket;
        }


        public async Task Add(MenuItemDto item)
        {
            Basket = await Mediator.Send(new AddItemToBasket(Basket.Id, item.Id));
            OnChange?.Invoke(this, new BasketChangedEventArgs { Basket = Basket });
        }

        public async Task Remove(MenuItemDto item)
        {
            Basket = await Mediator.Send(new RemoveItemFromBasket(Basket.Id, item.Id));
            OnChange?.Invoke(this, new BasketChangedEventArgs { Basket = Basket });
        }

    }
}
