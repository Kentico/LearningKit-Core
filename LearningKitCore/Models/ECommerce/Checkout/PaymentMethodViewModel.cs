using System.ComponentModel;

using Microsoft.AspNetCore.Mvc.Rendering;


namespace LearningKitCore.Models.ECommerce.Checkout
{

    //DocSection:PaymentViewModel
    public class PaymentMethodViewModel
    {
        [DisplayName("Payment method")]
        public int PaymentMethodID { get; set; }

        public SelectList PaymentMethods { get; set; }
    }
    //EndDocSection:PaymentViewModel
}
