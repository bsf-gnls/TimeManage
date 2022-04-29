using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace TimeManage
{
    public partial class Form1 : Form
    {
        public List<Course> m_course=new List<Course>();
        public int course_nums;//课程总数量
        public DateTime first_week;//第一周周一日期
        public DateTime selected_time;
        public Dictionary<DateTime,List<string>> m_list;
        public int selected_course_num;
        protected override void OnPaint(PaintEventArgs e)
        {
            panelMonth1.Location = new System.Drawing.Point(0, 0);
            panelMonth1.Size = new System.Drawing.Size((int)(0.75 * this.Width),this.Height-40);
            button1.Location = new System.Drawing.Point((int)(0.75 * this.Width), 0);
            button1.Size= new System.Drawing.Size((int)(0.25 * this.Width), (int)(0.125 * this.Height));
            button2.Location = new System.Drawing.Point((int)(0.75 * this.Width), (int)(0.125 * this.Height));
            button2.Size = new System.Drawing.Size((int)(0.25 * this.Width), (int)(0.125 * this.Height));
            listBox1.Location = new System.Drawing.Point((int)(0.75 * this.Width), (int)(0.25 * this.Height));
            listBox1.Size = new System.Drawing.Size((int)(0.25 * this.Width-20), (int)(0.75 * this.Height)-30);
            base.OnPaint(e);
        }
        public Form1()
        {
            m_list = new Dictionary<DateTime, List<string>>();
            selected_time =System.DateTime.Now;
            selected_time = new DateTime(selected_time.Year, selected_time.Month, selected_time.Day);
            InitializeComponent();
            course_nums = 0;
            Infile();
            m_Init();
            ListBoxInit();
            ((Control)panelMonth1).MouseClick += new MouseEventHandler(Form1_MouseClick);
        }
        private void ListBoxInit()
        {
            selected_course_num = 0;
            DateTime now = System.DateTime.Now;
            int ans = DateDiff(first_week, now);
            int week = ans / 7 + 1;
            int weekday = ans % 7 + 1;
            for (int i = 0; i < m_course.Count; i++)
            {
                if (weekday == m_course[i].weekday)
                {
                    foreach (int j in m_course[i].week.Keys)
                    {
                        if (j == week)
                        {
                            string str = "课程：" + m_course[i].course_name + " 地点:" + m_course[i].classroom + " 时间:" + m_course[i].start.ToString() + "-" + m_course[i].end.ToString() + "节";
                            listBox1.Items.Add(str);
                            selected_course_num++;
                        }
                    }
                }
            }
            foreach (KeyValuePair<DateTime, List<string>> i in m_list)
            {
                if (i.Key.Date == now.Date)
                {
                    for (int j = 0; j < i.Value.Count; j++)
                    {
                        string str = "日程: " + i.Value[j];
                        listBox1.Items.Add(str);
                    }
                }
            }
        }
        private void m_Init()
        {
            if (!File.Exists("./list.txt"))
                return;
           using (StreamReader sr = new StreamReader(@".\list.txt"))
            {
                string line;
                if ((line = sr.ReadLine()) != null)
                {
                    Date ans = new Date();
                    int.TryParse(line, out ans.year);
                    line = sr.ReadLine();
                    int.TryParse(line, out ans.month);
                    line = sr.ReadLine();
                    int.TryParse(line, out ans.day);
                    first_week = new DateTime(ans.year, ans.month, ans.day);
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "")
                            continue;
                        int.TryParse(line, out ans.year);
                        line = sr.ReadLine();
                        int.TryParse(line, out ans.month);
                        line = sr.ReadLine();
                        int.TryParse(line, out ans.day);
                        int n = new int();
                        line = sr.ReadLine();
                        int.TryParse(line, out n);
                        List<string> ans_list = new List<string>();
                        for (int i = 0; i < n; i++)
                        {
                            line = sr.ReadLine();
                            ans_list.Add(line);
                        }
                        DateTime dateTime = new DateTime(ans.year, ans.month, ans.day);
                        m_list.Add(dateTime, ans_list);
                    }
                }
            }
        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
        }

        private void button1_Click(object sender, EventArgs e)//导入课表
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //显示选择文件对话框
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.pdf)|*.pdf"; //所有的文件格式
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string filename="";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;   //显示文件路径
            }
            filename = "\""+filename+"\"";
            //string debugPath = System.Environment.CurrentDirectory;           //此c#项目的debug文件夹路径
            string pyexePath = @".\main.exe";
            //python文件所在路径，一般不使用绝对路径，此处仅作为例子，建议转移到debug文件夹下
            Process p = new Process();
            p.StartInfo.FileName = pyexePath;//需要执行的文件路径
            p.StartInfo.UseShellExecute = false; //必需
            p.StartInfo.RedirectStandardOutput = true;//输出参数设定
            p.StartInfo.RedirectStandardInput = true;//传入参数设定
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = filename;//参数以空格分隔，如果某个参数为空，可以传入””
            p.Start();
            p.StandardOutput.ReadToEnd();
            p.WaitForExit();//关键，等待外部程序退出后才能往下执行}
            p.Close();
            Infile();
            LoadCourse ans_form = new LoadCourse();
            ans_form.ShowDialog(this);
            first_week = ans_form.dt;
        }
        private void Infile()
        {
            m_course.Clear();
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(@".\课表.txt"))
                {
                    //string path = @".\课表.txt"; // 如果测试要改成自己地址
                    //                                // 声明一个utf-8编码对象
                    //UTF8Encoding utf8 = new UTF8Encoding();
                    //// 以行为单位读取所有文本文件内容，再赋值给一个字符串数组
                    //string[] contents = File.ReadAllLines(path, utf8);
                    string line;
                    int i = 1;
                    int n = 0;
                    Course ans_course=new Course();
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "")
                            continue;
                        switch (i)
                        {
                            case 1:
                                ans_course = new Course();
                                ans_course.course_name = line;
                                break;
                            case 2:
                                ans_course.weekday = line[0] - '0';
                                    break;
                            case 3:
                                ans_course.classroom = line;
                                break;
                            case 4:
                                ans_course.teacher = line;
                                break;
                            case 5:
                                if (line.Length == 3)
                                {
                                    ans_course.start = line[0] - '0';
                                    ans_course.end = line[2] - '0';
                                }
                                else if(line.Length==4)
                                {
                                    ans_course.start = line[0] - '0';
                                    ans_course.end = line[3] - '0'+10;
                                }
                                else
                                {
                                    ans_course.start = line[1] - '0'+10;
                                    ans_course.end = line[4] - '0' + 10;
                                }
                                break;
                            case 6:
                                int.TryParse(line, out n);
                                break;
                            case 7:
                                for(int j=0;j<n;j++)
                                {
                                    if(j!=0)
                                    line = sr.ReadLine();
                                    int ans=0;
                                    int.TryParse(line, out ans);
                                    ans_course.week.Add(ans, true);
                                }
                                m_course.Add(ans_course);
                                //ans_course.week.Clear();
                                break;
                        }
                        i = i % 7 + 1;
                    }
                    course_nums = m_course.Count;
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }
        private int DateDiff(DateTime dateStart, DateTime dateEnd)//计算日期差值;
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(course_nums==0)
            {
                DialogResult dr = MessageBox.Show("还未导入课表，请先导入课表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                CCourse ans = new CCourse(m_course);
                ans.Show();
            }
        }
            private void panelMonth1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panelMonth1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void panelMonth1_Click(object sender, EventArgs e)
        {
            if (panelMonth1.if_date)//点击的是日期
            {
                selected_time = panelMonth1.datetime;
                selected_course_num = 0;
                this.Controls.Remove(listBox1);
                listBox1 = new ListBox();
                listBox1.MouseClick += listBox1_MouseClick;
                this.listBox1.FormattingEnabled = true;
                this.listBox1.HorizontalScrollbar = true;
                this.listBox1.ScrollAlwaysVisible = true;
                listBox1.Location = new System.Drawing.Point((int)(0.75 * this.Width), (int)(0.25 * this.Height));
                listBox1.Size = new System.Drawing.Size((int)(0.25 * this.Width - 20), (int)(0.75 * this.Height) - 30);
                this.Controls.Add(listBox1);
                int ans = DateDiff(first_week, panelMonth1.datetime);
                if (ans >= 0)
                {
                    int week = ans / 7 + 1;
                    int weekday = ans % 7 + 1;
                    for (int i = 0; i < m_course.Count; i++)
                    {
                        if (weekday == m_course[i].weekday)
                        {
                            foreach (int j in m_course[i].week.Keys)
                            {
                                if (j == week)
                                {
                                    string str = "课程：" + m_course[i].course_name + " 地点:" + m_course[i].classroom+" 时间:" + m_course[i].start.ToString() + "-" + m_course[i].end.ToString()+"节";
                                    listBox1.Items.Add(str);
                                    selected_course_num++;
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<DateTime, List<string>> i in m_list)
                {
                    if (i.Key.Date == panelMonth1.datetime.Date)
                    {
                        for (int j = 0; j < i.Value.Count; j++)
                        {
                            string str = "日程: " + i.Value[j];
                            listBox1.Items.Add(str);
                        }
                    }
                }
                panelMonth1.if_date = false;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!File.Exists("./list.txt"))
            {
                FileStream fs1 = new FileStream("./list.txt", FileMode.Create, FileAccess.Write);//创建写入文件
            }
                using (StreamWriter sw = new StreamWriter("./list.txt"))
            {
                sw.WriteLine(first_week.Year.ToString());
                sw.WriteLine(first_week.Month.ToString());
                sw.WriteLine(first_week.Day.ToString());
                foreach(KeyValuePair<DateTime,List<string>>i in m_list)
                {
                    sw.WriteLine(i.Key.Year.ToString());
                    sw.WriteLine(i.Key.Month.ToString());
                    sw.WriteLine(i.Key.Day.ToString());
                    sw.WriteLine(i.Value.Count.ToString());
                    for(int j=0;j<i.Value.Count;j++)
                    {
                        sw.WriteLine(i.Value[j]);
                    }
                }
            }
            base.OnFormClosing(e);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 

        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string ans = listBox1.SelectedItem.ToString();
                int index = listBox1.Items.IndexOf(ans);
                if (ans[0] == '日')
                {
                    ans = ans.Substring(4);
                    ToDOList ans_form = new ToDOList(ans);
                    ans_form.ShowDialog();
                    if(ans_form.if_delete)
                    {
                        listBox1.Items.Remove(listBox1.SelectedItem);
                        m_list[selected_time].RemoveAt(index - selected_course_num);
                        if (m_list[selected_time].Count == 0)
                            m_list.Remove(selected_time);
                    }
                    else
                    {
                        listBox1.Items[index] = "日程: " + ans_form.m_str;
                        m_list[selected_time][index-selected_course_num]= ans_form.m_str;
                    }
                }
            }
            else
            {
                string ans = "";
                ToDOList ans_form = new ToDOList(ans);
                ans_form.ShowDialog();
                if(ans_form.if_insert)
                {
                    string str = "日程: " + ans_form.m_str;
                    int index = listBox1.Items.IndexOf(str);
                    if (index == -1)
                    {
                        listBox1.Items.Add(str);
                        bool if_haven = false;
                        foreach(DateTime i in m_list.Keys)
                        {
                            if (i == selected_time)
                                if_haven = true;
                        }
                        if (!if_haven)
                        {
                            List<string> ans_list = new List<string>();
                            ans_list.Add(ans_form.m_str);
                            m_list.Add(selected_time, ans_list);
                        }
                        else
                        m_list[selected_time].Add(str);
                    }
                }
            }
        }
    }
    public class Course
    {
        public string course_name;//课程名
        public int weekday;//周几的课
        public string classroom;//教室
        public string teacher;//老师
        public int start;//课程开始是第几节课
        public int end;//课程结束是第几节课
        public Dictionary<int, bool> week;//第几周上课
        public Course()
        {
            week = new Dictionary<int, bool>();
        }
    }
    public class Date
    {
       public int year;
       public int month;
       public int day;
    }
    }
