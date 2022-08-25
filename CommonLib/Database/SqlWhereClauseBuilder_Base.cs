using System.Collections;
namespace Libx;
public class SqlWhereClauseBuilder_Base
{
    protected internal ArrayList _dbConds = ArrayList.Synchronized(new ArrayList(10));
    protected bool IsOr { get; set; }

    //===================================================================================================
    protected void AddCond(String dbCond)
    {
        string cond = dbCond;
        if (cond == null) { throw new SystemException("null param"); }
        cond = cond.Trim();
        //if (cond.Length()<1) throw new Exception ("Cond must not be empty");
        if (cond.Equals("()"))
        {
            //log.Debug("empty for andCond ()");
            //return this;
            return;
        }
        if (cond.Length < 1)
        {
            //log.Debug("SQLSearchCond_BASE: cond len lt 1, not added to cond");
            //return this;
            return;
        }
        if (cond.IndexOf(" or ") > 0 && cond.IndexOf('(') < 0)
        { //not already (or a=1)
            string s = cond;
            //String s = XSqlStatement.DecodeTexts(cond); //makesure
            if (s.IndexOf(" or ") > 0)
            {
                cond = "(" + cond + ")";
                //log.Debug("added () b/c there was or cond");
            }
        }

        if (IsOr)
        {
            _dbConds.Add("or " + cond);
        }
        else
        {
            _dbConds.Add("and " + cond);
        }
        //if (userCond != null) { _userConds.Add("And " + userCond); }

        //Log.Debug("_dbConds=" + IEnumerableX.ToString(_dbConds, "\n"));
        //return this;
    }
}