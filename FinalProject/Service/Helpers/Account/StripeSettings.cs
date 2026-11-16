namespace Service.Helpers.Account
{
    public class StripeSettings
    {
        // https://dashboard.stripe.com/test/apikeys -> "Secret key" (sk_test_...)
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string Currency { get; set; } = "azn";
    }
}
