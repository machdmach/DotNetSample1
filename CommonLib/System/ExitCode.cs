namespace Libx
{
    public enum ExitCode
    {
        [StringValue("SUCCESS")]
        SUCCESS = 0,
        [StringValue("FAILED")]  ///means there was an exit message when command was invoked, but no result found...
        FAILED = 1,
        [StringValue("ERROR")]  //All other exceptions
        ERROR = 2,
    }

    //===================================================================================================
    public class StringValueAttribute : Attribute
    {
        private readonly string _value;
        public StringValueAttribute(string value)
        {
            _value = value;
        }
        public string Value => _value;
    }
}
