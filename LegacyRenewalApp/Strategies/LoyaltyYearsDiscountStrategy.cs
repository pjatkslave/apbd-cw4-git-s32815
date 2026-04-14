namespace LegacyRenewalApp
{
    public class LoyaltyYearsDiscountStrategy : IDiscountStrategy
    {
        public string Note => "loyalty years discount";
        public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints) => customer.YearsWithCompany >= 2;
        public decimal Calculate(decimal baseAmount, Customer customer, int seatCount) => baseAmount * (customer.YearsWithCompany >= 5 ? 0.07m : 0.03m);
    }
}