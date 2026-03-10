using CleanTeeth.Domain.Exceptions;

namespace CleanTeeth.Domain.Entities
{
    public class AppAction
    {
        public long Id { get; private set; }
        public long TypeId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public bool IsLoggable { get; private set; }

        public ActionType Type { get; private set; } = null!;
        public ICollection<Role> Roles { get; private set; } = new List<Role>();

        private AppAction() { }

        public AppAction(long typeId, string name, string title, bool isLoggable = true)
        {
            TypeId = typeId;
            EnforceNameBusinessRules(name);
            EnforceTitleBusinessRules(title);
            Name = name;
            Title = title;
            IsLoggable = isLoggable;
        }

        public void Update(long typeId, string name, string title, bool isLoggable = true)
        {
            TypeId = typeId;
            EnforceNameBusinessRules(name);
            EnforceTitleBusinessRules(title);
            Name = name;
            Title = title;
            IsLoggable = isLoggable;
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
