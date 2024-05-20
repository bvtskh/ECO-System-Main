using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Reflection;

namespace ECO
{
    class SqlHelper
    {
        public string strSql = @"Data Source=172.28.10.9;Initial Catalog=UMC3000; uid=sa;pwd=$umcevn123;";
        public string strSql1 = @"Data Source=172.28.10.9;Initial Catalog=UMCBASE; uid=sa;pwd=$umcevn123;";
        SqlConnection sqlcon, sqlcon1;
        DataSet ds;
        SqlDataAdapter ada;
        SqlCommand com, com1;
        SqlDataReader dr;

        public void KetNoi1()
        {
            sqlcon1 = new SqlConnection(strSql1);
            if (sqlcon1.State == ConnectionState.Closed)
            {
                sqlcon1.Open();
            }
        }

        public void KetNoi()
        {
            sqlcon = new SqlConnection(strSql);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
        }
        public void NgatKetNoi()
        {
            if (sqlcon.State == ConnectionState.Open)
                sqlcon.Close();
        }
        public void NgatKetNoi1()
        {
            if (sqlcon1.State == ConnectionState.Open)
                sqlcon1.Close();
        }
        public void LoadDLlenDatagridview(DataGridView dv, string tenStore, params object[] gt)
        {
            //ds = new DataSet();
            //try
            //{
                KetNoi();
                com = new SqlCommand(tenStore, sqlcon);
                com.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(com);
                for (int i = 1; i < com.Parameters.Count; ++i)
                {
                    com.Parameters[i].Value = gt[i - 1];
                }
                //com.Connection = sqlcon;
                // com.ExecuteNonQuery();
                ada = new SqlDataAdapter(com);
            ada.SelectCommand.CommandTimeout = 180;
                //ada.Fill(ds, "Bang");
                //dv.DataSource = ds;
                //dv.DataMember = "Bang";                
            DataTable tb = new DataTable();
                ada.Fill(tb);
                dv.DataSource = tb;
            
            // sqlcon.Close();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }
        public void CallStoreProcedure(string tenStore, params object[] gt)
        {
            try
            {
                KetNoi();
                com = new SqlCommand(tenStore, sqlcon);
                com.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(com);
                for (int i = 1; i < com.Parameters.Count; ++i)
                {
                    com.Parameters[i].Value = gt[i - 1];
                }
                com.Connection = sqlcon;
                com.ExecuteNonQuery();
                com.Dispose();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public string LayDLTuCombobox(string DauVao, string strSQL)
        {

            string strKetQua = "";
            KetNoi();
            com = new SqlCommand(strSQL, sqlcon);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                if (dr[0].ToString().ToLower() == DauVao.ToLower())
                    strKetQua = dr[1].ToString();
            }
            com.Dispose();
            NgatKetNoi();
            return strKetQua;
        }
        public void ThuchienSQL(string sql)
        {
            KetNoi();
            com = new SqlCommand(sql, sqlcon);
            com.ExecuteNonQuery();
            NgatKetNoi();
        }
        public int ThucHienDemBP()
        {
            int dem;
            KetNoi();
            com = new SqlCommand("Select count(*) from ENG_DEPTFLM", sqlcon);
            dem = (Int32)com.ExecuteScalar();

            NgatKetNoi();
            return dem;
        }

        public void LoadCombobox(ComboBox cb, string sql)
        {
            cb.Items.Clear();
            KetNoi();
            com = new SqlCommand(sql, sqlcon);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                //cb = new ComboBox();
                cb.Items.Add(dr[0].ToString());
            }
            NgatKetNoi();
            //cb.SelectedIndex = 0;
        }
        public bool DangNhap(string User, string bophan)
        {
            Object nsd;
            KetNoi1();

            com1 = new SqlCommand("Select USER_ID,DEPT_CODE From USRMASM where USER_ID='" + User + "' and DEPT_CODE='" + bophan + "'", sqlcon1);

            nsd = com1.ExecuteScalar();
            if (nsd == null)
            {
                return false;
            }
            else
                return true;
        }
        public bool CheckECOComfirm(string maECO, string maBP)
        {
            Object nsd;
            //string getcode = LayDLTuCombobox(maBP, "Select DEPT_NAME,DEPT_CODE from ENG_DEPTFLM where DEPT_NAME='" + maBP + "' ");
            KetNoi();
            com = new SqlCommand("Select ECO_No from  ENG_ECODETM where ECO_No='" + maECO + "' and DEPT_CODE='" + maBP + "'", sqlcon);
            nsd = com.ExecuteScalar();
            if (nsd == null)
            {
                return false;
            }
            else
                return true;
        }
        public bool CheckFlowDefault()
        {
            Object nsd;
            KetNoi();
            com = new SqlCommand("Select FlowName from FLOWECO where Flow_Default='1'",sqlcon);
            nsd = com.ExecuteScalar();
            if (nsd == null)
            {
                return false;
            }
            else
                return true;
        }


        public void loadLenListView(ListView lv, string sql)
        {
            KetNoi();
            com = new SqlCommand(sql, sqlcon);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                ListViewItem lstItem = new ListViewItem(dr[0].ToString());
                lv.Items.Add(lstItem);
            }
            NgatKetNoi();
        }
        //public void loadLenListView1(ListView lv, string sql)
        //{
        //    KetNoi();
        //    com = new SqlCommand(sql, sqlcon);
        //    dr = com.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        ListViewItem lstItem = new ListViewItem(dr[0].ToString());
        //        lstItem.SubItems.Add(dr[1].ToString());
        //        lv.Items.Add(lstItem);
        //    }
        //    NgatKetNoi();
        //}
        public void loadDLLenListView(ListView lv, string dk)
        {
            KetNoi();
            com = new SqlCommand("Select DEPT_CODE,DEPT_NAME from ENG_DEPTFLM where DEPT_NAME not in(Select DEPT_NAME from FlowECODEPT  where ECODEPT='" + dk + "')", sqlcon);
            dr = com.ExecuteReader();
            //listView1.Items.Clear();
            while (dr.Read())
            {
                ListViewItem lstItem = new ListViewItem(dr["DEPT_CODE"].ToString());
                lstItem.SubItems.Add(dr["DEPT_NAME"].ToString());
                lv.Items.Add(lstItem);
            }
            NgatKetNoi();
        }

        public void loadDLLenListView1(ListView lv, string dk)
        {
            KetNoi();
            com = new SqlCommand("Select FlowECODept.ECODEPT, ENG_DEPTFLM.DEPT_NAME from FlowECODept, ENG_DEPTFLM where FlowECODept.FlowECO_Name='" + dk + "' and FlowECODept.ECODEPT=ENG_DEPTFLM.DEPT_CODE Order by Squence ", sqlcon);
            dr = com.ExecuteReader();

            //listView1.Items.Clear();
            while (dr.Read())
            {
                ListViewItem lstItem = new ListViewItem(dr[0].ToString());
                lstItem.SubItems.Add(dr[1].ToString());
                lv.Items.Add(lstItem);
            }
            NgatKetNoi();
        }

    }

}
