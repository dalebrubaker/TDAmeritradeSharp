using Microsoft.Extensions.Logging;

namespace TDAmeritradeSharp
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;

        public MainForm(ILogger<MainForm> logger)
        {
            _logger = logger;
            InitializeComponent();
            _logger.LogInformation("Ctor");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _logger.LogInformation("Loading");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.LogInformation("Closing");
        }
    }
}