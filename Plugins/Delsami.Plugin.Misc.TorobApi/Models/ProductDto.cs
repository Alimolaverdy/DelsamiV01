using System.Collections.Generic;

namespace Delsami.Plugin.Misc.TorobApi.Models
{
	public class ProductDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        //public int ParentId { get; set; }
        public string Page_Unique { get; set; }
        public string Current_Price { get; set; }
        public string Old_Price { get; set; }
        public string Availability { get; set; }
        public string Category_Name { get; set; }
        public string Image_Link { get; set; }
        public List<string> Image_Links { get; set; } = new List<string>();
        public string Page_Url { get; set; }
        public string Short_Desc { get; set; }
        public List<Dictionary<string, string>> Spec { get; set; } = new List<Dictionary<string, string>>();
        //public DateTime Date { get; set; }
        public string Registry { get; set; }
        public string Guarantee { get; set; }
    }
}
