using System.Collections.Generic;

using LearningKitCore.Models.ECommerce.Products;


namespace LearningKitCore.Models.ECommerce.ProductFilters
{
    public class ProductFilterViewModel
    {
        public List<ProductFilterCheckboxViewModel> Manufacturers { get; set; }

        public List<ProductListItemViewModel> FilteredProducts { get; set; }

        public bool LPTWithFeature { get; set; }

        public decimal PriceFrom { get; set; }

        public decimal PriceTo { get; set; }

    }
}
