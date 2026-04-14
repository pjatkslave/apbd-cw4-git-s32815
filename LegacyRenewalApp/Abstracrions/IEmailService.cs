namespace LegacyRenewalApp
{
    public interface IEmailService
    {
        void SendRenewalConfirmation(Customer customer, RenewalInvoice invoice);
    }
}