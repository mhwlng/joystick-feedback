
namespace joystick_feedback
{
    public class SettingBindingExtension : System.Windows.Data.Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            Source = Properties.Settings.Default;
            Mode = System.Windows.Data.BindingMode.TwoWay;
        }
    }
}
