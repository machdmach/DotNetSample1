using System.Data;
using System.Data.Common;
using System.Net;
using System.Runtime.Serialization;

namespace Libx
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

        public string sortedBy { get; set; }
        public string exception { get; set; }
        public string debugInfo { get; set; }
        public Object payload { get; set; }
        //public Type   payloadDataType { get; set; }
        //NotSupportedException: Serialization and deserialization of 'System.Type' instances are not supported.

        public string payloadDataType
        {
            get
            {
                return (payload == null) ? "null" : payload.GetType().Name;
            }
        }
        public string payloadType { get; set; } //valid types=json,html,plaintext,usererror,syserror,exception
        public Object bag { get; set; }
        public string apiVersion { get; set; } = "0.1";
        public Object tag1 { get; set; }
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
            buf.AppendFormat(format, "exception", WebUtility.HtmlDecode(exception));
            buf.AppendFormat(format, "sqlStatements", WebUtility.HtmlEncode(sqlStatements));
            buf.AppendFormat(format, "debugInfo", debugInfo);
            buf.AppendFormat(format, "bag", bag);
            buf.AppendFormat(format, "tag1", tag1);
            buf.AppendFormat(format, "payloadDataType", payloadDataType);
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
                s += HtmlValue.OfAny(resData.payload);
            }
            return s;
        }
    }
    //===================================================================================================
    public class DataSvcInfo
    {
        public string AppName { get; set; }
        public string AppEnv { get; set; }
        public string DbEnv { get; set; }
        public bool IsAdminApp => ConfigX.IsAdminApp;
        public DateTime AppStartedDateTime { get; set; }
        public string SrvrName { get; set; }
        public string ApiVersion { get; set; }
        public DateTime DbServerDateTime { get; set; }
        public string Username { get; set; }// = "Uninit";
        public string UserRoles { get; set; }// = "Uninit";
        public List<string> OtherInfo { get; set; }
    }
}
