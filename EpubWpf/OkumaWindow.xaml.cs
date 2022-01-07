using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace EpubWpf
{
    /// <summary>
    /// Interaction logic for OkumaWindow.xaml
    /// </summary>
    public partial class OkumaWindow : Window
    {
        public string selected;
        private Book book;
        public OkumaWindow(Book book)
        {
            InitializeComponent();
            BookMark.Source = new BitmapImage(new Uri(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Images\bookmark.png"));
            FontUp.Source = new BitmapImage(new Uri(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Images\font_size_more.png"));
            FontDown.Source = new BitmapImage(new Uri(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Images\font_size_less.png"));
            this.book = book;
            OkumaSayfası.Width = (this.Width / 10.0) * 9;
            OkumaSayfası.Height = this.Height;
            if (!File.Exists(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\"+book.FolderName+@"\OEBPS\Combined.html"))
            {
                var xml1 = XDocument.Load(book.Sayfalar[0].Path);
                XElement scriptnode = new XElement("script");
                scriptnode.Add(new XAttribute("src", "../../main.js"));
                scriptnode.Value = "";
                xml1.Descendants().First(x=>x.Name.LocalName=="body").Add(new XElement(scriptnode));
                string txt = xml1.Descendants().First().ToString();
                foreach (Sayfa sayfa in book.Sayfalar.Where(x=>x.Path!=book.Sayfalar.First().Path))
                {
                    var xml = XDocument.Load(sayfa.Path);
                    xml.Descendants().First(x => x.Name.LocalName == "body").Add(new XElement(scriptnode));
                    txt += xml.Descendants().First().ToString();
                }
                File.WriteAllText(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + book.FolderName + @"\OEBPS\Combined.html", txt);
            }
            OkumaSayfası.Source = new Uri(@"C:\Users\VESTEL\source\repos\EpubWpf\EpubWpf\Books\" + book.FolderName + @"\OEBPS\Combined.html");
            OkumaSayfası.ObjectForScripting = new ScriptManager(this);
        }
        public void GetSelected(string a)
        {
            selected = a;
        }
        public class ScriptManager
        {
            private OkumaWindow okumaWindow;
            public ScriptManager(OkumaWindow okumaWindow)
            {
                this.okumaWindow = okumaWindow;
            }
            public void GetSelected(string a)
            {
                okumaWindow.selected = a;
                okumaWindow.NotMenü();
                
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string notst = new InputBox("Enter Your Note Text").ShowDialog();
            if (notst.Length!=0)
            {
                Notlar.Notes.Add(new Not(book.FolderName, selected, notst));
            }
        }
        private void NotMenü()
        {
            ContextMenu contextMenu = FindResource("cm") as ContextMenu;
            if (contextMenu == null) return;
            contextMenu.PlacementTarget = OkumaSayfası;
            contextMenu.IsOpen = true;
        }
        public class InputBox
        {

            Window Box = new Window();//window for the inputbox
            FontFamily font = new FontFamily("Tahoma");//font for the whole inputbox
            int FontSize = 30;//fontsize for the input
            StackPanel sp1 = new StackPanel();// items container
            string title = "InputBox";//title as heading
            string boxcontent;//title
            string defaulttext = "Write here your name...";//default textbox content
            string errormessage = "Invalid answer";//error messagebox content
            string errortitle = "Error";//error messagebox heading title
            string okbuttontext = "OK";//Ok button content
            string cancelbutton = "Cancel";
            Brush BoxBackgroundColor = Brushes.White;// Window Background
            Brush InputBackgroundColor = Brushes.Ivory;// Textbox Background
            bool clicked = false;
            TextBox input = new TextBox();
            Button ok = new Button();
            Button cancel = new Button();
            bool inputreset = false;

            public InputBox(string content)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Error!"; }
                windowdef();
            }

            public InputBox(string content, string Htitle, string DefaultText)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Error!"; }
                try
                {
                    title = Htitle;
                }
                catch
                {
                    title = "Error!";
                }
                try
                {
                    defaulttext = DefaultText;
                }
                catch
                {
                    DefaultText = "Error!";
                }
                windowdef();
            }

            public InputBox(string content, string Htitle, string Font, int Fontsize)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Error!"; }
                try
                {
                    font = new FontFamily(Font);
                }
                catch { font = new FontFamily("Tahoma"); }
                try
                {
                    title = Htitle;
                }
                catch
                {
                    title = "Error!";
                }
                if (Fontsize >= 1)
                    FontSize = Fontsize;
                windowdef();
            }

            private void windowdef()// window building - check only for window size
            {
                Box.Height = 500;// Box Height
                Box.Width = 300;// Box Width
                Box.Background = BoxBackgroundColor;
                Box.Title = title;
                Box.Content = sp1;
                Box.Closing += Box_Closing;
                TextBlock content = new TextBlock();
                content.TextWrapping = TextWrapping.Wrap;
                content.Background = null;
                content.HorizontalAlignment = HorizontalAlignment.Center;
                content.Text = boxcontent;
                content.FontFamily = font;
                content.FontSize = FontSize;
                sp1.Children.Add(content);

                input.Background = InputBackgroundColor;
                input.FontFamily = font;
                input.FontSize = FontSize;
                input.HorizontalAlignment = HorizontalAlignment.Center;
                input.MinWidth = 200;
                input.MouseEnter += input_MouseDown;
                sp1.Children.Add(input);
                ok.Width = 70;
                ok.Height = 30;
                ok.Click += ok_Click;
                ok.Content = okbuttontext;
                ok.HorizontalAlignment = HorizontalAlignment.Center;
                sp1.Children.Add(ok);
                cancel.Width = 70;
                cancel.Height = 30;
                cancel.Click += CancelButtonClick;
                cancel.Content = cancelbutton;
                cancel.HorizontalAlignment = HorizontalAlignment.Center;
                sp1.Children.Add(cancel);

            }

            void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                clicked = true;
                if (!clicked)
                    e.Cancel = true;
            }

            private void input_MouseDown(object sender, MouseEventArgs e)
            {
                if ((sender as TextBox).Text == defaulttext && inputreset == false)
                {
                    (sender as TextBox).Text = null;
                    inputreset = true;
                }
            }
            void CancelButtonClick(object sender,RoutedEventArgs e)
            {
                clicked = true;
                Box.Close();
            }
            void ok_Click(object sender, RoutedEventArgs e)
            {
                clicked = true;
                if (input.Text == defaulttext || input.Text == "")
                    MessageBox.Show(errormessage, errortitle);
                else
                {
                    Box.Close();
                }
                clicked = false;
            }

            public string ShowDialog()
            {
                Box.ShowDialog();
                return input.Text;
            }
        }

        private void ChangeOK(object sender, SizeChangedEventArgs e)
        {
            OkumaSayfası.Width = (this.ActualWidth / 10.0) * 9;
            OkumaSayfası.Height = this.ActualHeight;
        }

        private void FontDownClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Font Down Clicked");
        }

        private void FontUpClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Font Up Clicked");
        }

        private void BookMarkClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Bookmark Clicked");

        }
    }
}
