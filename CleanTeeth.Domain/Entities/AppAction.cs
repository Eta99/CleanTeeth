using CleanTeeth.Domain.Exceptions;

namespace CleanTeeth.Domain.Entities
{
    public class AppAction
    {
        public long Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Title { get; private set; } = null!;

        public ICollection<Role> Roles { get; private set; } = new List<Role>();

        private AppAction() { }

        public AppAction(string name, string title)
        {
            EnforceNameBusinessRules(name);
            EnforceTitleBusinessRules(title);
            Name = name;
            Title = title;
        }

        public void Update(string name, string title)
        {
            EnforceNameBusinessRules(name);
            EnforceTitleBusinessRules(title);
            Name = name;
            Title = title;
        }

        private static void EnforceNameBusinessRules(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException($"{nameof(name)} is required.");
        }

        private static void EnforceTitleBusinessRules(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleException($"{nameof(title)} is required.");
        }
    }
}
