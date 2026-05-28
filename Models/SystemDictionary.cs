using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_user.Models
{
    public class SystemDictionary
    {
        public long DictionaryID { get; set; }
        public string Category { get; set; }
        public int CodeValue { get; set; }
        public string DisplayNameEnglish { get; set; }
        public int SortOrder { get; set; }
    }
}
