using System.Windows.Forms;

namespace MarkdownViewer.UI
{
    public class AccessibleToolStripStatusLabel : ToolStripStatusLabel
    {
        private string _accessibleName = "";
        private string _accessibleDescription = "";

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new AccessibleToolStripStatusLabelAccessibleObject(this);
        }

        public string AccessibleNameValue
        {
            get => _accessibleName;
            set => _accessibleName = value ?? "";
        }

        public string AccessibleDescriptionValue
        {
            get => _accessibleDescription;
            set => _accessibleDescription = value ?? "";
        }

        private class AccessibleToolStripStatusLabelAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject
        {
            private readonly AccessibleToolStripStatusLabel _owner;

            public AccessibleToolStripStatusLabelAccessibleObject(AccessibleToolStripStatusLabel owner) : base(owner)
            {
                _owner = owner;
            }

            public override string? Name
            {
                get => !string.IsNullOrEmpty(_owner._accessibleName) ? _owner._accessibleName : base.Name;
                set => base.Name = value;
            }

            public override string? Description
            {
                get => !string.IsNullOrEmpty(_owner._accessibleDescription) ? _owner._accessibleDescription : base.Description;
            }

            public override AccessibleRole Role => AccessibleRole.Link;
        }
    }
}
