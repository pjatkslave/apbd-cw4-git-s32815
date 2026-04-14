using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyRenewalApp
{
    public class PricingService : IPricingService
    {
        private readonly IEnumerable<IDiscountStrategy> _discountStrategies;

        private static readonly Dictionary<string, (decimal Rate, string Note)> PaymentFees =
            new Dictionary<string, (decimal, string)>
            {
                { "CARD",          (0.020m, "card payment fee") },
                { "BANK_TRANSFER", (0.010m, "bank transfer fee") },
                { "PAYPAL",        (0.035m, "paypal fee") },
                { "INVOICE",       (0.000m, "invoice payment") },
            };

        private static readonly Dictionary<string, decimal> SupportFees =
            new Dictionary<string, decimal>
            {
                { "START",      250m },
                { "PRO",        400m },
                { "ENTERPRISE", 700m },
            };

        private static readonly Dictionary<string, decimal> TaxRates =
            new Dictionary<string, decimal>
            {
                { "Poland",         0.23m },
                { "Germany",        0.19m },
                { "Czech Republic", 0.21m },
                { "Norway",         0.25m },
            };

        private const decimal MinSubtotal = 300m;
        private const decimal MinFinalAmount = 500m;

        public PricingService(IEnumerable<IDiscountStrategy> discountStrategies)
        {
            _discountStrategies = discountStrategies;
        }

        public RenewalInvoice BuildInvoice(
            Customer customer,
            SubscriptionPlan plan,
            int seatCount,
            string normalizedPaymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            var notes = new StringBuilder();

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            decimal discountAmount = 0m;
            foreach (var strategy in _discountStrategies)
            {
                if (strategy.IsApplicable(customer, plan, seatCount, useLoyaltyPoints))
                {
                    discountAmount += strategy.Calculate(baseAmount, customer, seatCount);
                    notes.Append(strategy.Note).Append("; ");
                }
            }

            decimal subtotal = baseAmount - discountAmount;
            if (subtotal < MinSubtotal)
            {
                subtotal = MinSubtotal;
                notes.Append("minimum discounted subtotal applied; ");
            }

            decimal supportFee = 0m;
            if (includePremiumSupport && SupportFees.TryGetValue(plan.Code, out decimal fee))
            {
                supportFee = fee;
                notes.Append("premium support included; ");
            }

            if (!PaymentFees.TryGetValue(normalizedPaymentMethod, out var paymentInfo))
                throw new ArgumentException("Unsupported payment method");

            decimal paymentFee = paymentInfo.Rate * (subtotal + supportFee);
            notes.Append(paymentInfo.Note).Append("; ");

            decimal taxRate = TaxRates.TryGetValue(customer.Country, out decimal rate) ? rate : 0.20m;
            decimal taxBase = subtotal + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;

            decimal finalAmount = taxBase + taxAmount;
            if (finalAmount < MinFinalAmount)
            {
                finalAmount = MinFinalAmount;
                notes.Append("minimum invoice amount applied; ");
            }

            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{plan.Code}",
                CustomerName = customer.FullName,
                PlanCode = plan.Code,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.ToString().Trim(),
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}