using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RussJudge.WPFControlsAndStylizer"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RussJudge.WPFControlsAndStylizer;assembly=RussJudge.WPFControlsAndStylizer"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class ActivityIndicatorX : Canvas
    {
       
        public ActivityIndicatorX()
        {
            this.DefaultStyleKey = typeof(ActivityIndicatorX);
            this.Color1 = Brushes.Maroon;
            this.Color2 = Brushes.Magenta;
        }
        static ActivityIndicatorX()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ActivityIndicatorX),
                new FrameworkPropertyMetadata(typeof(ActivityIndicatorX))
                );
        }

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActivityIndicatorX me)
            {
                me.RenderArc();
            }
        }

        public static readonly DependencyProperty EllipseBackgroundProperty =
            DependencyProperty.Register(nameof(EllipseBackground), typeof(Brush),
            typeof(ActivityIndicatorX), new PropertyMetadata(Brushes.Gray, OnBrushChanged));

        public Brush EllipseBackground
        {
            get
            {
                return (Brush)this.GetValue(EllipseBackgroundProperty);
            }
            set
            {
                this.SetValue(EllipseBackgroundProperty, value);
            }
        }


        public static readonly DependencyProperty Color1Property =
            DependencyProperty.Register(nameof(Color1), typeof(Brush),
            typeof(ActivityIndicatorX), new PropertyMetadata(Brushes.Blue, OnBrushChanged));

        public Brush Color1
        {
            get
            {
                return (Brush)this.GetValue(Color1Property);
            }
            set
            {
                this.SetValue(Color1Property, value);
            }
        }

        public static readonly DependencyProperty Color2Property =
            DependencyProperty.Register(nameof(Color2), typeof(Brush),
            typeof(ActivityIndicatorX), new PropertyMetadata(Brushes.Plum, OnBrushChanged));

        public Brush Color2
        {
            get
            {
                return (Brush)this.GetValue(Color2Property);
            }
            set
            {
                this.SetValue(Color2Property, value);
            }
        }


        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(ActivityIndicatorX), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActivityIndicatorX me)
            {
                if (me.IsActive)
                {
                    me.StartArcAnimation();
                }
                else
                {
                    me.StopArcAnimation();
                }
            }
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


        private RotateTransform? _rotateTransform;
        private RotateTransform? _rotateTransform2;
        private void RenderArc()
        {

            double baseline = ActualWidth;
            if (ActualHeight < baseline)
            {
                baseline = ActualHeight;
            }
            if (baseline == 0)
            {
                baseline = 32;
            }



            double workItem = baseline * 1.5;

            this.Children.Clear();


            double adjust = workItem * 0.1;
            double adjust1 = workItem * 0.2;


            double radius1 = (workItem * 0.9 - adjust) / 2;
            double radius = (workItem * 0.9) / 2;

            Vector center = new(radius1, radius1);

            double start_angle = 0;


            double end_angle = Math.PI;
            double angle_diff = end_angle - start_angle;

            Ellipse ellipse = new()
            {
                Width = workItem,
                Height = workItem,
                Opacity = 0.5,
                Stroke = EllipseBackground,
                StrokeThickness = adjust1
            };


            this.Children.Add(ellipse);

            ArcSegment arcSegment = new()
            {
                IsLargeArc = angle_diff >= Math.PI,
                Point = new Point(center.X + radius * Math.Cos(end_angle), center.Y + radius * Math.Sin(end_angle)),

                Size = new Size(radius1, radius1),
                SweepDirection = SweepDirection.Clockwise
            };


            PathFigure pathFigure = new()
            {
                //Set start of arc
                StartPoint = new Point(center.X + radius * Math.Cos(start_angle), center.Y + radius * Math.Sin(start_angle))
            };
            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new();
            pathGeometry.Figures.Add(pathFigure);

            _rotateTransform = new(0, center.X, center.Y);

            var arc_path = new Path
            {
                Opacity = 0.75,
                Stroke = Color1,
                StrokeThickness = adjust1,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                Data = pathGeometry,
                RenderTransform = _rotateTransform
            };

            Canvas.SetLeft(arc_path, adjust);
            Canvas.SetTop(arc_path, adjust);

            this.Children.Add(arc_path);

            _rotateTransform2 = new(0, center.X, center.Y);
            arc_path = new Path
            {
                Opacity = 1,
                Stroke = Color2,
                StrokeThickness = adjust1,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                Data = pathGeometry,
                RenderTransform = _rotateTransform2
            };

            Canvas.SetLeft(arc_path, adjust);
            Canvas.SetTop(arc_path, adjust);
            this.Children.Add(arc_path);
            if (IsActive)
            {
                StartArcAnimation();
            }
            else
            {
                StopArcAnimation();
            }
        }
        private void StopArcAnimation()
        {
            _rotateTransform?.BeginAnimation(RotateTransform.AngleProperty, null);
            _rotateTransform2?.BeginAnimation(RotateTransform.AngleProperty, null);
        }
        private void StartArcAnimation()
        {
            if (_rotateTransform != null)
            {
                // Create the rotation animation
                DoubleAnimation rotateAnimation = new()
                {
                    From = 0,
                    To = 360,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                // Apply the animation to the RotateTransform's AngleProperty
                _rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
            if (_rotateTransform2 != null)
            {
                // Create the rotation animation
                DoubleAnimation rotateAnimation = new()
                {
                    From = 360,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(800)),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                // Apply the animation to the RotateTransform's AngleProperty
                _rotateTransform2.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RenderArc();

        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderArc();
        }
    }
}