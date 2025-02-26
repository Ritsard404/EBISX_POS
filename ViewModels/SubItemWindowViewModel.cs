using EBISX_POS.Models;

namespace EBISX_POS.ViewModels
{
    public class SubItemWindowViewModel
    {
        public ItemMenu Item { get; }

        public SubItemWindowViewModel(ItemMenu item)
        {
            Item = item;
        }
    }
}
