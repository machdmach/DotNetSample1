using System.Data.Common;

namespace System
{
    public class SampleData
    {
        public static byte[] GetBlankPDFBytes()
        {
            //ent.UploadedFileName = "BlankDoc.pdf";
            //string pfname = EnvironmentX.MapPath("~/App_Data/") + ent.UploadedFileName;
            string pfname = @"\\svfs-pfile01\PublicDocuments\assets\CDN\SampleData\BlankDoc.pdf";
            var BinaryContent = System.IO.File.ReadAllBytes(pfname);
            return BinaryContent;
        }
    }

    //===================================================================================================
    public class SampleEntity
    {
        protected string protectedPropString { get; set; } = "An protected string";

        [DbCol("cc", NoInsert = true)]
        public string InstancePropString { get; set; } = "An instance string";

        public static string StaticPropString { get; set; } = "a static string";

        [DbCol(NoInsert = true, NoUpdate = true)]
        public string instanceField = "instance field1";

        public static string staticInstanceField = "static instance field1";

        public int? aNullableInt32 { get; set; }

        //===================================================================================================
        public static void Test1()
        {
            Int16 x2 = 33;

            var sd = new SampleEntity();
            var pc = sd.GetType().GetProperty("aNullableInt32");
            pc.SetValue(sd, Convert.ChangeType(x2, typeof(Int32)));
            pc.SetValue(sd, null);

            MOutput.WriteObject(sd);
        }
    }
}
