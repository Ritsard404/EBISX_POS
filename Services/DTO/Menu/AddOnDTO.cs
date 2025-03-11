using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services.DTO.Menu
{
    /// <summary>
    /// Represents a collection of add-on categories and their associated items
    /// </summary>
    public class AddOnDTO
    {
        /// <summary>
        /// List of add-on categories (e.g., Sauces, Toppings) with their items
        /// </summary>
        public List<AddOnTypeDTO> AddOnTypesWithAddOns { get; set; }
    }

    /// <summary>
    /// Represents a category of add-on items
    /// </summary>
    public class AddOnTypeDTO
    {
        /// <summary>
        /// Unique identifier for the add-on category
        /// </summary>
        public int AddOnTypeId { get; set; }

        /// <summary>
        /// Display name for the add-on category
        /// </summary>
        public string AddOnTypeName { get; set; }

        /// <summary>
        /// List of individual add-on items in this category
        /// </summary>
        public List<AddOnDetailDTO> AddOns { get; set; }
    }

    /// <summary>
    /// Represents an individual add-on item
    /// </summary>
    public class AddOnDetailDTO
    {
        public int MenuId { get; set; }

        /// <summary>
        /// Name of the add-on item
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// Optional path to the add-on's display image
        /// </summary>
        public string? MenuImagePath { get; set; }

        /// <summary>
        /// Optional size specification for the add-on
        /// </summary>
        public string? Size { get; set; }
        public bool HasSize { get; set; } = false;

        /// <summary>
        /// Price of the add-on item
        /// </summary>
        public decimal Price { get; set; }
        public bool IsUpgradeMeal { get; set; }
    }
}
