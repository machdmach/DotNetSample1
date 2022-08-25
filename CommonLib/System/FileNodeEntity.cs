using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Libx
{
    [DataContract]
    public partial class FileNodeEntity
    {

        public FileNodeEntity() { }
        //public DocNode(DocNode parent)
        //{
        //    if (parent != null)
        //    {
        //        Level = parent.Level + 1;
        //        //this.Parent = parent;
        //        parent.IsFile = false;

        //        if (parent.Children == null)
        //        {
        //            parent.Children = new List<DocNode>();
        //        }
        //        //Children = new List<DocuShareNode>(); //Children are null by default to save space.
        //        parent.Children.Add(this);
        //    }
        //}
        //===================================================================================================

        [DataMember]
        public String KeyGUID { get; set; } //KeyGUID uniqueidentifier NOT NULL default NewID()

        [DataMember]
        public int Handle { get; set; }
        public int KeyID => Handle;

        [DataMember]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataMember]
        public int ParentHandle { get; set; }

        [DataMember]
        [Display(Name = "Desc")]
        public string Desc { get; set; }

        [DataMember]
        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; }

        [DataMember]
        [Display(Name = "Is File")]
        public bool IsFile { get; set; }

        [DataMember]
        [Display(Name = "File Ext")]
        public string FileExt { get; set; }

        [DataMember]
        [Display(Name = "Level")]
        public int Level { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public virtual string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [DataMember]
        [Display(Name = "Updated On")]
        public DateTime? UpdatedOn { get; set; }

        public DateTime? ModifiedDate => UpdatedOn;


        [DataMember]
        public byte[] BinaryContent { get; set; }

        [DataMember]
        public String TextContent { get; set; }

        [DataMember]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        [Display(Name = "Start Time")]
        public string StartTime { get; set; }

        [DataMember]
        [Display(Name = "End Time")]
        public string EndTime { get; set; }

        [DataMember]
        public int RecordStatus { get; set; }

        [DataMember]
        public int DisplayOrder { get; set; }

        [DataMember]
        public int RecordVersion { get; set; }


        //[DataMember]
        //[Display(Name = "Desc")]
        //public DateTime? DescDate { get; set; }

        //public DateTime ConvertDescDatezz()
        //{
        //    DateTime dt;
        //    if (!DateTime.TryParse(Desc, out dt))
        //    {
        //        throw new Exception("Invalid Date: " + Desc + " for handle: " + Handle);
        //    }
        //    return dt;
        //}


        //public string FilePath { get; set; }
        //public string Url { get; set; }
        //http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/
        //[NonSerialized]
        //[XmlIgnore]
        //public List<DocNode> Children;

        //public DocNode Parent;



    }

}
