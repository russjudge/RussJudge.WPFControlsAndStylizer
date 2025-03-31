using System.Drawing;
using System.Reflection;
using System.Windows;


namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        private About(Assembly assembly)
        {
            var nm = assembly.GetName();

            var vers = nm.Version;
            var companyAttrib = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            var copyrightAttrib = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var descriptionAttrib = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            var fileVersionAttrib = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var productAttrib = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            var titleAttrib = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var versionAttrib = assembly.GetCustomAttribute<AssemblyVersionAttribute>();

            if (titleAttrib != null)
            {
                AppTitle = titleAttrib.Title;

            }
            if (vers != null)
            {
                NameVersion = vers.ToString();
            }
            if (companyAttrib != null)
            {
                Company = companyAttrib.Company;
            }
            if (copyrightAttrib != null)
            {
                Copyright = copyrightAttrib.Copyright;
            }
            if (descriptionAttrib != null)
            {
                Description = descriptionAttrib.Description;
            }
            if (fileVersionAttrib != null)
            {
                FileVersion = fileVersionAttrib.Version;
            }
            if (productAttrib != null)
            {
                Product = productAttrib.Product;
            }
            if (versionAttrib != null)
            {
                Version = versionAttrib.Version;
            }


            InitializeComponent();


        }
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register(nameof(Version), typeof(string),
            typeof(About));


        public string Version
        {
            get
            {
                return (string)this.GetValue(VersionProperty);
            }
            set
            {
                this.SetValue(VersionProperty, value);
            }
        }

        public static readonly DependencyProperty ProductProperty =
            DependencyProperty.Register(nameof(Product), typeof(string),
            typeof(About));


        public string Product
        {
            get
            {
                return (string)this.GetValue(ProductProperty);
            }
            set
            {
                this.SetValue(ProductProperty, value);
            }
        }

        public static readonly DependencyProperty FileVersionProperty =
            DependencyProperty.Register(nameof(FileVersion), typeof(string),
            typeof(About));


        public string FileVersion
        {
            get
            {
                return (string)this.GetValue(FileVersionProperty);
            }
            set
            {
                this.SetValue(FileVersionProperty, value);
            }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string),
            typeof(About));


        public string Description
        {
            get
            {
                return (string)this.GetValue(DescriptionProperty);
            }
            set
            {
                this.SetValue(DescriptionProperty, value);
            }
        }


        public static readonly DependencyProperty OtherInfoProperty =
            DependencyProperty.Register(nameof(OtherInfo), typeof(string),
            typeof(About));


        public string OtherInfo
        {
            get
            {
                return (string)this.GetValue(OtherInfoProperty);
            }
            set
            {
                this.SetValue(OtherInfoProperty, value);
            }
        }


        public static readonly DependencyProperty CopyrightProperty =
            DependencyProperty.Register(nameof(Copyright), typeof(string),
            typeof(About));


        public string Copyright
        {
            get
            {
                return (string)this.GetValue(CopyrightProperty);
            }
            set
            {
                this.SetValue(CopyrightProperty, value);
            }
        }
        public static readonly DependencyProperty CompanyProperty =
            DependencyProperty.Register(nameof(Company), typeof(string),
            typeof(About));


        public string Company
        {
            get
            {
                return (string)this.GetValue(CompanyProperty);
            }
            set
            {
                this.SetValue(CompanyProperty, value);
            }
        }

        public static readonly DependencyProperty AppTitleProperty =
            DependencyProperty.Register(nameof(AppTitle), typeof(string),
            typeof(About));


        public string AppTitle
        {
            get
            {
                return (string)this.GetValue(AppTitleProperty);
            }
            set
            {
                this.SetValue(AppTitleProperty, value);
            }
        }


        public static readonly DependencyProperty NameVersionProperty =
            DependencyProperty.Register(nameof(NameVersion), typeof(string),
            typeof(About));


        public string NameVersion
        {
            get
            {
                return (string)this.GetValue(NameVersionProperty);
            }
            set
            {
                this.SetValue(NameVersionProperty, value);
            }
        }
        /// <summary>
        /// Displays the About window.
        /// </summary>
        /// <param name="otherDescriptiveInfo">Any descriptive information to display in addition to standard information from the assembly.</param>
        /// <param name="assembly">The Assembly from which to get information.  Default is the Entry Assembly.</param>
        public static void DisplayAbout(string otherDescriptiveInfo = "", Assembly? assembly = null)
        {
            Assembly? assm = null;
            if (assembly == null)
            {
                var asm = System.Reflection.Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    assm = asm;
                }
                else
                {
                    return;
                }
            }
            if (assm != null)
            {
                About win = new(assm)
                {
                    OtherInfo = otherDescriptiveInfo
                };
                win.Show();
            }

        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"About {AppTitle} v. {NameVersion}";
        }
    }
}
