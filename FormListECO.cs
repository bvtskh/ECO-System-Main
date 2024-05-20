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
using CommonProject.MsgBox_AQ;

namespace ECO
{
    public partial class FormListECO : Form
    {
        public string eco;
        SqlHelper cl = new SqlHelper();
        DateTime giohientai = System.DateTime.Now;
        TimeSpan t = TimeSpan.Parse(7.ToString());
        ComboBox cb = new ComboBox();
        public string getBP = null;
        public string GetUser = null;
        public FormListECO()
        {
            InitializeComponent();
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cb.Items.Clear();
            cl.LoadCombobox(cb, "Select DEPT_NAME from  ENG_DEPTFLM order by ORD_NO");
            dateTimePicker1.Value = giohientai - t;
            dateTimePicker2.Value = giohientai;
            Setlayout();
            dataGridView1.DoubleBuffered(true);
            CheckRule();
            if (!string.IsNullOrEmpty(this.eco))
            {
                txbECO.Text = this.eco;
                SearchClick(null, null);
            }
        }
        private void CheckRule()
        {
            if (Constants.ROLE == ROLE.VIEW_ONLY)
            {
                departmentComfirmToolStripMenuItem.Enabled = false;
            }
            if (GetUser == "SMT")
            {
                sửaWOToolStripMenuItem.Enabled = true;
            }
        }
        private void Setlayout()
        {
            dataGridView1.CellPainting -= dataGridView1_CellPainting;
            dataGridView1.CellPainting += dataGridView1_CellPainting;
            dataGridView1.Paint -= dataGridView1_Paint;
            dataGridView1.Paint += dataGridView1_Paint;
            dataGridView1.Scroll -= dataGridView1_Scroll;
            dataGridView1.Scroll += dataGridView1_Scroll;
            dataGridView1.ColumnWidthChanged -= dataGridView1_ColumnWidthChanged;
            dataGridView1.ColumnWidthChanged += dataGridView1_ColumnWidthChanged;

            DateTime gt1 = DateTime.Parse(dateTimePicker1.Value.ToShortDateString());
            DateTime gt2 = DateTime.Parse(dateTimePicker2.Value.ToShortDateString());
            cl.LoadDLlenDatagridview(dataGridView1, "DX_ShowReportECO", txbECO.Text, txbModel.Text, gt1, gt2);
            dataGridView1.Columns["ECO NO"].Frozen = true;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                Rectangle r2 = e.CellBounds;
                r2.Y += e.CellBounds.Height / 2;
                r2.Height = e.CellBounds.Height / 2;
                e.PaintBackground(r2, true);
                e.PaintContent(r2);
                e.Handled = true;
            }
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Rectangle r2 = new Rectangle(); int w2 = 0;

                int j = 0;
                int k = 0;
                for (int i = 11; i < 64;)
                {
                    Rectangle r1 = dataGridView1.GetCellDisplayRectangle(i, -1, true);
                    w2 = dataGridView1.GetCellDisplayRectangle(i + 1, -1, true).Width;
                    r1.X += 1;
                    r1.Y += 1;
                    r1.Width = r1.Width + w2 - 2;
                    r1.Height = r1.Height / 2 - 2;
                    e.Graphics.FillRectangle(new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.BackColor), r1);
                    e.Graphics.DrawLine(new Pen(dataGridView1.GridColor, 1), r1.X, r1.Bottom, r1.X + r1.Width, r1.Bottom);

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    // i += 4;
                    e.Graphics.DrawString(cb.Items[k].ToString(), dataGridView1.ColumnHeadersDefaultCellStyle.Font, new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);

                    i += 4;
                    k += 1;
                }

                r2 = dataGridView1.GetCellDisplayRectangle(0, -1, true);
                for (j = 0; j < 10; j++)
                {
                    w2 = dataGridView1.GetCellDisplayRectangle(j + 1, -1, true).Width;
                    r2.Width = r2.Width + w2;
                }
                r2.X += 1;
                r2.Y += 1;
                r2.Width = r2.Width - 2;
                r2.Height = r2.Height / 2 - 2;
                e.Graphics.FillRectangle(new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.BackColor), r2);
                e.Graphics.DrawLine(new Pen(dataGridView1.GridColor, 1), r2.X, r2.Bottom, r2.X + r2.Width, r2.Bottom);
                StringFormat formats = new StringFormat();
                formats.Alignment = StringAlignment.Center;
                formats.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("ECO Information", dataGridView1.ColumnHeadersDefaultCellStyle.Font, new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor), r2, formats);

            }
            catch (Exception ex)
            {

                //throw;
            }

        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle rtHeader = dataGridView1.DisplayRectangle;
            rtHeader.Height = dataGridView1.ColumnHeadersHeight / 2;
            dataGridView1.Invalidate(rtHeader);
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            Rectangle rtHeader = dataGridView1.DisplayRectangle;
            rtHeader.Height = dataGridView1.ColumnHeadersHeight / 2;
            dataGridView1.Invalidate(rtHeader);
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.Resizable = DataGridViewTriState.True;
            }
        }
        int currentCellIndex = 0;
        int currentRowIndex = 0;

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    currentCellIndex = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
                    currentRowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                    contextMenuStrip1.Show(Cursor.Position );
                    dataGridView1.CurrentCell = dataGridView1.Rows[currentRowIndex].Cells[currentCellIndex];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        private void departmentComfirmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int hangchon = dataGridView1.CurrentRow.Index;
            string maECO = dataGridView1[0, hangchon].Value.ToString();
            this.TopMost = false;
            Comfirm cf = new Comfirm();
            cf.deptName = getBP;
            cf.ECONo = maECO;
            cf.User = GetUser;
            cf.ShowDialog();
            //if(cf.ShowDialog()==DialogResult.Cancel)
            // {
            Setlayout();
            // }
        }


        private void SearchClick(object sender, EventArgs e)
        {
            //tim kiếm theo thời gian
            // cl.LoadDLlenDatagridview(dataGridView1, "ShowReportECO", textBox1.Text, textBox2.Text, gt1, gt2);
            if (txbECO.Text == "" && txbModel.Text == "")
            {
                Setlayout();
            }
            else
            {
                if (txbECO.Text != "" && txbModel.Text == "")
                {
                    cl.LoadDLlenDatagridview(dataGridView1, "DX_ShowReportECO_No", txbECO.Text, txbModel.Text);
                }
                if (txbECO.Text == "" && txbModel.Text != "")
                {
                    cl.LoadDLlenDatagridview(dataGridView1, "DX_ShowECOReport_Model", txbModel.Text);
                }
                if (txbECO.Text != "" && txbModel.Text != "")
                {
                    cl.LoadDLlenDatagridview(dataGridView1, "DX_ShowECOReportNO_Model", txbECO.Text, txbModel.Text);
                }
                try
                {
                    dataGridView1.Columns["ECO NO"].Frozen = true;
                }
                catch (Exception ex)
                {

                }
                
            }
            this.TopMost = false;
        }

        private void sửaWOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int hangchon = dataGridView1.CurrentRow.Index;
            string maECO = dataGridView1[0, hangchon].Value.ToString();
            var eco = Repository.GetWO(maECO);
            if (eco == null)
            {
                RJMessageBox.Show($"Chưa có Wo cho ECO {maECO}"
                    , "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            new FormEditWO(eco).ShowDialog();
        }
    }
}
