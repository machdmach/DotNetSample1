
namespace Libx;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DbColAttribute : Attribute
{
    public string ColumnName { get; set; }
    //public bool PrimaryKey { get; set; }
    public bool NoInsert { get; set; }
    public bool NoUpdate { get; set; }
    public bool NoInsertUpdate
    {
        set => NoInsert = NoUpdate = value;
        get => throw new NotImplementedException("");
    }
    //public bool NoInsertUpdate2 => NoUpdate = value;

    public DbColAttribute()
    {
        //this.ColName = "";
    }
    public DbColAttribute(string colName)
    {
        ColumnName = colName;
    }
}

//===================================================================================================
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class DbTabAttribute : Attribute
{
    public string TableName { get; set; }
    //public bool PrimaryKey { get; set; }
    //public bool NoInsert { get; set; }
    //public bool NoUpdate { get; set; }
    //public bool NoInsertUpdate
    //{
    //    set { NoInsert = NoUpdate = value; }
    //    get { throw new NotImplementedException(""); }
    //}
    //public bool NoInsertUpdate2 => NoUpdate = value;

    public DbTabAttribute()
    {
        //this.ColName = "";
    }
    public DbTabAttribute(string tabName)
    {
        TableName = tabName;
    }
}
