using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinFormScaffolding
{
    public partial class Form1 : Form
    {
  
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txtModel.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtOutput.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Scaffolding scaffold = new Scaffolding();

            if (!string.IsNullOrEmpty(txtOutput.Text))
                scaffold.OutPutAddress = txtOutput.Text;

            if (!string.IsNullOrEmpty(txtNameSpace.Text))
                scaffold.NameSpace = txtNameSpace.Text;

            if (scaffold.Pars(txtModel.Text))
            {
                MessageBox.Show("Complete!");
            }
        }
    }
}
