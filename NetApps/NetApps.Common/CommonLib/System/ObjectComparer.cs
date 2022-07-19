using System.Linq;
using System.Reflection;

namespace System
{
    public class ObjectComparer
    {
        //============================================================================================
        public static string CompareObjectsToHtml(Object o1, Object o2)
        {
            var c = new ObjectComparer();
            string s = c.CompareObjectsToHtml2(o1, o2);
            return s;
        }

        private StringBuilder buf;
        private HtmlTableWriter tw;

        //===================================================================================================
        private void AddProp(string prop1Name, object v1, string prop2Name, object v2)
        {
            string s1;
            string s2;
            if (prop1Name == null)
            {
                s1 = "";
                s2 = (v2 == null) ? "null" : v2.ToString();
            }
            else if (prop2Name == null)
            {
                s1 = (v1 == null) ? "null" : v1.ToString();
                s2 = "";
            }
            else
            {
                s1 = (v1 == null) ? "null" : v1.ToString();
                s2 = (v2 == null) ? "null" : v2.ToString();
                prop2Name = "''";
            }
            if (s2 == s1)
            {
                s2 = "''";
            }
            tw.AddRow_NValues(prop1Name, s1, s2, prop2Name);

        }
        //============================================================================================
        public string CompareObjectsToHtml2(Object o1, Object o2)
        {
            buf = new StringBuilder();
            tw = new HtmlTableWriter(buf)
            {
                TableAttributes = "border='1'"
            };
            tw.StartTable();
            tw.AddHeaderRow("Property1", o1.GetType().Name, o2.GetType().Name, "Property2");


            var nvs1 = ObjectX.GetPropFieldNameValues(o1);
            var nvs2 = ObjectX.GetPropFieldNameValues(o2);
            var nvs1Keys = nvs1.AllKeys;
            var nvs2Keys = nvs2.AllKeys;

            var nvsSame = nvs1Keys.Where(e => nvs2Keys.Contains(e));
            var nvs1Only = nvs1Keys.Where(e => !nvs2Keys.Contains(e));
            var nvs2Only = nvs2Keys.Where(e => !nvs1Keys.Contains(e));

            foreach (string k in nvsSame)
            {
                object val1 = nvs1[k];
                object val2 = nvs2[k];
                AddProp(k, val1, k, val2);
            }
            foreach (string k in nvs1Only)
            {
                object val1 = nvs1[k];
                AddProp(k, val1, null, null);
            }
            foreach (string k in nvs2Only)
            {
                object val2 = nvs2[k];
                AddProp(null, null, k, val2);
            }

            tw.EndTable();

            //MOutput.WriteObject(o1, "source Object");
            //MOutput.WriteObject(o2, "Dest Object");
            MOutput.WriteLine(buf.ToString());
            return buf.ToString();
        }

        //===================================================================================================
        private void AddProp(PropertyInfo prop1, object v1, PropertyInfo prop2, object v2)
        {
            String prop1Name = (prop1 == null) ? "" : prop1.Name;
            String prop2Name = (prop2 == null) ? "" : prop2.Name;
            if (prop2Name == prop1Name)
            {
                prop2Name = "''";
            }
            string s1 = (v1 == null) ? "null" : v1.ToString();
            string s2 = (v2 == null) ? "null" : v2.ToString();
            if (s2 == s1)
            {
                s2 = "''";
            }
            tw.AddRow_NValues(prop1Name, s1, s2, prop2Name);

        }
        //============================================================================================
        public string CompareObjectsToHtml2x(Object o1, Object o2)
        {

            Type t1 = o1.GetType();
            Type t2 = o2.GetType();

            BindingFlags srcBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            BindingFlags targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

            PropertyInfo[] srcProps = t1.GetProperties(srcBindingFlags);
            //srcProps = t1.GetProperties();
            PropertyInfo[] targetProps = t2.GetProperties(targetBindingFlags);
            tw.TableAttributes = "border='1'";
            tw.StartTable();
            tw.AddHeaderRow("Property1", o1.GetType().Name, o2.GetType().Name, "Property2");

            foreach (PropertyInfo srcProp in srcProps)
            {
                string propName = srcProp.Name;
                object p2Val = null;


                object val = null;
                try
                {
                    val = srcProp.GetValue(o1, null);
                    //[ArgumentException: An item with the same key has already been added.]
                }
                //[TargetParameterCountException: Parameter count mismatch.]
                catch (TargetParameterCountException ex)
                {
                    val = "TargetParameterCountException: " + ex.Message;
                };


                PropertyInfo targetProp = t2.GetProperty(propName);
                if (targetProp == null)
                {
                    //in srcObject only
                    AddProp(srcProp, val, null, "");
                    continue;
                }
                else
                {
                    try
                    {
                        p2Val = targetProp.GetValue(o2, null);
                        //[ArgumentException: An item with the same key has already been added.]
                    }
                    //[TargetParameterCountException: Parameter count mismatch.]
                    catch (TargetParameterCountException ex)
                    {
                        p2Val = "TargetParameterCountException: " + ex.Message;
                    };
                    AddProp(srcProp, val, targetProp, p2Val);
                }
                //continue;

                //if (!targetProp.CanWrite)
                //{
                //    continue;
                //}

                //if (val == null)
                //{
                //    //targetProp.SetValue(o2, null, null);
                //    tw.AddRow_NValues(propName, "null", "x");
                //}
                //else if (targetProp.PropertyType.IsGenericType && targetProp.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                //{
                //    object convertedValue = null;
                //    convertedValue = System.Convert.ChangeType(val, Nullable.GetUnderlyingType(targetProp.PropertyType));
                //    //targetProp.SetValue(o2, convertedValue, null);
                //    tw.AddRow_NValues(propName, convertedValue.ToString(), "z");
                //}
                //else
                //{
                //    tw.AddRow_NValues(propName, val.ToString(), "z2z");

                //    string info1 = string.Format("PropName: '{0}' propVal: {1}", propName, val);
                //    try
                //    {
                //        //targetProp.SetValue(o2, val, null);
                //    }
                //    catch (ArgumentException ex)
                //    {
                //        throw new Exception(info1, ex);
                //    }
                //    catch (TargetException ex) //Object does not match target type.
                //    {
                //        throw new Exception(info1, ex);
                //    }
                //}
            }
            tw.EndTable();

            //MOutput.WriteObject(o1, "source Object");
            //MOutput.WriteObject(o2, "Dest Object");
            MOutput.WriteLine(buf.ToString());
            return buf.ToString();
        }

    }
}
