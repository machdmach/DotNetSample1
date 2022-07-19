namespace System
{
    public class UserException : Exception
    {
        public UserException(string mesg) : base(mesg)
        {

        }
        public UserException(string mesg, Exception innerException) : base(mesg, innerException)
        {

        }
    }

    //===================================================================================================
    public class UserInputDataException : UserException
    {
        public UserInputDataException(string mesg) : base(mesg)
        {
        }
        public UserInputDataException(string mesg, Exception innerException) : base(mesg, innerException)
        {
        }
    }

    //===================================================================================================
    public class DosException : UserException
    {
        public DosException(string mesg) : base(mesg)
        {
        }
        public DosException(string mesg, Exception innerException) : base(mesg, innerException)
        {
        }

    }

}