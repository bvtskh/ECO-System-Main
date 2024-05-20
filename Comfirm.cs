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
    public partial class Comfirm : Form
    {
        public string deptName;
        public string ECONo;
        public string User;
        SqlHelper db = new SqlHelper();
        public Comfirm()
        {
            InitializeComponent();
        }

        private void Comfirm_Load(object sender, EventArgs e)
        {
            cbbDept.Text = deptName;
            cbbStatus.Items.Clear();
            cbbStatus.Items.Add("F.Implement");
            cbbStatus.SelectedIndex = 0;
            dpDueDate.MaxDate = DateTime.Now;
            this.Text = $"Confirm {ECONo}";
            var controlInDept = ValidateHelper.ControlViewByDept(cbbDept.Text);
            if (!string.IsNullOrEmpty(controlInDept))
            {
                this.Controls[controlInDept].Enabled = true;
            }

            // set data
            var WO = Repository.GetWO(ECONo);
            if (WO != null)
            {
                txtWO.Text = WO.ORDER_NO;
                rdAI.Checked = WO.TYPE_ID == (int)TYPE.AI;
                rdSMT.Checked = WO.TYPE_ID == (int)TYPE.SMT;
                rdFAT.Checked = WO.TYPE_ID == (int)TYPE.FAT;
                if (WO.PE_OPTION == (int)PE_OPTION.FULL)
                {
                    pbFlow.Image = global::ECO.Properties.Resources.congdoan1;
                    rbFull.Checked = true;
                }
                else if (WO.PE_OPTION == (int)PE_OPTION.EDIT)
                {
                    pbFlow.Image = global::ECO.Properties.Resources.congdoan2;
                    rbFlow2.Checked = true;
                }
                else
                {
                    pbFlow.Image = global::ECO.Properties.Resources.icons8_error_80;
                }
            }
            var DeptCode = db.LayDLTuCombobox(deptName, "Select DEPT_NAME, DEPT_CODE from ENG_DEPTFLM where DEPT_NAME = '" + deptName + "' ");
            if (db.CheckECOComfirm(ECONo, DeptCode) == true)
            {
                cbbStatus.Text = db.LayDLTuCombobox(ECONo, "Select ECO_NO,EXE_STATE from ENG_ECODETM where ECO_NO='" + ECONo + "' and DEPT_CODE='" + DeptCode + "'");
                dpDueDate.Text = db.LayDLTuCombobox(ECONo, "Select ECO_No, EXE_DATE from ENG_ECODETM where ECO_NO='" + ECONo + "' and DEPT_CODE='" + DeptCode + "'");
                txtActor.Text = db.LayDLTuCombobox(ECONo, "Select ECO_No, EXE_USER from ENG_ECODETM where ECO_NO='" + ECONo + "' and DEPT_CODE='" + DeptCode + "'");
                txtRemark.Text = db.LayDLTuCombobox(ECONo, "Select ECO_No, EXE_RMK from ENG_ECODETM where ECO_NO='" + ECONo + "' and DEPT_CODE='" + DeptCode + "'");
            }
        }

        private string GetTypeName()
        {
            string typeName = "";
            if (rdAI.Checked)
            {
                typeName = "AI";
            }
            else if (rdSMT.Checked)
            {
                typeName = "SMT";
            }
            else if (rdFAT.Checked)
            {
                typeName = "FAT";
            }
            else if (rbRom.Checked)
            {
                typeName = "ROM";
            }
            else typeName = "DEFAULT";
            return typeName;
        }
        private void SaveClick(object sender, EventArgs e)
        {
            var validate = ValidateTextBox();
            if (!validate) return;

            var DeptCode = db.LayDLTuCombobox(deptName, "Select DEPT_NAME, DEPT_CODE from ENG_DEPTFLM where DEPT_NAME = '" + deptName + "' ");
            var dept_ord = 0;
            if(DeptCode == "PE ACTUAL")
            {
                dept_ord = (int)Enum.Parse(typeof(DEPT), "PE_ACTUAL");
            }
            else
            {
                dept_ord = (int)Enum.Parse(typeof(DEPT), DeptCode);
            }
            var peOption = PE_OPTION.NONE;
            if (rbFull.Checked) peOption = PE_OPTION.FULL;
            if (rbFlow2.Checked) peOption = PE_OPTION.EDIT;
            if (rbFlow4.Checked) peOption = PE_OPTION.FLOW4;
            var woChange = new WoChanging()
            {
                ID = Guid.NewGuid(),
                DUE_DATE = DateTime.Now,
                ECO_NO = ECONo,
                ORDER_NO = txtWO.Text.Trim(),
                UPD_DATE = DateTime.Now,
                QUANTITY = 0,
                UPDATER_NAME = txtActor.Text.Trim(),
                HOSTNAME = Environment.MachineName,
                TYPE_ID = (int)Enum.Parse(typeof(TYPE), GetTypeName()),
                TYPE_NAME = GetTypeName(),
                DEPT_NAME = DeptCode,
                DEPT_ORD = dept_ord,
                PE_OPTION = (int)peOption,
                DEPT = deptName,
                STATUS = cbbStatus.Text.Trim(),
                USER_LOGIN = User,
                REMARK = txtRemark.Text.Trim()
            };

            var result = Repository.SaveECOInSAP(woChange);
            if (!string.IsNullOrEmpty(result))
            {
                RJMessageBox.Show(result, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            result = Repository.SaveCustomByDept(woChange);
            if (string.IsNullOrEmpty(result))
            {
                RJMessageBox.Show("Thêm vào SAP thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                RJMessageBox.Show(result, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private bool ValidateTextBox()
        {
            if (cbbStatus.Text == "" || txtActor.Text == "")
            {
                RJMessageBox.Show("Không được để trống trạng thái và người thực hiện", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (deptName == "MC:")
            {
                if (string.IsNullOrEmpty(txtWO.Text))
                {
                    RJMessageBox.Show("Chưa nhập thông tin WO!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!rdAI.Checked && !rdSMT.Checked && !rdFAT.Checked && !rbRom.Checked)
                {
                    RJMessageBox.Show("Chưa chọn loại WO!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string checkWo = ValidateHelper.CheckWOValid(txtWO.Text);
                if (!string.IsNullOrEmpty(checkWo))
                {
                    RJMessageBox.Show(checkWo);
                    txtWO.SelectAll();
                    txtWO.Focus();
                    return false;
                }
            }
            if(deptName == "PE:")
            {
                if(!rbFlow2.Checked && !rbFull.Checked && !rbFlow4.Checked)
                {
                    RJMessageBox.Show("Chưa chon Flow/ Công đoạn!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        private void OnlyNumberPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtWO_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlyNumberPress(sender, e);
        }

        private void rbFull_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFull.Checked)
            {
                pbFlow.Image = global::ECO.Properties.Resources.congdoan1;
            }
            else if (rbFlow2.Checked)
            {
                pbFlow.Image = global::ECO.Properties.Resources.congdoan2;
            }
            else if (rbFlow4.Checked)
            {
                pbFlow.Image = global::ECO.Properties.Resources.congdoan4;
            }
        }
    }
}

