namespace DS.BusinessObject.User
{
    public class FormSearchUser
    {
        public string COMTAXCODE { get; set; }
        public string KEYWORD { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
