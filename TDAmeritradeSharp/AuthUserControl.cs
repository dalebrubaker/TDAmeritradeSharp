using Serilog;

namespace TDAmeritradeSharp
{
    public partial class AuthUserControl : UserControl
    {
        public AuthUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Can't get constructor into a UserControl, so just do the Serilog static logger
        /// </summary>
        private static ILogger Logger => Log.Logger.ForContext<AuthUserControl>();

        private void AuthUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            Logger.Debug("Loading AuthUserControl");
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