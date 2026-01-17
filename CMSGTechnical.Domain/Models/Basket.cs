using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSGTechnical.Domain.Interfaces;

namespace CMSGTechnical.Domain.Models
{
    public class Basket : IEntity
    {
        public int Id { get; set; }
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();

        public int UserId { get; set; }

    }
}
