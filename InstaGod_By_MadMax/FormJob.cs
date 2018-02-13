using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaGod_By_MadMax
{
    public partial class FormJob : Form
    {
        private UserInfo User;
        public FormJob(UserInfo user)
        {
            InitializeComponent();
            User = user;
        }

        private void FormJob_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    textBox12.Text = saveFileDialog1.FileName;
                    myStream.Close();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage4) {
                User.status = "Ожидание";
                User.task = "Фолловинг";
                Globals.JobMaker.CreateAction("follow", new string[] { textBox7.Text}, User,FollowList);
            }

            if (tabControl1.SelectedTab == tabPage1)
            {
                User.status = "Ожидание";
                User.task = "Лайкинг";
                Globals.JobMaker.CreateAction("like", new string[] { textBox2.Text }, User, LikeList);
            }

            if (tabControl1.SelectedTab == tabPage5)
            {
                User.status = "Ожидание";
                User.task = "АнФолловинг";
                Globals.JobMaker.CreateAction("unfollow", new string[] { textBox11.Text ,Convert.ToString(checkBox1.Checked),textBox10.Text}, User);
            }
            if (tabControl1.SelectedTab == tabPage7)
            {
                User.status = "Ожидание";
                User.task = "Парсинг";
                string jobName;
                if (radioButton1.Checked)
                {
                  if (radioButton3.Checked) { 
                   jobName = "parsuserliker";
                    Globals.JobMaker.CreateAction(jobName,
                       new string[] { textBox13.Text, Convert.ToString(numericUpDown2.Value), textBox12.Text }, User);
                    }
                    if (radioButton4.Checked)
                    {
                        jobName = "parsuserfollowers";
                        Globals.JobMaker.CreateAction(jobName,
                           new string[] { textBox13.Text, Convert.ToString(numericUpDown1.Value), textBox12.Text }, User);
                    }
                }

                if (radioButton2.Checked)
                {
                        jobName = "parstag";
                    Globals.JobMaker.CreateAction(jobName,
                       new string[] { textBox4.Text, Convert.ToString(numericUpDown3.Value), textBox12.Text }, User);
                }
                   
                   
                
            }


            ActiveForm.Close();
        }
        Queue<string> FollowList = new Queue<string>();
        private void button3_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {

                    string line;
                    StreamReader reader = new StreamReader(myStream);
                    while ((line = reader.ReadLine()) != null)
                    {
                        FollowList.Enqueue(line);
                    }
                    label22.Text = Convert.ToString(FollowList.Count);
                    textBox9.Text = openFileDialog1.FileName;
                }
                
                
              
                    
                
            }
        }
        Queue<string> LikeList = new Queue<string>();
        private void button4_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {

                    string line;
                    StreamReader reader = new StreamReader(myStream);
                    while ((line = reader.ReadLine()) != null)
                    {
                        LikeList.Enqueue(line);
                    }
                    label1.Text = Convert.ToString(LikeList.Count);
                    textBox1.Text = openFileDialog1.FileName;
                }





            }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox2.Visible = false;
                groupBox1.Visible = true;
            }

            if (radioButton2.Checked)
            {
                groupBox1.Visible = false;
                groupBox2.Visible = true;
            }


        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
               
                label8.Visible = true;
                numericUpDown1.Visible = false;

                label18.Visible = false;
                numericUpDown2.Visible = true;
            }

            if (radioButton4.Checked)
            {
                label8.Visible = false;
                numericUpDown1.Visible = true;

                
                label18.Visible = true;
                numericUpDown2.Visible = false;



            }
        }
    }
}
