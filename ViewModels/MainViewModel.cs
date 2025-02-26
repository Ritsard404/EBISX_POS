namespace EBISX_POS.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ItemListViewModel ItemList { get; }

        public MainViewModel()
        {
            ItemList = new ItemListViewModel();
        }
    }
}
