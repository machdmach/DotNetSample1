namespace System
{

    [Serializable]
    public class GridViewOptions
    {
        public string SortFields { get; set; }
        public string SortDirection { get; set; }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int MaxRows { get; set; }

        //public int MaxRows { get; set; }
    }
}
