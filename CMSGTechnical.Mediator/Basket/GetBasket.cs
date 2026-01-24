using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Basket
{
    public record GetBasket(int Id) : IRequest<BasketDto>;

    public class GetBasketHandler : IRequestHandler<GetBasket, BasketDto>
    {

        private IRepo<Domain.Models.Basket> Baskets { get; }

        public GetBasketHandler(IRepo<Domain.Models.Basket> baskets)
        {
            Baskets = baskets;
        }

        public async Task<BasketDto> Handle(GetBasket request, CancellationToken cancellationToken)
        {
            var r = await Baskets.GetAll()
                .Include(b => b.Items)
                .ThenInclude(i => i.MenuItem)
                .SingleAsync(b => b.Id == request.Id, cancellationToken);
            return r.ToDto();
        }
    }
}
