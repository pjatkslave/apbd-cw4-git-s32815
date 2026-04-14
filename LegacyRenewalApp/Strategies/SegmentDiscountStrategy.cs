using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SegmentDiscountStrategy : IDiscountStrategy
    {
        private static readonly Dictionary<string, decimal> SegmentRates = new Dictionary<string, decimal>
        {
            { "Silver",    0.05m },
            { "Gold",      0.10m },
            { "Platinum",  0.15m },
        };

        public string Note => "segment discount";

        public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
        {
            if (SegmentRates.ContainsKey(customer.Segment))
                return true;

            return customer.Segment == "Education" && plan.IsEducationEligible;
        }

        public decimal Calculate(decimal baseAmount, Customer customer, int seatCount)
        {
            if (SegmentRates.TryGetValue(customer.Segment, out decimal rate))
                return baseAmount * rate;

            return baseAmount * 0.20m;
        }
    }
}