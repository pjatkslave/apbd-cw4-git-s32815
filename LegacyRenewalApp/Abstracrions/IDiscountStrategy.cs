namespace LegacyRenewalApp
{
    public interface IDiscountStrategy
    {
        bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints);
        decimal Calculate(decimal baseAmount, Customer customer, int seatCount);
        string Note { get; }
    }
}