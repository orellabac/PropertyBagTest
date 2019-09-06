using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PropertyBagTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(this.GetType());
            var x = resources.GetObject("MyCuteProperty1");

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MyProperty" + this.userControl1121.MyProperty);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("My Property1" + this.userControl11.MyProperty);
        }
    }
}
