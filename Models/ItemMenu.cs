using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Models
{
    public class ItemMenu
    {
        public int Id { get; set; }
        public required string ItemName { get; set; }
        public required decimal Price { get; set; }
        public string? ItemImage { get; set; }
        //public byte[]? ItemImage { get; set; }
    }
}
