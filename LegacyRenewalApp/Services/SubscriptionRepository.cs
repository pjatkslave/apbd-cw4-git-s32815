namespace LegacyRenewalApp
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly CustomerRepository _customerRepository;
        private readonly SubscriptionPlanRepository _planRepository;

        public SubscriptionRepository()
        {
            _customerRepository = new CustomerRepository();
            _planRepository = new SubscriptionPlanRepository();
        }

        public Customer GetCustomerById(int customerId)
            => _customerRepository.GetById(customerId);

        public SubscriptionPlan GetPlanByCode(string code)
            => _planRepository.GetByCode(code);
    }
}