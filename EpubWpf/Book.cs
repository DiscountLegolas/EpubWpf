using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EpubWpf
{
    public class Book
    {
        public string FolderName { get; set; }
        public Image Resim { get; set; }
        public string BookName { get; set; }
        public string Yazar { get; set; }
        public double PersentageRead { get; set; }
        public List<Sayfa> Sayfalar { get; set; }
    }
}
