using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System
{
    public class StackTraceX
    {
        private static void Main()
        {
            StackTrace st = new StackTrace();
            foreach (StackFrame sf in st.GetFrames())
            {
                //Log.Debug(StackFrameX.ToString(sf));
                //LogX.Debug(sf.GetMethod().DeclaringType);
            }
            Type t = st.GetFrame(st.FrameCount - 1).GetMethod().DeclaringType;
            //LogX.Debug(t);
        }
        //========================================================================
        public static void ShowInfo_eg()
        {
            StackTrace st = new StackTrace();
            foreach (StackFrame sf in st.GetFrames())
            {
                //LogX.Debug(sf.GetMethod().DeclaringType);
            }
        }
        //========================================================================
        public static string GetStackInfo(ICollection<Type> ignoredTypes, bool inHtml = false)
        {
            StringCollection CallerStack = new();

            StackTrace stackTrace = new (true);
            string prefix = "";
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame sf = stackTrace.GetFrame(i);
                MethodBase method = sf.GetMethod();

                Type methodClass = method.ReflectedType;

                if (ignoredTypes != null && ignoredTypes.Contains(methodClass))
                {
                    continue;
                }
                else if (methodClass == typeof(StackTraceX))
                {
                    continue;
                }
                string methodStr = method.ToString();
                string methodSig = methodStr.Substring(methodStr.IndexOf(' '));
                methodSig = Regex.Replace(methodSig, @"\w+\.", "");
                int lineNum = sf.GetFileLineNumber();

                //no file and line info are provided if Remote remote server 
                // machine.config<system.web><deployment retail="true"/>
                if (lineNum > 0)
                {
                    string s = string.Format("{0} @{1}:{2}"
                        , methodSig
                        //, Path.GetFileName(sf.GetFileName())
                        , sf.GetFileName()
                        , lineNum
                    );
                    s = prefix + s;
                    prefix += " ";
                    CallerStack.Add(s);
                }
            }
            //return IEnumerableX.ToString(CallerStack, "\n");
            StringBuilder buf = new StringBuilder();
            CallerStack.Cast<string>().ToList().ForEach(e => buf.Append(e + "\n"));
            if (buf.Length > 1) buf.Remove(buf.Length - 1, 1);
            string ret = buf.ToString();

            if (inHtml)
            {
                StringBuilder buf2 = new StringBuilder();
                buf2.Append("<pre>");
                buf2.Append(WebUtility.HtmlEncode(ret));
                buf2.Append("</pre>");
                ret = buf2.ToString();
            }

            return ret;
        }
    }

}
