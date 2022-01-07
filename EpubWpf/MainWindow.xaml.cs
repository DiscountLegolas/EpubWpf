using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Dapper;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace EpubWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (var item in new SqliteHelper().GetBooks())
            {
                try
                {
                    Book book = GetBook(item.FolderName);
                    AddBookToListView(book);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        private Book GetBook(string foldername)
        {
            List<Sayfa> sayfas = new List<Sayfa>();
            XmlDocument xd = new XmlDocument();
            xd.Load(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + foldername + @"\OEBPS\content.opf");
            XmlNamespaceManager manager = new XmlNamespaceManager(xd.NameTable);
            manager.AddNamespace("def", "http://www.idpf.org/2007/opf");
            manager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            XmlNode metadatanode = xd.SelectSingleNode("/def:package/def:metadata", manager);
            var authorname = metadatanode.SelectSingleNode("//dc:creator", manager).InnerText;
            var bookname = metadatanode.SelectSingleNode("//dc:title", manager).InnerText;
            var imageıtemname = XElement.Parse(metadatanode.OuterXml).Descendants().Where(x => x.Name.LocalName == "meta" && x.Attribute("name") != null && x.Attribute("name").Value == "cover").Select(x => x.Attribute("content").Value).SingleOrDefault();
            var imagefile = xd.SelectSingleNode("/def:package/def:manifest/def:item[@id='" + imageıtemname + "'" + "]", manager).Attributes["href"].Value;
            var imagefilepath = @"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + foldername + @"\OEBPS\" + imagefile;
            Image ımage = new Image() { Source = new BitmapImage(new Uri(imagefilepath)) };
            ımage.Width = 80;
            ımage.Height = 80;
            foreach (XmlNode node in xd.SelectNodes("/def:package/def:spine/def:itemref",manager))
            {
                var idref = node.Attributes["idref"].Value;
                var documentname = xd.SelectSingleNode("/def:package/def:manifest/def:item[@id='" + idref + "'" + "]", manager).Attributes["href"].Value;
                var documentpath = @"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + foldername + @"\OEBPS\" + documentname;
                sayfas.Add(new Sayfa() { Path = documentpath });
            }
            return new Book() { FolderName= foldername, Resim = ımage, Yazar = authorname, BookName = bookname, Sayfalar = sayfas };
        }
        private void AddBook(object sender, RoutedEventArgs e)
        {
            SqliteHelper helper = new SqliteHelper();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".epub";
            fileDialog.Filter = "Epub File (*.epub)|*.epub";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == true)
            {
                string filetoopen = fileDialog.FileName;
                FileInfo file = new FileInfo(fileDialog.FileName);
                string directoryname = file.Name.Replace(file.Extension, "");
                string directorypath = @"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + directoryname;
                if (!Directory.Exists(directorypath))
                {
                    helper.InsertBook(directoryname);
                    if (!file.Extension.Contains("zip"))
                    {
                        file.MoveTo(System.IO.Path.ChangeExtension(file.FullName, ".zip"));
                    }
                    Directory.CreateDirectory(directorypath);
                    ZipFile.ExtractToDirectory(file.FullName, directorypath);
                    file.MoveTo(System.IO.Path.ChangeExtension(file.FullName, ".epub"));
                }
                else
                {
                    return;
                } 
                Book book = GetBook(directoryname);
                AddBookToListView(book);
            }
        }

        private void KitapSeçimi(object sender, SelectionChangedEventArgs e)
        {
            Book book = ((Book)((ListViewItem)Kitaplar.SelectedItem).Tag);
            OkumaWindow okuma = new OkumaWindow(book);
            okuma.Show();
        }
        private void AddBookToListView(Book book)
        {
            ListViewItem ıtem = new ListViewItem();
            ıtem.Tag = book;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(book.Resim);
            stackPanel.Children.Add(new TextBlock() { Text = "     " + book.BookName });
            stackPanel.Children.Add(new TextBlock() { Text = "      " });
            stackPanel.Children.Add(new TextBlock() { Text = book.Yazar });
            ıtem.Content = stackPanel;
            Kitaplar.Items.Add(ıtem);
        }
    }
}
