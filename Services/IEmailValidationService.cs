namespace deneme.Services
{
    public interface IEmailValidationService
    {
        bool IsValidEmail(string email);
        string NormalizeEmail(string email);
    }
}

