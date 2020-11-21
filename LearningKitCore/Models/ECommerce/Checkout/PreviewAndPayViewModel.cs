namespace LearningKitCore.Models.ECommerce.Checkout
{
    //DocSection:PreviewAndPayViewModel
    public class PreviewAndPayViewModel
    {
        public DeliveryDetailsViewModel DeliveryDetails { get; set; }

        public ShoppingCartViewModel Cart { get; set; }

        public PaymentMethodViewModel PaymentMethod { get; set; }

    }
    //EndDocSection:PreviewAndPayViewModel
}
