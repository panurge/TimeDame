using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeDame
{
    public partial class MessageBoxK : Form
    {
        public string message = "";

        public MessageBoxK()
        {
            InitializeComponent();
        }

        private void MessageBoxK_Load(object sender, EventArgs e)
        {
            textBox1.Text = message;
        }
    }
}
