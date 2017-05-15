using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TimeDame
{
    public partial class Load : Form
    {
        public Load()
        {
            InitializeComponent();
        }
       
        public string opgave
        {
            get;
            private set;
        }


        private void Load_Load(object sender, EventArgs e)
        {
            foreach (string l in File.ReadLines("log.txt").Reverse())
            {
                string[] m = { l };
                dataGridView1.Rows.Add(m);
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {

            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                opgave = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            opgave = null;
            this.Close();
        }
    }
}

