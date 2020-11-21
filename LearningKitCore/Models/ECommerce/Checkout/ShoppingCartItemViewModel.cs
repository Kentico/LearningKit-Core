namespace LearningKitCore.Models.ECommerce.Checkout
{

    //DocSection:SCViewModel
    public class ShoppingCartItemViewModel
    {
        public string SKUName { get; set; }

        public int SKUID { get; set; }

        public string SKUImageUrl { get; set; }

        public int CartItemUnits { get; set; }

        public decimal TotalPrice { get; set; }

        public int CartItemID { get; set; }
    }
    //EndDocSection:SCViewModel

}
