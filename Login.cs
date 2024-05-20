using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECO
{
    public partial class Login : Form
    {
        public string eco;
        SqlHelper CL = new SqlHelper();
        public Login(string eco = "")
        {
            InitializeComponent();
            this.eco = eco;
        }
        private void Login_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            CL.LoadCombobox(comboBox1, "Select DEPT_NAME from  ENG_DEPTFLM");
        }

        private void LoginClick(object sender, EventArgs e)
        {
            // Kiem tra dang nhap
            if (CL.DangNhap(textBox1.Text.Trim(), comboBox1.Text) == true)
            {
                FormListECO f2 = new FormListECO();
                f2.getBP = comboBox1.Text;
                f2.GetUser = textBox1.Text;
                f2.eco = this.eco;
                Constants.ROLE = ROLE.CONFIRM;
                this.Hide();
                f2.ShowDialog();
                this.Close();
            }
            else
                MessageBox.Show("Không hợp lệ");
        }

        private void btnViewOnly_Click(object sender, EventArgs e)
        {
            Constants.ROLE = ROLE.VIEW_ONLY;
            FormListECO f2 = new FormListECO();
            this.Hide();
            f2.ShowDialog();
            this.Close();
        }
    }
}
