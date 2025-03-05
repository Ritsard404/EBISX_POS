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
        public string? Size { get; set; }
        public bool HasSize { get; set; }
        public bool IsSolo { get; set; }
        public bool  IsAddOn { get; set; }

        private string? _imagePath;
        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                ItemImage = string.IsNullOrEmpty(value) ? null : LoadBitmap(value);
            }
        }

        public Bitmap? ItemImage { get; private set; }

        private Bitmap? LoadBitmap(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                var uri = new Uri(path);
                var assets = AssetLoader.Open(uri);
                return new Bitmap(assets);
            }
            catch (ArgumentNullException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
