using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Libx
{
    public static class ObjectX
    {
        public static object GetDefaultValue(Type type)
        {
            ///<see cref="EnumUtils.GetDefaultValue{T}"/>
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static object GetDefault2(Type t)
        {
            return t.GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(null, null);
            //t.getdef
        }


        //===================================================================================================
        public static void SetPropertyValue(object targetObject, PropertyInfo prop, object val)
        {
            //called by ObjectX.Map
            if (!prop.CanWrite)
            {
                throw new Exception("Property to writable: " + prop.Name);
            }

            if (val != null && val != DBNull.Value &&
                prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var convertedValue = Convert.ChangeType(val, Nullable.GetUnderlyingType(prop.PropertyType));
                val = convertedValue;
            }
            //throw new Exception("test");
            string info1 = string.Format("PropName: '{0}' propVal: {1}", prop.Name, val);
            //MOutput.WriteLineFormat("Setting " + info1);
            try
            {
                prop.SetValue(targetObject, val, null);
            }
            catch (ArgumentException ex)
            {
                //System.ArgumentException: Object of type 'System.String' cannot be converted to type 'System.Boolean'
                //MOutput.WriteObject(prop);
                throw new Exception(info1, ex);
            }
            catch (TargetException ex)
            {
                //Object does not match target type.
                //MOutput.WriteObject(prop);
                throw new Exception(info1, ex);
            }
        }
        //===================================================================================================
        public static Object GetPropertyValue(object srcObj, PropertyInfo prop)
        {
            object val = null;
            try
            {
                val = prop.GetValue(srcObj, null);
                //[ArgumentException: An item with the same key has already been added.]
            }
            //[TargetParameterCountException: Parameter count mismatch.]
            catch (TargetParameterCountException ex)
            {
                val = "TargetParameterCountException: " + ex.Message;
            }
            //System.InvalidOperationException: There is already an open DataReader associated with this Command which must be closed first.
            catch (TargetInvocationException ex)
            {
                val = "propName: " + prop.Name + ", mesg=" + ex.Message;
                throw new Exception(val + "");
            }
            return val;
        }
        //============================================================================================
        public static void CopyPropertyValues(Object srcObj, Object targetObj)
        {
            if (srcObj == null)
            {
                throw new Exception("srcObj is null");
            }
            if (targetObj == null)
            {
                throw new Exception("targetObj is null");
            }

            Type t1 = srcObj.GetType();
            Type t2 = targetObj.GetType();

            BindingFlags srcBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

            BindingFlags targetBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

            PropertyInfo[] srcProps = t1.GetProperties(srcBindingFlags);
            //srcProps = t1.GetProperties();
            PropertyInfo[] targetProps = t2.GetProperties(targetBindingFlags);

            foreach (PropertyInfo srcProp in srcProps)
            {
                string propName = srcProp.Name;
                PropertyInfo targetProp = t2.GetProperty(propName);
                if (targetProp == null)
                {
                    continue;
                }
                if (!targetProp.CanWrite)
                {
                    continue;
                }

                //if (!targetProps.Any(e => e.Name == propName))
                //{
                //    //No set property found in target object type.
                //    continue;
                //}
                //MOutput.WriteHtmlTable("targetObj", targetProps);

                object val = GetPropertyValue(srcObj, srcProp);
                SetPropertyValue(targetObj, targetProp, val);

                //if (val == null)
                //{
                //    targetProp.SetValue(targetObj, null, null);
                //}
                //else if (targetProp.PropertyType.IsGenericType && targetProp.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                //{
                //    object convertedValue = null;
                //    convertedValue = System.Convert.ChangeType(val, Nullable.GetUnderlyingType(targetProp.PropertyType));
                //    targetProp.SetValue(targetObj, convertedValue, null);
                //}
                //else
                //{
                //    string info1 = string.Format("PropName: '{0}' propVal: {1}", propName, val);
                //    try
                //    {
                //        targetProp.SetValue(targetObj, val, null);
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
            //ObjectComparer.CompareObjectsToHtml(srcObj, targetObj);
            //MOutput.WriteFormat("Copying properties from src: {0} to target: {1}", t1.Name, t2.Name);
            //MOutput.WriteObject(srcObj, "source Object");
            //MOutput.WriteObject(targetObj, "Dest Object");
        }
        //============================================================================================
        public static string ToString(Object o)
        {
            string format = "{0} = {1}\n";
            //string format = "{0}={1}\n<br/>";

            var nvs = GetPropFieldNameValues(o);
            //string rval = nvs.ToStringX();

            StringBuilder buf = new StringBuilder();
            List<string> keys = nvs.Keys.OfType<String>().ToList();
            keys.Sort();
            foreach (String key in keys)
            {
                Object objVal = nvs[key]; // ?? "";
                string v = (objVal == null) ? "" : objVal.ToString();
                buf.Append(string.Format(format, key, v));
            }
            return buf.ToString();
        }
        //========================================================================
        public static string ToString2(Object o)
        {
            NameValueCollection nvs = new NameValueCollection();
            Type t = typeof(string);

            BindingFlags bindingFlags = BindingFlags.GetProperty;
            bindingFlags = BindingFlags.Default;

            //bindingFlags = BindingFlags.Public + BindingFlags.
            PropertyInfo[] props;
            props = t.GetProperties(bindingFlags);
            props = t.GetProperties();
            foreach (PropertyInfo p in props)
            {

                //if (p.isPu
                //LogX.Debug(p.Name);
                //Log.Debug(p.GetValue(o, null));
            }

            string propertyName = "Length";
            PropertyInfo pi = t.GetProperty(propertyName);
            //Log.Debug(pi == null);
            //LogX.Debug(pi.GetValue(o, null));

            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo m in methods)
            {
                //Log.Debug(m.Name);
                //Log.Debug(p.GetValue(o, null));
            }

            //return NameValueCollectionX.ToString(nvs);
            //StringBuilder buf = new StringBuilder();
            //return buf.ToString();
            return "?";
        }
        //===========================================================
        public static Object Clone(Object obj)
        {
            using (System.IO.MemoryStream memStream = new MemoryStream())
            {
                //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter 
                //System.Runtime.Serialization.StreamingContext
                //System.Runtime.Serialization.StreamingContextStates
                BinaryFormatter binaryFormatter = new BinaryFormatter(null,
                     new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memStream, obj);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj2 = binaryFormatter.Deserialize(memStream);
                return obj2;
            }
        }
        //===========================================================
        public static T DeepCopyObject<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        //===========================================================
        public static string GetSizeStr(Object obj)
        {
            return string.Format("{0:N0}", GetSize(obj));
        }
        //===========================================================
        public static long GetSize(Object obj)
        {
            if (obj == null) { return -1; }
            using (System.IO.MemoryStream memStream = new MemoryStream())
            {
                //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter 
                //System.Runtime.Serialization.StreamingContext
                //System.Runtime.Serialization.StreamingContextStates
                BinaryFormatter binaryFormatter = new BinaryFormatter(null,
                     new StreamingContext(StreamingContextStates.Clone));
                try
                {
                    binaryFormatter.Serialize(memStream, obj);
                }
                catch (SerializationException)
                {
                    return -2;
                }
                memStream.Seek(0, SeekOrigin.Begin);
                return memStream.Length;
            }
        }
        //===================================================================================================
        public static string SerializeObjectToXml(object obj)
        {
            Type objType = obj.GetType();
            XmlSerializer xs = new XmlSerializer(objType);
            string xml;

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xs.Serialize(writer, obj);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }
        //===================================================================================================
        public static T EvaluateProperty<T>(object container, string property)
        {
            //eg: Eval(person, "Address.ZipCode") => return person.Address.ZipCode

            string[] propertyNames = property.Split('.');
            object lastContainer = container;
            foreach (string propName in propertyNames)
            {
                //string currentProperty = propertyNames[i];
                if (string.IsNullOrEmpty(propName))
                {
                    throw new Exception("propName is null or empty");
                }
                PropertyDescriptorCollection descriptorcollection = TypeDescriptor.GetProperties(lastContainer);
                PropertyDescriptor descriptor = descriptorcollection.Find(propName, true);
                lastContainer = descriptor.GetValue(lastContainer);

            }
            return (T)lastContainer;
        }
        //============================================================================================
        public static NameObjectCollection GetPropFieldNameValues(Object o)
        {
            var ret = getPropNameValues(o);
            ret.Add(getFieldNameValues(o));
            return ret;
        }

        //============================================================================================
        public static NameObjectCollection getPropNameValues(Object o)
        {
            bool includeNullableIndicator = true;

            var nvs = new NameObjectCollection();
            Type t = o.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public; // BindingFlags.Public; // | BindingFlags.GetProperty;
            var props = t.GetProperties(bindingFlags);
            foreach (PropertyInfo p in props)
            {
                string propName = p.Name;
                if (includeNullableIndicator)
                {
                    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        propName = propName + "?";
                    }
                }
                object val;
                try
                {
                    val = p.GetValue(o, null);
                }
                catch (TargetParameterCountException ex) //Parameter count mismatch.
                {
                    val = "Error: TargetParameterCountException: " + ex.Message + ", propName: " + propName + ", propType=" + p.PropertyType;
                };
                nvs.Add(p.Name, val);
            }
            return nvs;
        }

        //============================================================================================
        public static NameObjectCollection getFieldNameValues(Object o)
        {
            var nvs = new NameObjectCollection();
            Type t = o.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public; // BindingFlags.Public; // | BindingFlags.GetProperty;

            var props = t.GetFields(bindingFlags);
            foreach (FieldInfo p in props)
            {
                string propName = p.Name;
                //if (includeNullableIndicator)
                //{
                //    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                //    {
                //        propName = propName + "?";
                //    }
                //}
                object val;
                try
                {
                    val = p.GetValue(o);
                }
                catch (TargetParameterCountException ex)
                {
                    val = "Error: TargetParameterCountException: " + ex.Message + ", propName: " + propName + ", propType=" + p.FieldType;
                };
                nvs.Add(p.Name, val);
            }
            return nvs;
        }
        //===================================================================================================
    }
}
