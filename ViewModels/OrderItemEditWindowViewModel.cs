using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public partial class OrderItemEditWindowViewModel : ViewModelBase
    {
        public OrderItemState OrderItem { get; }
        public OrderItemEditWindowViewModel(OrderItemState orderItem)
        {
            OrderItem = orderItem;

        }
    }
}
