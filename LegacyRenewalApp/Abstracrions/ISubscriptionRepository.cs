namespace LegacyRenewalApp
{
    public interface ISubscriptionRepository
    {
        Customer GetCustomerById(int customerId);
        SubscriptionPlan GetPlanByCode(string code);
    }
}