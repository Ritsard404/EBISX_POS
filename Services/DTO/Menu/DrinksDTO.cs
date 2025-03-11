using EBISX_POS.API.Services.DTO.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services.DTO.Menu
{
    /// <summary>
    /// Represents drink options and available sizes for a menu item
    /// </summary>
    public class DrinksDTO
    {
        /// <summary>
        /// Categorized drink types (e.g., Sodas, Juices) with their options
        /// </summary>
        public List<DrinkTypeDTO> DrinkTypesWithDrinks { get; set; }

        /// <summary>
        /// Available size options for drinks
        /// </summary>
        public List<string> Sizes { get; set; }
    }

    /// <summary>
    /// Represents a category of drinks
    /// </summary>
    public class DrinkTypeDTO
    {
        /// <summary>
        /// Unique identifier for the drink category
        /// </summary>
        public int DrinkTypeId { get; set; }

        /// <summary>
        /// Display name for the drink category
        /// </summary>
        public string DrinkTypeName { get; set; }

        /// <summary>
        /// List of drink options in this category
        /// </summary>
        //public List<DrinkDetailDTO> Drinks { get; set; }


        public List<SizesWithPricesDTO>? SizesWithPrices { get; set; } = new List<SizesWithPricesDTO>();
    }

    /// <summary>
    /// Represents an individual drink option
    /// </summary>
    public class DrinkDetailDTO
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string? MenuImagePath { get; set; }
        public decimal MenuPrice { get; set; }
        public string? Size { get; set; }
        public bool IsUpgradeMeal { get; set; }
    }
}
