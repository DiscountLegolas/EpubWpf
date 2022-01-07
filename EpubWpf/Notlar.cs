using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubWpf
{
    public static class Notlar
    {
        public static List<Not> Notes = new List<Not>();
    }
    public class Not
    {
        public string Folder { get; set; }
        public string NotString { get; set; }
        public string Note { get; set; }
        public Not(string folder,string notstring,string note)
        {
            Folder = folder;
            NotString = notstring;
            Note = note;
        }
    }
}
