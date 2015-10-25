using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using Company.Utilities.SQL;
using Company.Utilities.SQL.Interfaces;
using Company.Utilities.SQL.MicroORM;
using Common.Model;
using Common.Entity;
using Common.Entity.Interface;

namespace Common.Base
{
    public class BoBase
    {
        #region 私有物件

        private string connectionString = "";
        private IParameter[] parameters;
        private string dbType = "System.Data.SqlClient";

        #endregion

        #region 屬性

        /// <summary>
        /// 資料庫連線字串
        /// </summary>
        protected string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        protected string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// 執行是否成功
        /// </summary>
        protected bool IsSuccess
        {
            get;
            private set;
        }

        /// <summary>
        /// 輸入參數
        /// </summary>
        protected IParameter[] Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new IParameter[] { };
                    return parameters;
                }
                else
                    return parameters;
            }
            set
            {
                if (value == null)
                    parameters = new IParameter[] { };
                else
                    parameters = value;
            }
        }

        /// <summary>
        /// Exception object
        /// </summary>
        protected Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        protected string DataBaseName
        {
            get;
            set;
        }

        /// <summary>
        /// 資料庫種類
        /// </summary>
        protected string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        #endregion

        #region 建構子
        public BoBase()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        }

        public BoBase(string DbName, string DbType = "System.Data.SqlClient")
        {
            connectionString = ConfigurationManager.ConnectionStrings[DbName].ToString();
            DataBaseName = DbName;
            this.DbType = DbType;
        }

        #endregion

        #region Public Method

        #region DataSet Query
        /// <summary>
        /// 執行 SQL Command 取得 DataSet
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="createTrans"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sqlCommand, bool createTrans = false, bool isCache = false)
        {
            bool result = true;
            SQLHelper helper = new SQLHelper(sqlCommand, connectionString, CommandType.Text, DataBaseName, DbType);
            if (Parameters != null && Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            DataSet ds = null;
            try
            {
                ds = helper.ExecuteDataSet(createTrans, isCache);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return ds;
        }

        public DataSet GetDataSet(Command cmd)
        {
            bool result = true;
            SQLHelper helper = new SQLHelper(cmd, connectionString, DataBaseName, DbType);

            if (Parameters != null && Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            DataSet ds = null;
            try
            {
                ds = helper.ExecuteDataSet();
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return ds;

        }

        public DataSet GetPagedDataSet(string sqlCommand, string OrderBy = "", int PageSize = 10, int CurrentPage = 1, bool createTrans = false, bool isCache = false)
        {
            bool result = true;
            string PagedCommand = SetupPagedCommand(sqlCommand, OrderBy, PageSize, CurrentPage);
            SQLHelper helper = new SQLHelper(PagedCommand, connectionString, CommandType.Text, DataBaseName, DbType);
            DataSet ds = null;

            if (Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            try
            {
                ds = helper.ExecuteDataSet(createTrans, isCache);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;

            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return ds;
        }

        /// <summary>
        /// 執行 Procedure 取得 DataSet
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="createTrans"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public DataSet GetDataSetProcedure(string sqlCommand, bool createTrans = false, bool isCache = false)
        {
            bool result = true;
            SQLHelper helper = new SQLHelper(sqlCommand, connectionString, CommandType.StoredProcedure, DataBaseName, DbType);
            if (Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            DataSet ds = null;
            try
            {
                ds = helper.ExecuteDataSet(createTrans, isCache);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return ds;
        }

        #endregion

        #region None Query
        /// <summary>
        /// 執行非查詢 Command
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="createTrans"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public int ExecuteNoneQuery(string sqlCommand, bool createTrans = false, bool isCache = false)
        {
            bool result = true;
            int execCount = 0;
            SQLHelper helper = new SQLHelper(sqlCommand, connectionString, CommandType.Text, DataBaseName, DbType);
            if (Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            try
            {
                execCount = helper.ExecuteNonQuery(createTrans);
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return execCount;
        }

        public int ExecuteNoneQuery(Command cmd)
        {
            bool result = true;
            int execCount = 0;
            SQLHelper helper = new SQLHelper(cmd, connectionString, DataBaseName, DbType);
            if (Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            try
            {
                execCount = helper.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            finally
            {
                helper.Dispose();
            }

            IsSuccess = result;
            return execCount;

        }

        #endregion

        #region IResultModel Query

        /// <summary>
        /// 執行返回資料查詢
        /// </summary>
        /// <param name="cmd">SQL Command</param>
        /// <param name="createTrans">是否使用交易查詢</param>
        /// <param name="isCache">是否使用 cache</param>
        /// <returns></returns>
        public IResultModel ExecQuery(string sqlCommand, bool createTrans = false, bool isCache = false)
        {
            DataSet ds = GetDataSet(sqlCommand, createTrans, isCache);
            IResultModel result = new ResultModel
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                ResultObject = ds
            };
            return result;
        }

        /// <summary>
        /// 執行返回資料查詢
        /// </summary>
        /// <param name="cmd">ICommand</param>
        /// <returns></returns>
        public IResultModel ExecQuery(Command cmd)
        {
            DataSet ds = GetDataSet(cmd);
            IResultModel result = new ResultModel()
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                ResultObject = ds
            };
            return result;
        }

        public IResultModel ExecQueryPaged(string sqlCommand, string orderBy, int PageSize, int CurrentPage)
        {
            DataSet ds = GetPagedDataSet(sqlCommand, orderBy, PageSize, CurrentPage);
            IResultModel result = new ResultModel
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                ResultObject = ds
            };

            return result;
        }

        public IResultModel ExecQueryPaged(string cmd, string orderBy, int PageSize, int? CurrentPage)
        {
            int currentPage = CurrentPage == null ? 0 : CurrentPage.Value;
            return ExecQueryPaged(cmd, orderBy, PageSize, currentPage);
        }

        public IResultModel ExecQueryPaged(string cmd, PageModel pageModel)
        {
            return ExecQueryPaged(cmd, pageModel.Order, pageModel.PageSize, pageModel.CurrentPage);
        }

        public IResultModel ExecProcedure(string sqlCommand, bool createTrans = false, bool isCache = false)
        {
            DataSet ds = GetDataSetProcedure(sqlCommand, createTrans, isCache);
            IResultModel result = new ResultModel()
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                ResultObject = ds
            };
            return result;
        }

        #endregion

        #region IResultModel None Query

        /// <summary>
        /// 執行新增/修改/除查詢
        /// </summary>
        /// <param name="cmd">SQL Command</param>
        /// <param name="createTrans">是否使用交易查詢</param>
        /// <param name="isCache">是否使用 cache</param>
        /// <returns></returns>
        public IResultModel ExecNoneQuery(string cmd, bool createTrans = false, bool isCache = false)
        {
            int exeCount = ExecuteNoneQuery(cmd, createTrans, isCache);
            IResultModel result = new ResultModel
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                AffectCount = exeCount
            };
            return result;
        }

        /// <summary>
        /// 執行新增/修改/除查詢
        /// </summary>
        /// <param name="cmd">ICommand</param>
        /// <returns></returns>
        public IResultModel ExecNoneQuery(Command cmd)
        {
            int exeCount = ExecuteNoneQuery(cmd);
            IResultModel result = new ResultModel()
            {
                IsSuccess = IsSuccess,
                Exception = Exception,
                AffectCount = exeCount
            };
            return result;

        }

        /// <summary>
        /// 執行 StoredProcedure取得輸出參數
        /// </summary>
        /// <param name="sqlCommand">SQL Command</param>
        /// <param name="outParamsId">輸出參數的Id</param>
        /// <param name="ParameterStarter">參數前置符號</param>
        /// <returns></returns>
        public IResultModel GetOutProcedure(string sqlCommand, string[] outParamsId)
        {
            IResultModel result = new ResultModel();
            DataSet ds = new DataSet();
            int execCount = 0;

            SQLHelper helper = new SQLHelper(sqlCommand, connectionString, CommandType.Text, DataBaseName, DbType);
            if (Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            try
            {
                execCount = helper.ExecuteNonQuery();
                //取得輸出參數
                DataTable dt = new DataTable("OutParameters");
                dt.Columns.Add(new DataColumn("Id", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("Value", Type.GetType("System.Object")));

                for (int i = 0; i < outParamsId.Length; i++)
                {
                    var Id = outParamsId[i];
                    var p = FindParameter(Id);
                    if (p != null)
                    {
                        object objVal = helper.GetParameter(Id, "", ParameterDirection.Output);
                        DataRow dr = dt.NewRow();
                        dr["Id"] = Id;
                        dr["Value"] = objVal;
                        dt.Rows.Add(dr);
                    }
                }

                ds.Tables.Add(dt);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
                IsSuccess = false;
            }
            finally
            {
                helper.Dispose();
            }

            result.IsSuccess = IsSuccess;
            result.Exception = Exception;
            result.ResultObject = ds;

            return result;
        }

        #endregion

        public DataType ExecScalar<DataType>(string cmd, bool createTrans = false, bool isCache = false)
        {
            SQLHelper helper = new SQLHelper(cmd, connectionString, CommandType.Text, DataBaseName, DbType);
            if (Parameters != null && Parameters.Length > 0)
            {
                helper.AddParameter(Parameters);
            }

            DataType ReturnValue = default(DataType);
            try
            {
                ReturnValue = helper.ExecuteScalar<DataType>(createTrans, isCache);
                IsSuccess = true;
            }
            catch(Exception ex)
            {
                Exception = ex;
                IsSuccess = false;
            }
            finally
            {
                helper.Dispose();
            }

            return ReturnValue;
        }

        public IParameter GetParameter<T>(string paraName, object objValue) 
            where T : struct
        {
            IParameter parameter;
            Type t = typeof(T);
            switch(t.Name)
            {
                case "Int16":
                case "Int32":
                    parameter = new Parameter<int>(paraName, int.Parse(objValue + ""));
                    break;
                case "Int64":
                    parameter = new Parameter<long>(paraName, long.Parse(objValue + ""));
                    break;
                case "Decimal":
                    parameter = new Parameter<decimal>(paraName, decimal.Parse(objValue + ""));
                    break;
                case "Double":
                    parameter = new Parameter<double>(paraName, double.Parse(objValue + ""));
                    break;
                case "Float":
                    parameter = new Parameter<float>(paraName, float.Parse(objValue + ""));
                    break;
                case "String":
                    parameter = new Parameter<string>(paraName, objValue + "");
                    break;
                case "DateTime":
                    parameter = new Parameter<DateTime>(paraName, (DateTime)objValue);
                    break;
                case "Boolean":
                    parameter = new Parameter<bool>(paraName, (bool)objValue);
                    break;
                default:
                    parameter = new Parameter<string>(paraName, objValue + "");
                    break;
            }

            return parameter;
        }

        public IParameter GetParameter<T>(string key, Dictionary<string, object> dictionary) 
            where T : struct
        {
            return GetParameter<T>(key, dictionary[key]);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 產生分頁的 SQL 查詢
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        private string SetupPagedCommand(string sqlCommand, string orderBy, int pageSize, int currentPage)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "1";
            }

            currentPage = currentPage == 0 ? 0 : currentPage - 1;
            int PageStart = currentPage * pageSize;
            string Query = "";
            switch(dbType)
            {
                case "MySql.Data.MySqlClient":
                    Query = string.Format("{0} ORDER BY {1} LIMIT {2}, {3} ;",
                        sqlCommand,
                        orderBy,
                        PageStart,
                        pageSize);
                    break;

                default:
                    Query = string.Format("SELECT Paged.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, Query.* FROM ({0}) as Query) AS Paged WHERE Row>{2} AND Row<={3} ;",
                            sqlCommand,
                            orderBy,
                            PageStart,
                            PageStart + pageSize);
                    break;
            }

            Query += string.Format("SELECT COUNT(*) AS TotalCount FROM ({0}) AS Query ;", sqlCommand);

            return Query;
        }

        /// <summary>
        /// 以 Parameter Id 取得設定的 Parameter
        /// </summary>
        /// <param name="Id">Parameter Id</param>
        /// <returns></returns>
        private IParameter FindParameter(string Id)
        {
            foreach(var p in Parameters)
            {
                if (p.ID == Id)
                    return p;
            }

            return null;
        }

        #endregion

    }
}
