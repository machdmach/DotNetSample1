#if USAGE
           Linq:

            SearchFilterValueCollection f = new SearchFilterValueCollection(cbNegativeOnCriteria.Checked);
            SearchFilterValue v;
            //-------------------------------------------------------------String
            v = f.NewFilterValue(tbString);
            string String_Value = v.StringValue;
            q = v.IsEmpty ? q :
                v.IsNull ? q.Where(r => r.String == null) :
                v.IsNotNull ? q.Where(r => r.String != null) :
                v.IsLike ? q.Where(r => SqlMethods.Like(r.String, String_Value)) :
                v.IsNotLike ? q.Where(r => !SqlMethods.Like(r.String, String_Value)) :
                v.IsNot ? q.Where(r => r.String != String_Value) :
                q.Where(r => r.String == String_Value);

            //-------------------------------------------------------------int
            v = f.NewFilterValue(tbNumber1);
            int Number1_Value = v.IntValue;
            q = v.IsEmpty ? q :
                v.IsNull ? q.Where(r => r.Number1 == null) :
                v.IsNotNull ? q.Where(r => r.Number1 != null) :
                v.IsNot ? q.Where(r => r.Number1 != Number1_Value) :
                q.Where(r => r.Number1 == Number1_Value);


            //------------------------------------------------------------DateTime
            v = f.NewFilterValue(tbDateTime1);
            DateTime DateTime1_Value = v.DateTimeValue;
            q = v.IsEmpty ? q :
                v.IsNull ? q.Where(r => r.DateTime1 == null) :
                v.IsNotNull ? q.Where(r => r.DateTime1 != null) :
                q.Where(r => DateTime1_Value <= r.DateTime1 && r.DateTime1 < DateTime1_Value.AddDays(1));

#endif
namespace Libx;
public class SearchFilterValue
{
    private readonly string val = "";


    //bool _contains = false;
    //bool _startsWith = false;
    //bool _endsWith = false;

    //bool _notContains = false;
    //bool _notStartsWith = false;
    //bool _notEndsWith = false;		

    private readonly bool _isLike = false;
    private readonly bool _isNotLike = false;
    private readonly bool _isNull = false;
    private readonly bool _isNotNull = false;
    private readonly bool _isEmpty = false;
    private readonly bool _isNot = false;
    public bool NegativeOnCriteria { get; set; }
    public SearchFilterValue(long value) : this(value + "", false) { }

    //public SearchFilterValue(string value) : this(value, false) { }
    //public SearchFilterValue(string value, bool addWildCards) : this(value, addWildCards, false) { }
    public SearchFilterValue(string value, bool addWildCards = true, bool negativeOnCriteria = false)
    {
        //}
        //public void SetValue(string value, bool negativeOnCriteria, bool addWildCards)
        //{
        NegativeOnCriteria = negativeOnCriteria;
        string s = value;

        if (s == null) s = "";
        s = s.Trim();
        s = s.Replace('*', '%');


        //string origS = s;

        //--- Last, if none of above conditions not met, then check for if search criteria value is present
        if (s == "")
        {
            _isEmpty = true;
            //if (negativeOnCriteria) { _isNot = !_isNot; } _isNot apply only to non-wildcard values.
        }
        //----------------------------------------
        //--- null/notNull
        //bool hasWildCards = true;
        else if (s == "!" || s == "!%")
        {
            _isNull = true;
            if (negativeOnCriteria) { _isNull = !_isNull; }
            s = "";
        }
        else if (s == "%")
        {
            _isNotNull = true;
            if (negativeOnCriteria) { _isNotNull = !_isNotNull; }
            s = "";
        }

        //--- like
        else if (s.Contains("%"))
        {
            if (s.StartsWith("!"))
            {
                _isNotLike = true;
                if (negativeOnCriteria) { _isNotLike = !_isNotLike; }
                s = s.Substring(1);
            }
            else
            {
                _isLike = true;
                if (negativeOnCriteria) { _isLike = !_isLike; }
            }
        }
        //--- not  :  where id <> 123
        else if (s.StartsWith("!"))
        {
            _isNot = true;
            if (negativeOnCriteria) { _isNot = !_isNot; }
            s = s.Substring(1);
        }
        else //not empty, not has wildcards
        {
            //hasWildCards = false;
            if (negativeOnCriteria) { _isNot = !_isNot; }
        }

        //--- Last, if none of above conditions not met, then check for if search criteria value is present
        //else if (s != "")
        //{
        //    _isEmpty = false;
        //    if (negativeOnCriteria) { _isNot = !_isNot; }
        //}

        if (addWildCards)
        {
            if (s.IndexOf('%') < 0)
            {
                s = '%' + s + '%';
            }
            if (_isNot)
            {
                _isNot = false;
                _isNotLike = true;
            }
            else //if (!_isLike)
            {
                _isLike = true;
            }
        }
        //v = new SearchFilterValue(tbMessage.Text, false, true);
        //string Message_value = v.StringValue;
        //must be in this order
        //q = v.IsEmpty ? q :
        //v.IsNull ? q.Where(r => r.Message == null) :
        //v.IsNotNull ? q.Where(r => r.Message != null) :
        //v.IsLike ? q.Where(r => SqlMethods.Like(r.Message, Message_value)) :
        //v.IsNotLike ? q.Where(r => !SqlMethods.Like(r.Message, Message_value)) :
        //v.IsNot ? q.Where(r => r.Message != Message_value) :
        //q.Where(r => r.Message == Message_value);


        val = s;

        //if (negativeOnCriteria)
        //{
        //    _isNull = !_isNull;
        //    _isNotNull = !_isNotNull;
        //    _isLike = !_isLike;
        //    _isNotLike = !_isNotLike;
        //    _isEmpty = !_isEmpty;
        //    _isNot = !_isNot;
        //}

    }
    //bool orderByAsc = true;
    //int orderByPosition = -1;
    //public int OrderByPositionzz
    //{
    //    get
    //    {
    //        return orderByPosition;
    //    }
    //}
    //public bool IsOrderByDescendingzz
    //{
    //    get
    //    {
    //        return !orderByAsc;
    //    }
    //}
    //public WebControl WebControl { get; set; }
    //public SearchFilterValue(TextBox tb, bool negativeOnCriteria)
    //    : this(tb.Text, negativeOnCriteria)
    //{
    //    WebControl = tb;
    //}
    public bool IsEmpty => _isEmpty;
    public bool IsNull => _isNull;
    public bool IsNotNull => _isNotNull;
    //public bool IsStartWith { get { return _isStartWith; } }
    //public bool IsNotStartWith { get { return _isNotStartWith; } }
    public bool IsLike => _isLike;
    public bool IsNotLike => _isNotLike;
    public bool IsNot => _isNot;
    //========================================================================
    public DateTime DateTimeValue
    {
        get
        {
            if (string.IsNullOrEmpty(val) || val == "!" || val == "%") { return DateTime.MinValue; }

            if (val.ToUpper() == "NOW" || val.ToUpper() == "TODAY" || val == ".")
            {
                return DateTime.Today;
            }
            if (Int32.TryParse(val, out int x))
            {
                return DateTime.Today.AddDays(x);
            }
            // = DateTime.Now;
            if (!DateTime.TryParse(val, out DateTime rval))
            {
                throw new Exception("invalid date: " + val);
            }
            return rval;
        }
    }
    public string StringValue => val;
    public int IntValue
    {
        get
        {
            //Debug.WriteLine("IntValue, control.ID=" + WebControl.ID + ", Text=" + s);
            if (string.IsNullOrEmpty(val)) { return -1; }
            return Int32.Parse(val);
            //Debug.WriteLine(WebControl.ID + ", Text=" + s + ", IntValue=" + rval);
            //return rval;
        }
    }

}
public class DbSearchCriterion : SearchFilterValue
{
    public string DbColumnName { get; set; }
    public string FormFieldNamez { get; set; }
    public string DisplayTextz { get; set; }
    public Type DataType { get; set; }

    //public DbSearchCriterion(long value) : this(value + "", false) { }
    //public DbSearchCriterion(string value) : this(value, true) { }
    //public DbSearchCriterion(string value, bool negativeOnCriteria) : this(value, false, false) { }
    public DbSearchCriterion(string dbColumnName, string value, bool addWildCards = true, bool negativeOnCriteria = false) : base(value, addWildCards, negativeOnCriteria)
    {
        DbColumnName = dbColumnName;
        DataType = typeof(String);
    }
}

