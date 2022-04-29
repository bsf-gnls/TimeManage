using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeManage
{
    
    public partial class ToDOList : Form
    {
        public bool if_delete;
        public bool if_insert;
        public ToDOList(string str)
        {
            InitializeComponent();
            m_str = str;
            if_delete = false;
            if_insert = false;
            textBox1.Text = str;
        }
        public string m_str;

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text=="")
            {
                DialogResult dr = MessageBox.Show("添加为空,请输入内容!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if_insert = true;
            m_str = textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if_delete = true;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_str = textBox1.Text;
            this.Close();
        }
    }
}
