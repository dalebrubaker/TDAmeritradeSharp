using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TDAmeritradeSharp
{
    public partial class AuthUserControl : UserControl
    {
        private ILogger<AuthUserControl>? _logger;

        public AuthUserControl()
        {
            InitializeComponent();
        }

        private void AuthUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            // Here in Load we now have a ParentForm and can retrieve a logger from DI
            // ReSharper disable once AssignNullToNotNullAttribute
            var mainForm = (MainForm)ParentForm;
            _logger = mainForm.ServiceProvider.GetRequiredService<ILogger<AuthUserControl>>();
            _logger?.LogTrace("Loading AuthUserControl");
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            base.OnHandleDestroyed(e);
        }
    }
}