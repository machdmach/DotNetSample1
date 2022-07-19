namespace Libx.Mvc.App;
public class DbManController : AppAdminControllerBase
{
    protected override Func<DbManDbContext> NewDbContext => () =>
    {
        string connStr = req.QueryData.GetString_withCookieAsStorage("ConnStrKey", req);
        return new(connStr, logger);
    };
    protected override DbManDbContext db => (DbManDbContext)base.db;
    protected override void Init()
    {
        base.Init();
        req.EnsureDevMachine();
    }

    //===================================================================================================
    public async Task<IActionResult> Index()
    {
        try
        {
            Init();
            var buf = new StringBuilder();

            var nvs = new List<Tuple<string, string, string>>();
            foreach (KeyValuePair<string, string> nv in DbConnectionString.ConnectionStrings)
            {
                string dbInfoLink = string.Format("<a href='~/api/DbMan/DbInfo?ConnStrKey={0}'>{1}</a>", nv.Key, "DbInfo");
                string TableListLink = string.Format("<a href='~/api/DbMan/TableList?ConnStrKey={0}'>{1}</a>", nv.Key, nv.Key);
                string val = string.Format("<pre>{0}</pre>", nv.Value);
                nvs.Add(new(TableListLink, dbInfoLink, val));
            }
            string s = HtmlValue.OfList(nvs);
            //s = s.Replace("~/", ConfigX.WebAppVirtualPath + "/");
            s = MvcLib.ResolveHtmlTextToAbsolutePath(s);
            return MyPassThroughView(s);
        }
        catch (Exception ex) { return await MyErrorResult(ex); }
    }
    //===================================================================================================
    public async Task<IActionResult> DbInfo()
    {
        return await MyApiResultWrapper(() =>
        {
            var buf = new StringBuilder();
            buf.Append(db.GetConnectionInfo());
            return buf;
        });
    }

    //===================================================================================================
    public async Task<IActionResult> TableListRaw()
    {
        return await MyApiResultWrapper(() =>
        {
            string sql = db.DbmsType switch
            {
                DbmsType.Oracle => "select * from user_tables order by table_name",
                DbmsType.SqlServer => "select * from information_Schema.tables order by table_name",
                _ => throw new Exception("unknown db")
            };
            DataTable dt = db.QueryDataTable(sql);
            return dt;
        });
    }
    //===================================================================================================
    public async Task<IActionResult> TableList()
    {
        return await MyApiResultWrapper(() =>
        {
            //if (req.Is_zzRaw())
            //{
            //}
            string sql = db.DbmsType switch
            {
                DbmsType.Oracle => "select TABLE_NAME, NUM_ROWS from user_tables order by table_name",
                //TABLE_CATALOG	TABLE_SCHEMA	TABLE_NAME	TABLE_TYPE
                //wherec += "table_type='base table'";
                DbmsType.SqlServer => "select TABLE_NAME, 1 as NUM_ROWS from information_Schema.tables order by table_name",
                _ => throw new Exception("unknown db")
            };
            DataTable dt = db.QueryDataTable(sql);
            dt.Columns.Add("Columns", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                var tabName = (string)row[0];
                //if (tabName.Contains(' ')) { continue; }

                string tabDataLink = string.Format("<a href='TableData?tableName={0}'> {0} </a>", tabName);
                string tabColsLink = string.Format("<a href='TableColumns?tableName={0}'> Columns </a>", tabName);

                row[0] = tabDataLink;

                row[1] = db.ExecuteScalar<object>(string.Format("select count(*) from {0}", db.WrapLiteral(tabName)));
                row[2] = tabColsLink;
            }
            DataView dataSource = dt.DefaultView;

            if (req.QueryData.GetOrDefault("orderByRowCount", false))
            {
                EnumerableRowCollection<DataRow> query =
                    from lst in dt.AsEnumerable()
                        //where order.Field<Int32>("rowCountx") == true
                    orderby lst.Field<object>("NUM_ROWS") descending
                    select lst;
                dataSource = query.AsDataView();
            }
            dt = dataSource.ToTable();

            HtmlBuilder buf = new();
            buf.Br().Anchor("TableList");
            buf.Br().Anchor("TableList?orderByRowCount=1");
            buf.Br().Anchor("TableListRaw");
            buf.Append(HtmlValue.OfDataTable(dt));

            return buf;
        });
    }
    //===================================================================================================
    public async Task<IActionResult> TableColumns(string tableName)
    {
        return await MyApiResultWrapper(() =>
        {
            var dt = db.TableInfo.GetColumns(tableName);

            HtmlBuilder buf = new();
            buf.Br().Anchor("TableRawColumns?tableName="+ tableName);
            buf.Append(HtmlValue.OfList(dt));
            return buf;
        });
    }
    //===================================================================================================
    public async Task<IActionResult> TableRawColumns(string tableName)
    {
        return await MyApiResultWrapper(() =>
        {
            var cols = db.TableInfo.GetRawColumns(tableName);
            return cols;
        });
    }
    //===================================================================================================
    public async Task<IActionResult> TableData(string tableName)
    {
        return await MyApiResultWrapper(() =>
        {
            string sql = string.Format("select * from {0}", tableName);
            var dt = db.QueryDataTable(sql);
            return dt;
        });
    }
    //===================================================================================================
}

