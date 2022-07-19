using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace System
{

    [DataContract]
    public class ResponseData
    {
        public static class DataPayloadType
        {
            public const string UserError = "UserError";
            public const string Exception = "Exception";
        }

        [DataMember]
        public string method { get; set; }

        [DataMember]
        public int keyID { get; set; }

        [DataMember]
        public int totalRowCount { get; set; }

        [DataMember]
        public int pageIndex { get; set; }

        [DataMember]
        public int pageSize { get; set; }

        //public void SetPagingInfo(PagingInfo pageInfo)
        //{
        //    this.pageIndex = pageInfo.pageIndex;
        //    this.pageSize = pageInfo.pageSize;
        //    this.totalRowCount = pageInfo.totalRowCount;
        //}

        [DataMember]
        public string sortedBy { get; set; }

        //[DataMember]
        //public bool success { get; set; }

        //[DataMember]
        //public bool isUserError { get; set; }

        [DataMember]
        public string exception { get; set; }

        [DataMember]
        public string debugInfo { get; set; }

        //[DataMember]
        //public RequestData reqData { get; set; }

        [DataMember]
        public Object payload { get; set; }

        //[DataMember]
        //public string payloadText { get; set; }

        [DataMember]
        public string payloadType { get; set; } //"json"; //text, html
                                                //public string Json { get; set; }
                                                //public string Html { get; set; }

        [DataMember]
        public Object bag { get; set; }

        [DataMember]
        public string apiVersion { get; set; } = "0.1";

        [DataMember]
        public Object tag1 { get; set; }

        [DataMember]
        public string sqlStatements { get; set; }
        public static CircularQueue<string> SqlStatementQueue = new CircularQueue<string>(30);
        public string addSql(DbCommand cmd, string label = null)
        {
            var sql = DbCommandX.ToStr(cmd);
            sqlStatements += "\n\n";
            if (label != null)
            {
                sqlStatements += label + "\n";
            }
            sqlStatements += sql;
            SqlStatementQueue.EnqueueThreadSafe(sql);

            MOutput.WriteHtmlEncodedPre("ResponseData.addSql: " + sql);
            return sql;
        }

        public string addSql(string sql, string label = null)
        {
            sqlStatements += "\n\n";
            if (label != null)
            {
                sqlStatements += label + "\n";
            }
            sqlStatements += sql;
            SqlStatementQueue.EnqueueThreadSafe(sql);

            MOutput.WriteHtmlEncodedPre("ResponseData.addSql: " + sql);
            return sql;
        }
        public string addSqlWithFormat(string sql, params object[] args)
        {
            sql = String.Format(sql, args);
            sqlStatements += "\n\n";
            //if (label != null)
            //{
            //    sqlStatements += label + "\n";
            //}
            sqlStatements += sql;
            return sql;
        }

        //===================================================================================================
        public string ToHtml()
        {
            var buf = new StringBuilder();
            string format = "{0}={1}<br>\n";
            buf.AppendFormat(format, "method", method);
            buf.AppendFormat(format, "keyID", keyID);
            buf.AppendFormat(format, "exception", HtmlText.HtmlEncode(exception));
            buf.AppendFormat(format, "sqlStatements", HtmlText.HtmlEncode(sqlStatements));
            buf.AppendFormat(format, "debugInfo", debugInfo);
            buf.AppendFormat(format, "bag", bag);
            buf.AppendFormat(format, "tag1", tag1);
            buf.AppendFormat(format, "payload", StringifyPayload());
            return buf.ToString();
        }
        //===================================================================================================
        public string StringifyPayload()
        {
            var resData = this;
            string s = "";
            if (resData.payload == null)
            {
                s = "payload is null";
            }
            else
            {
                var type = resData.payload.GetType();
                s += "payload.getType()=" + type;
                s += HtmlValue.OfAny(resData.payload);
            }
            //s += "<br>";
            return s;
        }
    }
    //===================================================================================================
    public class DataSvcInfo
    {
        //public string dbName { get; set; }
        //public string dbServer { get; set; }
        public string AppName { get; set; }
        public string AppEnv { get; set; }
        public string DbEnv { get; set; }
        public bool IsAdminApp => ConfigX.IsAdminApp;

        public DateTime AppStartedDateTime { get; set; }
        //public string SeverName { get; set; } = "Uninit";
        public string SrvrName { get; set; }
        //public string UserIP { get; set; }
        //public string  { get; set; }

        public string ApiVersion { get; set; }

        public DateTime DbServerDateTime { get; set; }
        public string Username { get; set; }// = "Uninit";
        public string UserRoles { get; set; }// = "Uninit";
        //public string svcAppPath { get; set; }
        //public string PublicFacingUrlRoot { get; set; } //= "Uninit";
        public List<string> OtherInfo { get; set; }
    }
}
