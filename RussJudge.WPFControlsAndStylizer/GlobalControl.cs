using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RussJudge.WPFControlsAndStylizer
{
    public class GlobalControl : DependencyObject 
    {
        private GlobalControl() { }
        public static GlobalControl Current { get; set; } = new();

        public static readonly DependencyProperty IsActiveProperty =
           DependencyProperty.Register(nameof(IsActive), typeof(bool),
           typeof(GlobalControl), new PropertyMetadata(false, OnChange));

        private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public bool IsActive
        {
            get
            {
                return (bool)this.GetValue(IsActiveProperty);
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }
    }
}
