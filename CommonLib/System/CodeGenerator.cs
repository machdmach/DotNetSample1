using System.Linq;

namespace Libx
{
    public class CodeGenerator //#code
    {
        public void GenCodeAll()
        {
            GenCode_HtmlFormFields();
            GenCode_GetSampleData();

            GenerateCode_GetSet_Properties();
            GenerateCode_GetSet_Properties_OneLine();
            GenerateCode_SQLInsertUpdateParams();
            GenerateCode_SQLCreateTable();
            GenerateCode_Audit_AddField();
            GenerateCode_zz();

            Generate_TypeScript_Code();

        }
        public void GenCode_HtmlFormFields()
        {
            string template = @"  
          <div class='fRow'>
            <div class='fCell-6'>
                <label> {label}: </label>
                <input type='text' name='{name}' maxlength='200' class='ff-control'>
                <span class='val-err' />
            </div>
          </div>
";
            string templateDate = @"  
          <div class='fRow'>
            <div class='fCell-6'>
                <label> {label}: </label>
                <input type='date' name='{name}' maxlength='10' class='ff-control'>
                <span class='val-err' />
            </div>
          </div>
";

            GenCode("", (def) =>
            {
                //return $"{def.Name}?: {def.JSType}| null | undefined;";
                string s = template;
                if (def.Type == typeof(DateTime))
                {
                    s = templateDate;
                }
                s = s.TrimEnd();
                s = s.Replace("{label}", def.Label);
                s = s.Replace("{name}", def.Name);
                return s;
            });

        }
        //===================================================================================================
        public void Generate_TypeScript_Code()
        {
            GenCode("", (def) =>
            {
                return $"{def.Name}?: {def.jsPrimitiveType}| null | undefined;";
            });

            GenCode("", (def) =>
            {
                return $"{def.Name}: {def.jsPrimitiveType} = '';";
            });

            GenCode("", (def) =>
            {
                return string.Format("g.addCell(rec.{0}, \"{0}\");", def.Name);
            });

        }
        //===================================================================================================
        //string indent = "      ";
        public string GenerateCode_GetSet_Properties()
        {
            return GenerateCode("", (DataFieldDef def, StringBuilder buf) =>
            {
                buf.AppendLine("[DataMember]");
                //buf.AppendFormat(indent + "[Display(Name=\"{0}\")]\n", def.Label);
                //if (options.IncludeAttribute_Required)
                //{
                //    buf.AppendLine(indent + "[Required]");
                //}
                string typeName = def.TypeShortName;
                if (typeName != "string" && !def.Required)
                {
                    typeName += "?";
                }
                buf.AppendFormat("public {0} {1} {{ get; set; }}\n\n", typeName, def.Name);
                //buf.AppendLine();
            });

        }
        //===================================================================================================
        //string indent = "      ";
        public string GenerateCode_GetSet_Properties_OneLine()
        {
            return GenerateCode("", (DataFieldDef def, StringBuilder buf) =>
            {
                string typeName = def.TypeShortName;
                if (typeName != "string" && !def.Required)
                {
                    typeName += "?";
                }
                buf.AppendFormat("public {0} {1} {{ get; set; }}\n", typeName, def.Name);
            });
        }
        //===================================================================================================
        public string GenerateCode_Audit_AddField()
        {
            return GenerateCode("", (DataFieldDef def, StringBuilder buf) =>
            {
                buf.AppendFormat("b.AddField(e => e.{0});\n", def.Name);
            });

        }

        //===================================================================================================
        public void GenCode_GetSampleData()
        {
            //FirstName = "Test";            
            GenCode(string.Format("public static {0} GetSampleData() {{\nvar e = new {0}();", type.Name), (def) =>
            {
                string val = DataFieldDefHelper.getSampleValueForDef(def);
                return $"e.{def.Name} = {val};";
            }, "return e;\n}");

            GenCode("", (def) =>
            {
                string val = DataFieldDefHelper.getSampleValueForDef(def, "json");
                string line = $"{def.Name}: {val},";
                if (def == defs[defs.Count - 1])
                {
                    //line = line.TrimEnd(',');
                }
                return line;
            });

        }

        //===================================================================================================
        public void GenerateCode_zz()
        {
            if (RunThis) return;

            //FirstName = "Test";            
            GenCode(string.Format("public {0} GetSampleData() {{\nvar e = new {0}();", type.Name), (def) =>
            {
                string val = DataFieldDefHelper.getSampleValueForDef(def);
                return $"e.{def.Name} = {val};";
            }, "return e;\n}");

        }


        //===================================================================================================
        public void GenerateCode_SQLInsertUpdateParams()
        {
            GenerateCode("", (def, buf) =>
            {
                var pi = def;

                if (pi.PropertyType == typeof(String))
                {
                    buf.AppendFormat("bldr.SetString(\"{0}\", ent.{0});\n", pi.Name);
                    //bldr.SetString("CommentType", ent.CommentType); 
                }
                else if (pi.PropertyType == typeof(DateTime) ||
                    pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                       && Nullable.GetUnderlyingType(pi.PropertyType) == typeof(DateTime))
                {

                    buf.AppendFormat("bldr.SetDateTime(\"{0}\", ent.{0});\n", pi.Name);
                    //bldr.SetDateTime("CreatedOn", ent.CreatedOn);
                }
                else
                {
                    buf.AppendFormat("bldr.SetValue(\"{0}\", ent.{0});\n", pi.Name);
                    //bldr.SetValue("FeedbackID", ent.CrmID);
                }
            });
        }
        //===================================================================================================
        public void GenerateCode_SQLCreateTable()
        {
            var sqlReservedWords = new string[] { "Name", "Description", "Image", "Url" };

            GenerateCode("", (def, buf) =>
            {
                var pi = def;

                string sqlType = "varchar";
                if (pi.PropertyType == typeof(String))
                {
                }
                else if (pi.PropertyType == typeof(DateTime) ||
                    pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                       && Nullable.GetUnderlyingType(pi.PropertyType) == typeof(DateTime))
                {
                    sqlType = "datetime";
                }
                else
                {
                }
                if (pi.PropertyType == typeof(String) || pi.PropertyType.IsPrimitive)
                {
                    //Exception  varchar(max) null,
                    var colName = pi.Name;
                    colName = StringX.ToTitleCase(colName);

                    if (sqlReservedWords.Any(e => e.Equals(colName, StringComparison.OrdinalIgnoreCase)))
                    {
                        colName = string.Format("[{0}]", colName);
                    }
                    buf.AppendFormat("    {0}  {1}(1),\n", colName, sqlType);
                }
                else
                {
                    buf.AppendFormat("    --{0}  {1},\n", pi.Name, pi.PropertyType.Name);
                }
            });
        }

        //===================================================================================================
        //===================================================================================================
        //===================================================================================================
        //===================================================================================================
        public List<DataFieldDef> defs;
        public Type type;

        //protected String header, footer;
        //===================================================================================================
        public CodeGenerator() { }
        public CodeGenerator(Type csType)
        {
            type = csType;
            defs = DataFieldDef.GetDataFieldDefs(csType);
            MOutput.WriteHtmlTable("defs for: " + csType.FullName, defs);
        }
        //===================================================================================================
        public CodeGenerator(Dictionary<string, object> dict)
        {
            type = typeof(object);
            defs = DataFieldDef.GetDataFieldDefs(dict);
            MOutput.WriteHtmlTable("defs", defs);
        }

        //===================================================================================================
        private string WriteResult(StringBuilder buf)
        {
            MOutput.Write_TextArea(buf.ToString());
            return buf.ToString();
        }
        //===================================================================================================
        public string GenerateCode(string header, Action<DataFieldDef, StringBuilder> processPropertyCallback, string footer = null)
        {
            var buf = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(header))
            {
                buf.AppendLine(header);
            }
            foreach (var def in defs)
            {
                processPropertyCallback(def, buf);
            }
            if (!string.IsNullOrWhiteSpace(header))
            {
                buf.AppendLine(header);
            }
            MOutput.Write_TextArea(buf.ToString());
            return buf.ToString();
        }
        //===================================================================================================
        public string GenCode(string header, Func<DataFieldDef, string> getLineStrCallback, string footer = null)
        {
            var buf = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(header))
            {
                buf.AppendLine(header);
            }
            foreach (var def in defs)
            {
                string line = getLineStrCallback(def);
                if (line != null)
                {
                    buf.AppendLine(line);
                }
            }
            if (!string.IsNullOrWhiteSpace(footer))
            {
                buf.AppendLine(footer);
            }
            MOutput.Write_TextArea(buf.ToString());
            return buf.ToString();
        }

        //===================================================================================================
        public static bool IsFalse(bool? val)
        {
            if (!val.HasValue)
            {
                return true;
            }
            else
            {
                return !val.Value;
            }
        }
        //===================================================================================================
        public static CodeGenerator FromMsSql(string mssql)
        {
            if (NotRunThis) DataFieldDef.GetDataFieldDefs_mssqlScript_Test();

            var ret = new CodeGenerator
            {
                defs = DataFieldDef.GetDataFieldDefs(mssql),
                type = typeof(object)
            };
            MOutput.WriteHtmlTable("defs", ret.defs);
            return ret;
        }
    }
}
