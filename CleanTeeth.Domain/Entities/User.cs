using CleanTeeth.Domain.Exceptions;

namespace CleanTeeth.Domain.Entities
{
    public class User
    {
        public long Id { get; private set; }
        public string Login { get; private set; } = null!;

        public ICollection<Role> Roles { get; private set; } = new List<Role>();

        private User() { }

        public User(string login)
        {
            EnforceLoginBusinessRules(login);
            Login = login;
        }

        public void UpdateLogin(string login)
        {
            EnforceLoginBusinessRules(login);
            Login = login;
        }

        private static void EnforceLoginBusinessRules(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new BusinessRuleException($"{nameof(login)} is required.");
        }
    }
}
