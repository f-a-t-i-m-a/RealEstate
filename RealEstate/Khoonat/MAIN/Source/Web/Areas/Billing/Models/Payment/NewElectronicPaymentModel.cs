using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    public class NewElectronicPaymentModel
    {
        [Range(0, 100000000, ErrorMessage = "پوزش مارو بپذیرید. درگاه پرداخت به زودی راه اندازی خواهد شد.")]
        public decimal Amount { get; set; }

        public PaymentGatewayProvider PaymentGatewayProvider { get; set; }
    }
}