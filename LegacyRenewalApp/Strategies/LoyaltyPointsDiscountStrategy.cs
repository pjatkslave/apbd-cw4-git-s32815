namespace LegacyRenewalApp
{
    public class LoyaltyPointsDiscountStrategy : IDiscountStrategy
    {
        public string Note => "loylty points discount";
        public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints) => useLoyaltyPoints && customer.LoyaltyPoints > 0;

        public decimal Calculate(decimal baseAmount, Customer customer, int seatCount)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            return pointsToUse;
        }
    }
}