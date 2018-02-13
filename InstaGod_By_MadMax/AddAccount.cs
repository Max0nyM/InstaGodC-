using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;


namespace InstaGod_By_MadMax
{
    public partial class AddAccount : Form
    {
        public AddAccount()
        {
            InitializeComponent();
        }
      

    

       

        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {

                
            for(int i = 0; i <= textBox1.Lines.Length - 1; i++) {
            string[] acc = textBox1.Lines[i].Split(':');
            Globals.Acclist.AddAcount(acc[0],acc[1]);
           }
     
            Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                MessageBox.Show(exception.ToString());
               
            }
        }

        private void accListBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {


        }

        private void AddAccount_Load(object sender, EventArgs e)
        {
           // LoadAccs();

        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }
    }
}
