using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMonitorApplication.FunctionClass
{
    public class DataBase
    {
        /// <summary>
        /// 服务器
        /// </summary>
        string m_Server;
        /// <summary>
        /// 数据库
        /// </summary>
        string m_DatabaseName;
        /// <summary>
        /// 用户ID
        /// </summary>
        string m_userID;
        /// <summary>
        /// 用户密码
        /// </summary>
        string m_userPW;

        #region 初始化数据库、连接数据库
        /// <summary>
        /// 初始化数据库链接设置
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="dataBaseName">数据库名称</param>
        /// <param name="userID">用户ID</param>
        /// <param name="userPW">用户密码</param>
        public void InitDB(string server, string dataBaseName, string userID, string userPW)
        {
            m_Server = server;
            m_DatabaseName = dataBaseName;
            m_userID = userID;
            m_userPW = userPW;
        }

        /// <summary>
        /// 编写连接数据库SQL语言
        /// </summary>
        /// <returns>连接数据库SQL语言</returns>
        public string sql_conn()
        {
            string strSql;         // 返回的连接数据库SQL语言
            strSql = "Data Source=" + m_Server + ";Initial Catalog=" + m_DatabaseName + ";Persist Security Info=True;User ID=" +
                        m_userID + ";Password=" + m_userPW + ";pooling = false";
            return strSql;
        }
        #endregion

        #region 注册登录相关操作
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="sUserID">登录名</param>
        /// <param name="sUserPW">密码</param>
        /// <param name="ModifyPermission">修改信息权限</param>
        /// <returns></returns>
        public bool LoginCheck(string sUserID, string sUserPW, ref int bModifyPermission, ref string sUserPhoto)
        {
            string sSQL, sID, sPW;
            SqlConnection conn;

            //1、连接数据库
            sSQL = sql_conn();//编写连接数据库SQL语言
            conn = new SqlConnection(sSQL);// 初始化连接数据库
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("Worker_DB连接数据库出错：" + e.Message);
                return false;
            }

            //2、执行操作
            try
            {
                sID = sUserID;
                sSQL = String.Format("Select * from DB_User where userID like '{0}'", sID);  //SQL查找语句
                // 通过DataAdapter对象将数据库中计量员的全部信息提取到DataSet中构成虚拟表格tempTable，然后再从tempTable中提取对应的密码，最后判断是否正确。
                // 生成DataAdapter对象，把数据库中的数据通过DataAdapter对象填充DataSet。
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sSQL, conn);
                // 生成包含数据表、行、列的DataSet，可以把DataSet当成内存中的数据库，DataSet是不依赖于数据库的独立数据集合。
                DataSet thisDataSet = new DataSet();
                // Fill DataSet using query defined previously for DataAdapter
                thisAdapter.Fill(thisDataSet, "tempTable"); //temp为自定义虚拟表名

                if (thisDataSet.Tables["tempTable"].Rows.Count < 1)
                {
                    return false;
                }
                else
                {
                    sPW = thisDataSet.Tables["tempTable"].Rows[0]["userPW"].ToString();
                    if (sPW == sUserPW)
                    {
                        bModifyPermission = Convert.ToInt32(thisDataSet.Tables["tempTable"].Rows[0]["ModifyPermission"]);
                        //sUserPhoto = Convert.ToString(thisDataSet.Tables["tempTable"].Rows[0]["UserPhoto"]);

                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("WorkerLoginConfrm函数出错：" + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 主界面中数据库操作
        /// <summary>
        /// 表格数据
        /// </summary>
        /// <param name="FIDs"></param>
        /// <param name="trainNames"></param>
        /// <param name="trainCounts"></param>
        /// <param name="volumns"></param>
        /// <param name="JZs"></param>
        /// <param name="PWs"></param>
        /// <param name="measureTimes"></param>
        /// <returns></returns>
        public bool SearchforList(ref List<string> FIDs, ref List<string> trainNames, ref List<string> trainCounts, ref List<double> volumns, ref List<double> JZs, ref List<double> PWs, ref List<string> measureTimes)
        {
            //查询最新10条数据
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            string request = string.Format("select top(10) [FID], [trainName],[trainCount],[volumn],[JZ],[PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 order by MeasureTime DESC");
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("SearchforList链接数据库出错：" + e.Message);
                return false;
            }

            try
            {
                //执行查询
                SqlDataAdapter thisAdapter = new SqlDataAdapter(request, conn);
                // 生成包含数据表、行、列的DataSet，可以把DataSet当成内存中的数据库，DataSet是不依赖于数据库的独立数据集合。
                DataSet thisDataSet = new DataSet();
                // Fill DataSet using query defined previously for DataAdapter
                thisAdapter.Fill(thisDataSet, "tempTable"); //temp为自定义虚拟表名
                int RowCount = thisDataSet.Tables["tempTable"].Rows.Count;

                //若没有查询到符合要求的数据则直接返回一个false
                if (RowCount < 1)
                    return false;
                for (int i = 0; i < RowCount; i++)
                {
                    //FID
                    string fID = thisDataSet.Tables["tempTable"].Rows[i]["FID"].ToString();
                    FIDs.Add(fID);
                    //列车名
                    string name = thisDataSet.Tables["tempTable"].Rows[i]["trainName"].ToString();
                    trainNames.Add(name);
                    //列车车厢数
                    string count = thisDataSet.Tables["tempTable"].Rows[i]["trainCount"].ToString();
                    trainCounts.Add(count);
                    //体积
                    double volumn = Convert.ToDouble(thisDataSet.Tables["tempTable"].Rows[i]["volumn"].ToString());
                    volumns.Add(volumn);
                    //净重
                    double jz = Convert.ToDouble(thisDataSet.Tables["tempTable"].Rows[i]["JZ"].ToString());
                    JZs.Add(jz);
                    //品位值
                    double pw = Convert.ToDouble(thisDataSet.Tables["tempTable"].Rows[i]["PW"].ToString());
                    PWs.Add(pw);
                    //时间
                    string time = thisDataSet.Tables["tempTable"].Rows[i]["MeasureTime"].ToString().Split(' ')[1];
                    measureTimes.Add(time);
                }

                return true;
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("SearchforList出错：" + e.Message);
                return false;
            }
            finally
            {
                //conn.Close();
                //conn.Dispose();
            }
        }

        /// <summary>
        /// 搜索指定日期的品位数据,返回折线图数据
        /// </summary>
        /// <param name="day">具体指定的日期</param>
        /// <param name="X">时间:时分秒</param>
        /// <param name="Y">品位值</param>
        /// <returns>查询是否成功</returns>
        public bool SearchforChart(DateTime day, ref List<string> X, ref List<double> Y, int type)
        {
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            string request = "";
            if (type == 0)//当日
                request = string.Format("select [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 and [measureTime]>'{0}' and [measureTime]<'{1}'", day.ToString("yyyy-MM-dd"), day.AddDays(1).ToString("yyyy-MM-dd"));
            else if (type == 1)//最近一日
                request = string.Format("select TOP(1) [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 order by [measureTime] desc");
            else if (type == 2)//本周
            {
                DateTime left, right;
                left = day.AddDays((Convert.ToInt32(day.DayOfWeek) - 1) % 7);
                right = left.AddDays(7);
                request = string.Format("select [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 and [measureTime]>'{0}' and [measureTime]<'{1}'", left.ToString("yyyy-MM-dd"), right.ToString("yyyy-MM-dd"));
            }
            else if (type == 3)//本月
            {
                request = string.Format("select [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 and [measureTime]>'{0}' and [measureTime]<'{1}'", day.ToString("yyyy-MM") + "-01", day.AddMonths(1).ToString("yyyy-MM") + "-01");
            }
            else if (type == 4)//本年
            {
                request = string.Format("select [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 and [measureTime]>'{0}' and [measureTime]<'{1}'", day.ToString("yyyy") + "-01-01", day.AddYears(1).ToString("yyyy") + "-01-01");
            }
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("SearchforChart链接数据库出错：" + e.Message);
                return (false);
            }

            try
            {
                //执行查询
                SqlDataAdapter thisAdapter = new SqlDataAdapter(request, conn);
                // 生成包含数据表、行、列的DataSet，可以把DataSet当成内存中的数据库，DataSet是不依赖于数据库的独立数据集合。
                DataSet thisDataSet = new DataSet();
                // Fill DataSet using query defined previously for DataAdapter
                thisAdapter.Fill(thisDataSet, "tempTable"); //temp为自定义虚拟表名

                int RowCount = thisDataSet.Tables["tempTable"].Rows.Count;
                //若没有查询到符合要求的数据则直接返回一个false
                if (RowCount < 1)
                {
                    X.Add("0 0");
                    Y.Add(45);
                    return (false);

                }
                if (type == 1)//对搜索最近日期的记录单独处理,进行二次查询,然后替换掉第一次的查询结果
                {
                    string time = thisDataSet.Tables["tempTable"].Rows[0]["MeasureTime"].ToString().Split(' ')[0];
                    request = string.Format("select [PW],[MeasureTime] from DB_TrainMeasureResult where WB = 2 and [measureTime]>'{0}' ", time);
                    thisAdapter = new SqlDataAdapter(request, conn);
                    thisDataSet = new DataSet();
                    thisAdapter.Fill(thisDataSet, "tempTable");
                    RowCount = thisDataSet.Tables["tempTable"].Rows.Count;
                }
                for (int i = 0; i < RowCount; i++)
                {
                    //写品位值
                    double pw = Convert.ToDouble(thisDataSet.Tables["tempTable"].Rows[i]["PW"].ToString());
                    Y.Add(pw);
                    //写时间
                    string time = thisDataSet.Tables["tempTable"].Rows[i]["MeasureTime"].ToString();
                    X.Add(time);
                }
                return (true);
            }
            catch (Exception e)
            {
                //出错回滚
                Function.WriteErrorLog("SearchforChart出错：" + e.Message);
                return (false);
            }
            finally
            {
                conn.Close();
            }

        }

        #endregion

        #region 统计报表中数据库操作
        /// <summary>
        /// 获取某天数据
        /// </summary>
        /// <param name="prjrcdlst"></param>
        /// <returns></returns>
        public bool GetDayData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult, DateTime dayTime)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("GetDayData连接数据库出错：" + e.Message);
                return false;
            }

            //依据条件获取数据库数据
            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where WB = 2 and datediff(day,measureTime,'{0}')=0", dayTime);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");
                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn= Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetDayData出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 获取某周数据
        /// </summary>
        /// <param name="prjrcdlst"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public bool GetWeekData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult, int weekIndex)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("GetMeasPrjRecordHistory连接数据库出错：" + e.Message);
                return false;
            }
            DateTime yearFirstDay = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime weekTime = yearFirstDay.AddDays((weekIndex - 1) * 7);
            DateTime startTime = weekTime.AddDays(-(Convert.ToInt32(weekTime.DayOfWeek)-1)%7);
            DateTime endTime = startTime.AddDays(7);

            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where WB = 2 and [measureTime] between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", startTime, endTime);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");

                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetMeasPrjRecordHistory连接数据库出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 获取某月数据
        /// </summary>
        /// <param name="prjrcdlst"></param>
        /// <returns></returns>
        public bool GetMonthData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult, DateTime dayTime)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("GetMonthData连接数据库出错：" + e.Message);
                return false;
            }

            //依据条件获取数据库数据
            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where WB = 2 and datediff(month,measureTime,'{0}')=0", dayTime);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");
                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetMonthData出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 获取某年数据
        /// </summary>
        /// <param name="prjrcdlst"></param>
        /// <returns></returns>
        public bool GetYearData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult, DateTime dayTime)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("GetYearData连接数据库出错：" + e.Message);
                return false;
            }

            //依据条件获取数据库数据
            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where WB = 2 and datediff(year,measureTime,'{0}')=0", dayTime);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");
                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetYearData出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 依据起始时间获取历史数据
        /// </summary>
        /// <param name="prjrcdlst"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public bool GetHistoryData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult, DateTime startTime, DateTime endTime)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("GetMeasPrjRecordHistory连接数据库出错：" + e.Message);
                return false;
            }

            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where WB = 2 and [measureTime] between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", startTime, endTime);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");

                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetMeasPrjRecordHistory连接数据库出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 重载1：获取所有的错误信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool GetErrorData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("GetErrorData连接数据库出错：" + e.Message);
                return false;
            }

            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where [WB] != 2 ");
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");

                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetErrorData连接数据库出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 重载2：获取指定时间段的错误数据信息
        /// </summary>
        /// <param name="list_TrainMeasureResult"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool GetErrorData(ref List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult,DateTime begin,DateTime end)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("GetErrorData连接数据库出错：" + e.Message);
                return false;
            }

            try
            {
                list_TrainMeasureResult.Clear();
                sql = String.Format("select * from DB_TrainMeasureResult where [WB] != 2 and [measureTime] between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'", begin, end);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");

                //返回各列的值
                foreach (DataRow theRow in thisDataSet.Tables["tempTable"].Rows)
                {
                    DB_TrainMeasureResult_DataStruct Temp = new DB_TrainMeasureResult_DataStruct();
                    Temp.sFID = theRow["FID"].ToString();
                    Temp.sTrainName = theRow["trainName"].ToString();
                    Temp.nTrainCount = Convert.ToInt32(theRow["trainCount"]);
                    Temp.dVolumn = Convert.ToDouble(theRow["volumn"]);
                    Temp.dMZ = Convert.ToDouble(theRow["MZ"]);
                    Temp.dJZ = Convert.ToDouble(theRow["JZ"]);
                    Temp.dBZ = Convert.ToDouble(theRow["BZ"]);
                    Temp.dPW = Convert.ToDouble(theRow["PW"]);
                    Temp.sLY = theRow["LY"].ToString();
                    Temp.dSpeed = Convert.ToDouble(theRow["speed"]);
                    Temp.dtMeasureTime = (DateTime)theRow["measureTime"];
                    Temp.intWB = Convert.ToInt32(theRow["WB"]);

                    list_TrainMeasureResult.Add(Temp);

                }
                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetErrorData连接数据库出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 存储数据
        /// <summary>
        /// 添加整列车测量结果
        /// </summary>
        /// <param name="time">体积测量完成时间</param>
        /// <param name="count">车厢数</param>
        /// <param name="volumn">体积和</param>
        /// <param name="weight">重量和</param>
        /// <param name="pw">品位值</param>
        /// <param name="speed">整列平均速度</param>
        /// <param name="fin">数据状态标志</param>
        /// <returns>是否成功</returns>
        public bool AddTrainResult(DateTime time, int count, double volumn, double weight, double pw, double speed, int fin)
        {
            string FID = time.ToString("yyyyMMddHHmmss");
            string measureTime = time.ToString();
            string TrainName = "H001";
            string LY = "G884";
            double JZ = weight - 12.59;
            double BZ = JZ / volumn;
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("AddTrainResult连接数据库出错：" + e.Message);
                return false;
            }
            SqlTransaction tx = conn.BeginTransaction();    ////创建一个事物处理
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                sql = string.Format("INSERT INTO [dbo].[DB_TrainMeasureResult] VALUES ('{0}' ,'{1}' ,{2} ,{3} ,{4} ,{5} ,{6} ,{7} ,'{8}' ,{9} ,'{10}' ,{11})", FID, TrainName, count, volumn, weight, JZ, BZ, pw, LY, speed, measureTime, fin);
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                tx.Commit();
                return (true);
            }
            catch (Exception e)
            {
                tx.Rollback();
                Function.WriteErrorLog("AddTrainResult出错：" + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool AddCarResult(DateTime time,double volumn,double MZ,double speed)
        {
            string FID = time.ToString("yyyyMMddHHmmss");
            double JZ = MZ - 12.59;
            double BZ = JZ / volumn;
            volumn -= 7.0;
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("AddCarResult连接数据库出错：" + e.Message);
                return false;
            }
            SqlTransaction tx = conn.BeginTransaction();    ////创建一个事物处理
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                sql = string.Format("INSERT INTO [dbo].[DB_CarMeasureResult] VALUES ('{0}' ,{1} ,{2} ,{3} ,{4} ,{5} )", FID, volumn, MZ, JZ, BZ, speed);
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                tx.Commit();
                return (true);
            }
            catch (Exception e)
            {
                tx.Rollback();
                Function.WriteErrorLog("AddCarResult出错：" + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region (新增)RFID与车辆信息管理
        /// <summary>
        /// 根据RFID卡号查找对应的车辆名称
        /// </summary>
        /// <param name="RFID">RFID号，要求取substring(12,36)部分</param>
        /// <param name="trainName">返回的列车名称</param>
        /// <returns></returns>
        public bool GetTrainNameFromRFID(string RFID, ref string trainName)
        {
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("GetTrainNameFromRFID连接数据库出错：" + e.Message);
                return false;
            }
            //依据条件获取数据库数据
            try
            {

                sql = String.Format("SELECT [trainName] FROM [DB_MengKuGradeMonitor].[dbo].[DB_RFID2Train] where [RFID]='{0}'", RFID);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");
                //返回各列的值
                if (thisDataSet.Tables["tempTable"].Rows[0]["trainName"].ToString() != "")
                    trainName = thisDataSet.Tables["tempTable"].Rows[0]["trainName"].ToString();
                else
                    Function.WriteErrorLog("RFID 卡号查询失败，ID:" + RFID);

                return true;
            }
            catch (Exception ex)
            {
                Function.WriteErrorLog("GetDayData出错：" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 检查此RFID是否已存在于数据库中
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        public string CheckRFID(string rfid)
        {
            string name = "";
            string R = "";
            for (int pr = 0; pr < rfid.Length / 2; pr++)
                R += rfid.Substring(pr * 2, 2) + " ";
            //连接数据库
            string sql;
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("CheckRFID连接数据库出错：" + e.Message);
                return name;
            }

            try
            {
                sql = String.Format("select * from DB_RFID2Train where [RFID]='{0}'",R);
                SqlDataAdapter thisAdapter = new SqlDataAdapter(sql, conn);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "tempTable");
                int RowCount = thisDataSet.Tables["tempTable"].Rows.Count;
                if (RowCount < 1)
                    return name;
                else
                {
                    name= thisDataSet.Tables["tempTable"].Rows[0]["trainName"].ToString();
                    return name;
                }

            }
            catch (Exception e)
            {
                //出错回滚
                Function.WriteErrorLog("CheckRFID出错：" + e.Message);
                return ("");
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 添加车辆与RFID信息
        /// </summary>
        /// <param name="rfid">来自小读卡器的12字节16进制数，合计24位</param>
        /// <param name="name">列车名称</param>
        /// <returns></returns>
        public bool AddRFID(string rfid,string name)
        {
            string sql;
            string R = "";
            for (int pr = 0; pr < rfid.Length / 2; pr++)
                R += rfid.Substring(pr * 2, 2) + " ";
            SqlConnection conn;
            sql = sql_conn();
            conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("连接数据库出错！");
                Function.WriteErrorLog("AddRFID连接数据库出错：" + e.Message);
                return false;
            }

            SqlTransaction tx = conn.BeginTransaction();    ////创建一个事物处理
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                sql = string.Format("INSERT INTO [dbo].[DB_RFID2Train] VALUES ('{0}' ,'{1}' )", R,name);
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                tx.Commit();
                return (true);
            }
            catch (Exception e)
            {
                tx.Rollback();
                Function.WriteErrorLog("AddRFID出错：" + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region (新增)用户管理操作
        /// <summary>
        /// 查询所有权限为0的用户ID
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUser()
        {
            List<string> names = new List<string>();
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("GetAllUser链接数据库出错:"+e.Message);
                MessageBox.Show("链接数据库出错");
            }
            try
            {
                sql = "select [UserID] from [dbo].[DB_User] where [ModifyPermission]=0";
                SqlDataAdapter adapter = new SqlDataAdapter(sql,conn);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "tempTable");
                foreach(DataRow row in dataset.Tables["tempTable"].Rows)
                {
                    names.Add(row["UserID"].ToString());
                }
                return (names);
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("GetAllUser执行出错:"+e.Message);
                MessageBox.Show("查询数据库出错");
                return (names);
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 检查用户名是否已存在
        /// </summary>
        /// <returns>true为存在,false为不存在</returns>
        public bool CheckUser(string ID)
        {
            
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("CheckUser链接数据库出错:" + e.Message);
                MessageBox.Show("链接数据库出错");
            }
            try
            {
                sql = $"select * from [dbo].[DB_User] where [UserID]='{ID}'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "tempTable");
                if ( dataset.Tables["tempTable"].Rows.Count>0)
                {
                    return(true);
                }
                return (false);
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("CheckUser执行出错:" + e.Message);
                MessageBox.Show("查询数据库出错");
                return (true);
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public bool AddUser(string ID,string pw)
        {
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("AddUser链接数据库出错:"+e.Message);
                MessageBox.Show("链接数据库出错");
            }
            SqlCommand command = conn.CreateCommand();
            SqlTransaction tx = conn.BeginTransaction();
            try
            {
                command.CommandText = $"insert into [dbo].[DB_User] values('{ID}','{ID}','{pw}',0)";
                command.Transaction = tx;
                command.ExecuteNonQuery();
                tx.Commit();
                return (true);
            }
            catch(Exception e)
            {
                tx.Rollback();
                Function.WriteErrorLog("AddUser写入数据库出错:" + e.Message);
                return (false);
            }
            finally
            {
                conn.Close();
            }
        }

        public bool ChangePW(string ID,string pw)
        {
            string sql = sql_conn();
            SqlConnection conn = new SqlConnection(sql);
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("ChangePW链接数据库出错:" + e.Message);
                MessageBox.Show("链接数据库出错！");
            }
            SqlTransaction tx = conn.BeginTransaction();
            SqlCommand comm = conn.CreateCommand();
            try
            {
                comm.CommandText = $"update [dbo].[DB_User] set [UserPW]='{pw}' where [UserID]={ID}";
                comm.Transaction = tx;
                comm.ExecuteNonQuery();
                tx.Commit();
                return (true);
            }
            catch(Exception e)
            {
                tx.Rollback();
                Function.WriteErrorLog("ChangePW修改数据出错:" + e.Message);
                MessageBox.Show("修改密码出错！");
                return (false);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion
    }
}
