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
    public partial class CCourse : Form
    {
        public List<Course> m_list;
        public int m_max = 0;
        public TextBox[] head;
        public Dictionary<Course,TextBox> course;
        public CCourse(List<Course> list)
        {
            m_list = list;
            m_max = 0;
            course = new Dictionary<Course, TextBox>();
                for(int i=0;i<m_list.Count;i++)
                {
                    foreach(int key in m_list[i].week.Keys)
                {
                    if (key > m_max)
                        m_max = key;
                }
                }
            InitializeComponent();
            head = new TextBox[14];
            IList<Info> infoList = new List<Info>();
            for(int i=0;i<m_max;i++)
            {
                string x = "第";
                string y = (i + 1).ToString();
                string z = "周";
                Info ans = new Info { Id = i + 1, Name = x + y+z };
                infoList.Add(ans);
            }
            comboBox1.DataSource = infoList;
            comboBox1.ValueMember = "Id";
            comboBox1.DisplayMember = "Name";
            for(int i=0;i<8;i++)
            {
                string ans = "";
                if(i!=0)
                {
                    switch (i)
                    {
                        case 1:
                            ans = "星期一";
                                break;
                        case 2:
                            ans = "星期二";
                            break;
                        case 3:
                            ans = "星期三";
                            break;
                        case 4:
                            ans = "星期四";
                            break;
                        case 5:
                            ans = "星期五";
                            break;
                        case 6:
                            ans = "星期六";
                            break;
                        case 7:
                            ans = "星期七";
                            break;
                    }
                }
                head[i] = new TextBox();
                head[i].Multiline = true;
                head[i].Size = new System.Drawing.Size((this.Width-10) / 8,((int)(0.954*this.Height-36)) / 7);
                head[i].Location = new System.Drawing.Point(i * ((this.Width - 10) / 8), (int)(0.046 * this.Height));
                head[i].Font = new Font(head[i].Font.FontFamily, 12, head[i].Font.Style);
                head[i].Text = ans;
                this.Controls.Add(head[i]);
            }
            for (int i = 8; i < 14; i++)
            {
                string ans = ((i - 7) * 2 - 1).ToString() + "-" + ((i - 7) * 2).ToString() + "节";
                head[i] = new TextBox();
                head[i].Font = new Font(head[i].Font.FontFamily, 12, head[i].Font.Style);
                head[i].Multiline = true;
                head[i].Size = new System.Drawing.Size((this.Width - 10) / 8, ((int)(0.954 * this.Height-36)) / 7);
                head[i].Location = new System.Drawing.Point(0,(i-7) * ((int)(0.954 * this.Height-36)) / 7+ (int)(0.046 * this.Height));
                head[i].Text = ans;
                this.Controls.Add(head[i]);
            }
            find(1);
        }
        public class Info
        {
            public int Id { get; set; }
            public string Name { get; set; }

        }
        private void find(int x)
        {
            foreach(TextBox i in course.Values)
            {
                this.Controls.Remove(i);
            }
            course.Clear();
            int Height = ((int)(0.954 * this.Height - 36)) / 7;
            int Width = (this.Width - 10) / 8;
            TextBox ans = new TextBox();
            for (int i = 0; i < m_list.Count; i++)
            {
                foreach (int j in m_list[i].week.Keys)
                {
                    if (j == x)
                    {
                        ans = new TextBox();
                        int start = m_list[i].start;
                        int end = m_list[i].end;
                        ans.Size = new System.Drawing.Size(Width, (end / 2 - start / 2) *Height);
                        ans.Location = new System.Drawing.Point(m_list[i].weekday * (this.Width - 10) / 8, (start / 2 + 1) * ((int)(0.954 * this.Height - 36)) / 7 + (int)(0.046 * this.Height));
                        ans.Multiline = true;
                        ans.Text = m_list[i].course_name + "\r\n" + m_list[i].classroom;
                        this.Controls.Add(ans);
                        course.Add(m_list[i],ans);
                        break;
                    }
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            comboBox1.Location = new System.Drawing.Point((int)(0.384 * this.Width), 0);
            comboBox1.Size = new System.Drawing.Size((int)(0.181* this.Width), (int)(0.046 * this.Height));
            for (int i = 0; i < 8; i++)
            {
                head[i].Size = new System.Drawing.Size((this.Width - 10) / 8, ((int)(0.954 * this.Height - 36)) / 7);
                head[i].Location = new System.Drawing.Point(i * ((this.Width - 10) / 8), (int)(0.046 * this.Height));
            }
            for (int i = 8; i < 14; i++)
            {
                head[i].Size = new System.Drawing.Size((this.Width - 10) / 8, ((int)(0.954 * this.Height - 36)) / 7);
                head[i].Location = new System.Drawing.Point(0, (i - 7) * ((int)(0.954 * this.Height - 36)) / 7 + (int)(0.046 * this.Height));
            }
            foreach(KeyValuePair< Course,TextBox >i in course)
            {
                int Height = ((int)(0.954 * this.Height - 36)) / 7;
                int Width = (this.Width - 10) / 8;
                i.Value.Location = new System.Drawing.Point(i.Key.weekday * (this.Width - 10) / 8, (i.Key.start / 2 + 1) * Height + (int)(0.046 * this.Height));
                i.Value.Size = new System.Drawing.Size(Width, (i.Key.end / 2 - i.Key.start / 2) * Height);
            }
            base.OnPaint(e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            find(comboBox1.SelectedIndex + 1);
        }
    }
}
