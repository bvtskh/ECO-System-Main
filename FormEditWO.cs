using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using ECO.Busisness;
using ECO.DAL;
using CommonProject.MsgBox_AQ;

namespace ECO
{
    public partial class FormEditWO : Form
    {
        WoChanging ECOObject;
        public FormEditWO(WoChanging eco)
        {
            InitializeComponent();
            ECOObject = eco;
            SqlHelper cl = new SqlHelper();
            txbEco.Text = eco.ECO_NO;
            txbDept.Text = eco.DEPT_NAME;
            txbStatus.Text = "F.Implement";
            dateTimePicker1.Value = eco.DUE_DATE;
            txbActor.Text = eco.UPDATER_NAME;
            txbActor.Text = cl.LayDLTuCombobox(eco.ECO_NO, "Select ECO_No, EXE_USER from ENG_ECODETM where ECO_NO='" + eco.ECO_NO + "' and DEPT_CODE='" + eco.DEPT_NAME + "'");
            txtRemark.Text = cl.LayDLTuCombobox(eco.ECO_NO, "Select ECO_No, EXE_RMK from ENG_ECODETM where ECO_NO='" + eco.ECO_NO + "' and DEPT_CODE='" + eco.DEPT_NAME + "'");
            txtWO.Text = eco.ORDER_NO;
            rdAI.Checked = eco.TYPE_ID == (int)TYPE.AI;
            rdSMT.Checked = eco.TYPE_ID == (int)TYPE.SMT;
            rdFAT.Checked = eco.TYPE_ID == (int)TYPE.FAT;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var validate = ValidateHelper.CheckWOValid(txbWoChange.Text.Trim());
            if (!string.IsNullOrEmpty(validate))
            {
                RJMessageBox.Show(validate, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtWO.Text.Trim() == txbWoChange.Text.Trim())
            {
                RJMessageBox.Show("Không có sự thay đổi WO", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = Repository.UpdateWO(new WoChanging()
            {
                ECO_NO = lbEco.Text.Trim(),
                ORDER_NO = txbWoChange.Text.Trim(),
                DEPT_NAME = ECOObject.DEPT_NAME
            });
            RJMessageBox.Show(result, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

