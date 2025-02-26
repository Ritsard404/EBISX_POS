using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace EBISX_POS.Models
{
    public class ItemMenu
    {
        public int Id { get; set; }
        public required string ItemName { get; set; }
        public required decimal Price { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                ItemImage = LoadBitmap(value);
            }
        }

        public Bitmap? ItemImage { get; private set; }

        private Bitmap? LoadBitmap(string path)
        {
            try
            {
                var uri = new Uri(path);
                var assets = AssetLoader.Open(uri);
                return new Bitmap(assets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image loading error: {ex.Message}");
                return null;
            }
        }
    }
}
