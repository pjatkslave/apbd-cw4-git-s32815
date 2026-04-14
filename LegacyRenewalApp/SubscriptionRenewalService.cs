using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IPricingService _pricingService;
        private readonly IBillingService _billingService;
        private readonly IEmailService _emailService;
        public SubscriptionRenewalService(
            ISubscriptionRepository repository,
            IPricingService pricingService,
            IBillingService billingService,
            IEmailService emailService)
        {
            _repository = repository;
            _pricingService = pricingService;
            _billingService = billingService;
            _emailService = emailService;
        }

        public SubscriptionRenewalService()
            : this(
                repository: new SubscriptionRepository(),
                pricingService: new PricingService(
                    discountStrategies: new IDiscountStrategy[]
                    {
                        new SegmentDiscountStrategy(),
                        new LoyaltyYearsDiscountStrategy(),
                        new TeamSizeDiscountStrategy(),
                        new LoyaltyPointsDiscountStrategy(),
                    }),
                billingService: new BillingService(),
                emailService: new EmailService())
        {
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            ValidateInputs(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _repository.GetCustomerById(customerId);
            var plan = _repository.GetPlanByCode(normalizedPlanCode);

            if (!customer.IsActive)
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

            var invoice = _pricingService.BuildInvoice(
                customer, plan, seatCount,
                normalizedPaymentMethod,
                includePremiumSupport,
                useLoyaltyPoints);

            _billingService.SaveInvoice(invoice);
            _emailService.SendRenewalConfirmation(customer, invoice);

            return invoice;
        }

        private static void ValidateInputs(int customerId, string planCode, int seatCount, string paymentMethod)
        {
            if (customerId <= 0)
                throw new ArgumentException("Customer id must be positive");

            if (string.IsNullOrWhiteSpace(planCode))
                throw new ArgumentException("Plan code is required");

            if (seatCount <= 0)
                throw new ArgumentException("Seat count must be positive");

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("Payment method is required");
        }
    }
}