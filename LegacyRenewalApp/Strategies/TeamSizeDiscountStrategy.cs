namespace LegacyRenewalApp
{
    public class TeamSizeDiscountStrategy : IDiscountStrategy
    {
        public string Note => "team size discount";
        public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints) => seatCount >= 10;
        public decimal Calculate(decimal baseAmount, Customer customer, int seatCount) => baseAmount * (seatCount >= 50 ? 0.12m : seatCount >= 20 ? 0.08m : 0.04m);
    }
}