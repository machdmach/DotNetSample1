using System.Runtime.Serialization;

namespace System
{
    [DataContract]
    public class DeletableEntity
    {
        [DataMember]
        public bool value { get; set; }

        [DataMember]
        public String reasons { get; set; }
    }

    [DataContract]
    public class RecordStatusEntity
    {
        [DataMember]
        public int value { get; set; }

        [DataMember]
        public String stringValue { get; set; }

        [DataMember]
        public String label { get; set; }

        [DataMember]
        public String description { get; set; }
    }

    public enum RecordStatus
    {
        Active = 1,
        Inactive = 0,
    }
    [DataContract]
    public class EntityObjectBase
    {
        [DataMember]
        public int KeyID { get; set; }

        [DataMember]
        public String KeyGUID { get; set; } //KeyGUID uniqueidentifier NOT NULL default NewID()
        //public Guid KeyGUID { get; set; } //KeyGUID uniqueidentifier NOT NULL default NewID()
        //[ExternalID] [uniqueidentifier] NOT NULL,

        //public virtual string GetCreateUrl()
        //{
        //    string rval = string.Format("Create?keyID={0}", KeyID);
        //    return rval;
        //}
        public virtual string GetDetailsUrl()
        {
            string rval = string.Format("Details?keyID={0}", KeyID);
            return rval;
        }
        public virtual string GetEditUrl()
        {
            string rval = string.Format("Edit?keyID={0}", KeyID);
            return rval;
        }
        public virtual string GetDeleteUrl()
        {
            string rval = string.Format("Delete?keyID={0}", KeyID);
            return rval;
        }
        public virtual string GetDeleteLink(bool confirmOnClick = true)
        {
            string deleteLink = string.Format("<a href='{0}' onClick=\"return confirm('Are you sure to delete this record?');\"> Delete </a>", GetDeleteUrl());
            //urlLabel = "<button type='button' class='btn btn-primary btn-danger' onclick='return confirm(\"Are you sure to delete this record?\");'>Delete</button>";
            return deleteLink;
        }

        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
        [DataMember]
        public DateTime? UpdatedOn { get; set; }
        public int RecordVersion { get; set; }
        public int RecordStatus { get; set; }
        public int DisplayOrder { get; set; }

        public string GetAuditInfoHtml()
        {
            var buf = new StringBuilder();

            string lastUpdatedBy = string.IsNullOrEmpty(UpdatedBy) ? CreatedBy : UpdatedBy;
            DateTime lastUpdatedOn = UpdatedOn ?? CreatedOn;
            buf.AppendFormat("Last updated by: {0}, ", lastUpdatedBy);
            buf.AppendFormat("on : {0}", lastUpdatedOn);

            string s = buf.ToString();
            s = string.Format("<p> {0} </p>", s);
            return s;
        }
    }

}
