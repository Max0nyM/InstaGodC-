using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace InstaGod_By_MadMax
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.listView1.ColumnWidthChanging += new ColumnWidthChangingEventHandler(listView1_ColumnWidthChanging);
        }



        public Thread[] Threads;


        void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            Console.Write("Column Resizing");
            e.NewWidth = this.listView1.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAccount FormAdd = new AddAccount();
            FormAdd.ShowDialog();
            AccList();
        }


        private void удалитьАккаунтToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Globals.Acclist.DeleteAccount(Globals.GetMainUserList[listView1.SelectedIndices[0]]);
                AccList();
            }
            catch
            {
            }


        }

        int sInd = 0;

        public void ChangeGui()
        {
            for (int i = 0; i <= listView1.Items.Count - 1; i++)
            {
                if (Globals.GetMainUserList[i].status != "-")
                {
                    listView1.Items[i].SubItems[1].Text = (Globals.GetMainUserList[i].posts.ToString());
                    listView1.Items[i].SubItems[2].Text = (Globals.GetMainUserList[i].following.ToString());
                    listView1.Items[i].SubItems[3].Text = (Globals.GetMainUserList[i].followers.ToString());
                    listView1.Items[i].SubItems[4].Text = (Globals.GetMainUserList[i].task.ToString());
                    listView1.Items[i].SubItems[5].Text = (Globals.GetMainUserList[i].status.ToString());
                    listView1.Items[i].SubItems[6].Text = (Globals.GetMainUserList[i].process.ToString());
                }
            }
        }

        public void AccList()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            label3.Text = Convert.ToString(Globals.GetMainUserList.Count);
            label4.Text = "0";
            foreach (var m in Globals.GetMainUserList)
            {
                ListViewItem item1 = new ListViewItem(m.login);
                item1.SubItems.Add(m.posts.ToString());
                item1.SubItems.Add(m.following.ToString());
                item1.SubItems.Add(m.followers.ToString());
                item1.SubItems.Add(m.task);
                item1.SubItems.Add(m.status);
                item1.SubItems.Add(m.process.ToString());
                listView1.Items.AddRange(new ListViewItem[] {item1});
                label4.Text = Convert.ToString(Convert.ToInt32(label4.Text) + m.followers);
            }

            listView1.EndUpdate();
        }


       
        private void Form1_Load(object sender, EventArgs e)
        {


            AccList();

        }

        void LoginAndCheck()
        {
            var User = Globals.GetMainUserList[listView1.SelectedIndices[0]];
            InstaGodApi Insta = new InstaGodApi();
            var info = Insta.GetInfo(User);
            if (info != null)
            {
                User.followers = Convert.ToInt32(info["user"]["followed_by"]["count"]);
                User.following = Convert.ToInt32(info["user"]["follows"]["count"]);
                User.posts = Convert.ToInt32(info["user"]["media"]["count"]);
                if (User.cookie == null)
                    User.cookie = Insta.login(User);
                if (User.cookie != null && User.cookie.ToString().Contains("session"))
                    User.status = "Авторизирован";
                else
                {
                    User.status = "ОшАвторизации";
                    User.cookie = null;
                }
                MessageBox.Show(User.cookie.ToString());
                Globals.Acclist.ChangeAccount(User);
            }
            else
            {
                User.status = "Бан/Смс";
                User.cookie = null;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoginAndCheck();
            AccList();
        }




        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            var User = Globals.GetMainUserList[listView1.SelectedIndices[0]];
            if (User.cookie == null)
                LoginAndCheck();
            if (User.cookie != null)
                using (FormJob MassLikeByTags = new FormJob(User))
                {
                    MassLikeByTags.Text = "Задание аккаунта - " + User.login;
                    MassLikeByTags.ShowDialog();
                }
            else
            {
                MessageBox.Show("Ошибка в Авторизации", "ОШИБКА", buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error);
            }
            AccList();
        }

        private void поТегамToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            var User = Globals.GetMainUserList[listView1.SelectedIndices[0]];
            Globals.JobMaker.GetActions(User);
            AccList();
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ChangeGui();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i <= listView1.Items.Count - 1; i++)
            {
                var User = Globals.GetMainUserList[i];
                User.status = "-";
                User.process = 0;
                User.task = "-";
                Globals.Acclist.ChangeAccount(User);
            }
        }



        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var User = Globals.GetMainUserList[e.ItemIndex];
            groupBox1.Text = "Управление Аккаунтом - " + User.login;
            if (User.cookie != null && User.task != "-" && User.status=="Ожидание")
                button4.Enabled = true;
            else
            {
               button4.Enabled = false;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                contextMenuStrip1.Items[3].Enabled = false;
                contextMenuStrip1.Items[4].Enabled = false;
            }
            else
            {
                var User = Globals.GetMainUserList[listView1.SelectedIndices[0]];
                if (User.cookie != null && User.status !="Работает")
                {
                    contextMenuStrip1.Items[3].Enabled = true;
                    contextMenuStrip1.Items[4].Enabled = true;
                }
                else
                {
                    contextMenuStrip1.Items[3].Enabled = true;
                    contextMenuStrip1.Items[4].Enabled = false;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
