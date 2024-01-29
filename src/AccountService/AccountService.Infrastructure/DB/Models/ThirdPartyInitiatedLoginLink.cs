namespace AccountService.Infrastructure.DB.Models
{
    public class ThirdPartyInitiatedLoginLink
    {
        public string LinkText { get; set; }
        public string InitiateLoginUri { get; set; }
    }
}
