namespace LegacyRenewalApp
{
    public class EmailService : IEmailService
    {
        public void SendRenewalConfirmation(Customer customer, RenewalInvoice invoice)
        {
            if (string.IsNullOrWhiteSpace(customer.Email))
                return;

            string subject = "Subscription renewal invoice";
            string body = $"Hello {customer.FullName}, your renewal for plan {invoice.PlanCode} " +
                             $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            LegacyBillingGateway.SendEmail(customer.Email, subject, body);
        }
    }
}