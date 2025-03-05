﻿using System;
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
        public List<DrinkDetailDTO> Drinks { get; set; }
    }

    /// <summary>
    /// Represents an individual drink option
    /// </summary>
    public class DrinkDetailDTO
    {
        /// <summary>
        /// Name of the drink item
        /// </summary>
        public string MenuName { get; set; }
        
        /// <summary>
        /// Optional path to the drink's display image
        /// </summary>
        public string? MenuImagePath { get; set; }
    }
}
