namespace LegacyRenewalApp
{
    public interface IPricingService
    {
        RenewalInvoice BuildInvoice(
            Customer customer,
            SubscriptionPlan plan,
            int seatCount,
            string normalizedPaymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints);
    }
}