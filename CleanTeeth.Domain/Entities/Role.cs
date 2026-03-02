using CleanTeeth.Domain.Exceptions;

namespace CleanTeeth.Domain.Entities
{
    public class Role
    {
        public long Id { get; private set; }
        public string Title { get; private set; } = null!;

        public ICollection<User> Users { get; private set; } = new List<User>();
        public ICollection<AppAction> Actions { get; private set; } = new List<AppAction>();

        private Role() { }

        public Role(string title)
        {
            EnforceTitleBusinessRules(title);
            Title = title;
        }

        public void UpdateTitle(string title)
        {
            EnforceTitleBusinessRules(title);
            Title = title;
        }

        private static void EnforceTitleBusinessRules(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleException($"{nameof(title)} is required.");
        }
    }
}
