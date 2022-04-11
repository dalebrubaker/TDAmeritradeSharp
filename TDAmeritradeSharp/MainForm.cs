using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TDAmeritradeSharp
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;

        public MainForm(ILogger<MainForm> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            InitializeComponent();
            _logger.LogTrace("Ctor");
        }

        /// <summary>
        ///     This is used by user controls created by the designer (empty constructor) to access a logger or other services
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            _logger.LogTrace("Loading");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }
            _logger.LogTrace("Closing");
        }
    }
}