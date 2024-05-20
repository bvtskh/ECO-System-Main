using CommonProject.Entities;
using ECO.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECO.Busisness
{
    public class Repository
    {
        public static WoChanging GetWO(string ecoNo)
        {
            using (DAL.DataContext context = new DAL.DataContext())
            {
                var exist = context.WoChangings.FirstOrDefault(r => r.ECO_NO == ecoNo);
                if (exist != null)
                {
                    return exist;
                }
                return null;
            }
        }
        public static string SaveCustomByDept(WoChanging wo)
        {
            using (DAL.DataContext context = new DAL.DataContext())
            {
                if (wo.DEPT_NAME == "MC")
                {
                    var exist = context.WoChangings.FirstOrDefault(r => r.ECO_NO == wo.ECO_NO);
                    if (exist == null)
                    {
                        context.WoChangings.Add(wo);
                    }
                    else
                    {
                        exist.DUE_DATE = wo.DUE_DATE;
                        exist.ORDER_NO = wo.ORDER_NO;
                        exist.UPD_DATE = wo.UPD_DATE;
                        exist.UPDATER_NAME = wo.UPDATER_NAME;
                        exist.HOSTNAME = Environment.MachineName;
                        exist.DEPT_NAME = wo.DEPT_NAME;
                        exist.DEPT_ORD = wo.DEPT_ORD;
                        exist.PE_OPTION = wo.PE_OPTION;
                    }
                    context.SaveChanges();
                }

                if (wo.DEPT_NAME == "PE")
                {
                    if (wo.PE_OPTION == 0) return "Chưa chọn công đoạn!";
                    if (wo.PE_OPTION == (int)PE_OPTION.EDIT)
                    {
                        wo.UPDATER_NAME = "System";
                        wo.DEPT = "PL:";
                        var result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                        wo.DEPT = "IQC:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    else if (wo.PE_OPTION == (int)PE_OPTION.FLOW3)
                    {
                        wo.UPDATER_NAME = "System";
                        wo.DEPT = "PL:";
                        var result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        wo.DEPT = "IQC:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        wo.DEPT = "SMT:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                    else if (wo.PE_OPTION == (int)PE_OPTION.FLOW4)
                    {
                        wo.UPDATER_NAME = "System";
                        wo.DEPT = "PL:";
                        var result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        wo.DEPT = "IQC:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        wo.DEPT = "MC:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        wo.DEPT = "SMT:";
                        result = SaveECOInSAP(wo);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }

                        
                    }

                }
                return "";
            }

        }

        public static string UpdateWO(WoChanging entity)
        {
            using (DAL.DataContext context = new DAL.DataContext())
            {
                if (entity.DEPT_NAME == "SMT")
                {
                    var exist = context.WoChangings.FirstOrDefault(r => r.ECO_NO == entity.ECO_NO);
                    if (exist == null) return "MC chưa nhập Wo cho ECO này";
                    context.Entry(exist).State = EntityState.Modified;
                    exist.ORDER_NO = entity.ORDER_NO;
                    exist.UPD_DATE = DateTime.Now;
                    context.SaveChanges();
                    return "Lưu thành công!";
                }
                return $"Đang ECO ở {entity.DEPT_NAME}.Không thể thực hiện chức năng này";

            }
        }
        public static string SaveECOInSAP(WoChanging wo)
        {
            var db = new SqlHelper();
            ComboBox cb = new ComboBox();
            ComboBox cb1 = new ComboBox();
            string sott = db.LayDLTuCombobox(wo.DEPT, "Select DEPT_NAME, ord_no from ENG_DEPTFLM where DEPT_NAME = '" + wo.DEPT + "' ");
            // Lấy tên bộ phận mà có số thứ tự nhỏ hơn bộ phận đang đứng
            db.LoadCombobox(cb, "Select DEPT_CODE from ENG_DEPTFLM where Ord_no<'" + int.Parse(sott) + "' order by Ord_no ");
            // Lấy tên bộ phận tiếp theo của bộ phận đang đứng
            db.LoadCombobox(cb1, "Select DEPT_CODE from ENG_DEPTFLM where Ord_no>'" + int.Parse(sott) + "' order by Ord_no ");
            var DeptCode = db.LayDLTuCombobox(wo.DEPT, "Select DEPT_NAME, DEPT_CODE from ENG_DEPTFLM where DEPT_NAME = '" + wo.DEPT + "' ");

            // Kiem tra neu chua Xác nhận
            if (db.CheckECOComfirm(wo.ECO_NO, DeptCode) == false)
            {
                // Nếu đằng trước nó đã có thằng xác nhận thì kiểm tra về ngày xác nhận và thứ tự bộ phận xác nhận
                if (cb.Items.Count > 0)
                {
                    for (int i = 0; i < cb.Items.Count; ++i)
                    {
                        if (db.CheckECOComfirm(wo.ECO_NO, cb.Items[i].ToString()) == false)
                        {
                            return "Bộ phận " + cb.Items[i].ToString() + " chưa xác nhận ECO này. Vì thế bạn không thể thực hiện xác nhận trước được";
                        }
                        else
                        {
                            //deptcode = cl.LayDLTuCombobox(cb.Items[i].ToString(), "Select DEPT_NAME, DEPT_CODE from ENG_DEPTFLM where DEPT_NAME = '" + cb.Items[i].ToString() + "'");
                            var DateConfirm = db.LayDLTuCombobox(wo.ECO_NO, "Select ECO_NO,EXE_DATE from  ENG_ECODETM where ECO_NO='" + wo.ECO_NO + "' and DEPT_CODE ='" + cb.Items[i].ToString() + "'");
                            if (wo.DUE_DATE < Convert.ToDateTime(DateConfirm))
                            {
                                return "Ngày thực hiện ECO không thể nhỏ hơn ngày thực hiện ECO của bộ phận " + cb.Items[i].ToString();
                            }
                            else
                            {
                                if (i == cb.Items.Count - 1)
                                {
                                    db.CallStoreProcedure("Add_ECO_Comfirm"
                                        , wo.ECO_NO
                                        , DeptCode
                                        , wo.STATUS
                                        , wo.DUE_DATE.ToShortDateString()
                                        , wo.UPDATER_NAME
                                        , wo.REMARK
                                        , wo.USER_LOGIN
                                        , DateTime.Now);
                                    return "";
                                }
                            }

                        }
                    }
                }
                else
                {
                    // Là bộ phận đầu tiên comfirm
                    db.CallStoreProcedure("Add_ECO_Comfirm"
                        , wo.ECO_NO
                        , DeptCode
                        , wo.STATUS
                        , wo.DUE_DATE.ToShortDateString()
                        , wo.UPDATER_NAME
                        , wo.REMARK
                        , wo.USER_LOGIN
                        , DateTime.Now);
                    return "";
                }
            }
            //Nếu bộ phận đã được xác nhận rồi, Kiểm tra bộ phận đằng sau xác nhận Chưa? Nếu chưa xác nhận thì cho UPdate tình trạng xác nhận
            else
            {
                if (cb1.Items.Count > 0)
                {
                    if (db.CheckECOComfirm(wo.ECO_NO, cb1.Items[0].ToString()) == false)
                    {
                        // gọi update
                        if (cb.Items.Count > 0)
                        {
                            if (db.CheckECOComfirm(wo.ECO_NO, (cb.Items[cb.Items.Count - 1]).ToString()) == true)
                            {
                                var DateConfirm = db.LayDLTuCombobox(wo.ECO_NO
                                    , "Select ECO_NO,EXE_DATE from  ENG_ECODETM where ECO_NO='" + wo.ECO_NO + "' and DEPT_CODE ='" + cb.Items[cb.Items.Count - 1].ToString() + "'");
                                if (wo.DUE_DATE < Convert.ToDateTime(DateConfirm))
                                {
                                    return "Ngày thực hiện ECO không thể nhỏ hơn ngày thực hiện ECO của bộ phận " + (cb.Items[cb.Items.Count - 1]).ToString();
                                }
                                else
                                {
                                    db.CallStoreProcedure("UpdateECO_Confirm"
                                        , wo.STATUS
                                        , wo.DUE_DATE.ToShortDateString()
                                        , wo.UPDATER_NAME
                                        , wo.REMARK
                                        , wo.USER_LOGIN
                                        , DateTime.Now
                                        , wo.ECO_NO
                                        , DeptCode);
                                    return "";
                                }
                            }
                            else
                            {
                                return "Bộ phận " + DeptCode + " đã xác nhận ECO này.";
                            }

                        }
                        if (cb.Items.Count == 0)
                        {
                            db.CallStoreProcedure("UpdateECO_Confirm"
                                , wo.STATUS
                                , wo.DUE_DATE.ToShortDateString()
                                , wo.UPDATER_NAME
                                , wo.REMARK
                                , wo.USER_LOGIN
                                , DateTime.Now
                                , wo.ECO_NO
                                , DeptCode);
                            return "";
                        }

                    }
                    else
                    {
                        return "Bộ phận " + cb1.Items[0].ToString() + " đã xác nhận ECO này. Bạn không thể chỉnh sửa được";
                    }
                }
            }

           
            return "";
        }

    }
}
