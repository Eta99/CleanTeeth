using CleanTeeth.Domain.Exceptions;

namespace CleanTeeth.Domain.Entities
{
    public class ActionType
    {
        public long Id { get; private set; }
        public string Name { get; private set; } = null!;

        public ICollection<AppAction> Actions { get; private set; } = new List<AppAction>();

        private ActionType() { }

        public ActionType(string name)
        {
            EnforceNameBusinessRules(name);
            Name = name;
        }

        public void Update(string name)
        {
            EnforceNameBusinessRules(name);
            Name = name;
        }

        private static void EnforceNameBusinessRules(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException($"{nameof(name)} is required.");
        }
    }
}
