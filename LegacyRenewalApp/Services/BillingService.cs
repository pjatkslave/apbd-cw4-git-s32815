namespace LegacyRenewalApp
{
    public class BillingService : IBillingService
    {
        public void SaveInvoice(RenewalInvoice invoice)
            => LegacyBillingGateway.SaveInvoice(invoice);
    }
}