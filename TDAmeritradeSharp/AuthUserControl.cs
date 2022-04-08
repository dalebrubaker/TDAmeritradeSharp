using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDAmeritradeSharp
{
    public partial class AuthUserControl : UserControl
    {
        public AuthUserControl()
        {
            InitializeComponent();
        }

        private void AuthUserControl_Load(object sender, EventArgs e)
        {

        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }
    }
}
