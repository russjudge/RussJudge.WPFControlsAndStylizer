using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// Interaction logic for ActivityIndicator.xaml
    /// </summary>
    public partial class ActivityIndicator : UserControl
    {
        public ActivityIndicator()
        {

            InitializeComponent();
        }

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActivityIndicator me)
            {
                me.RenderArc();
            }
        }

        public static readonly DependencyProperty EllipseBackgroundProperty =
            DependencyProperty.Register(nameof(EllipseBackground), typeof(Brush),
            typeof(ActivityIndicator), new PropertyMetadata(Brushes.Gray, OnBrushChanged));

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
            typeof(ActivityIndicator), new PropertyMetadata(Brushes.Blue, OnBrushChanged));

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
            typeof(ActivityIndicator), new PropertyMetadata(Brushes.Plum, OnBrushChanged));

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
            typeof(ActivityIndicator), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActivityIndicator me)
            {
                if (me.isRendered)
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
                else
                {
                    me.RenderArc();
                }
            }
        }
        bool isRendered = false;
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
            if (MyContent == null)
            {
                return;
            }
            double baseline = ActualWidth;
            if (ActualHeight < baseline)
            {
                baseline = ActualHeight;// * 0.5;
            }
            if (baseline == 0)
            {

                baseline = 32;
            }

            double workItem = baseline * 0.75; // 0.9;// * 0.75; //* 1.5;

            double workWidth = ActualWidth * 0.75;
            double workHeight = ActualHeight * 0.75;


            MyContent.Children.Clear();


            double adjust = workItem * 0.1;  //top-left corner placement.
            double adjustWidth = workWidth * 0.1;
            double adjustHeight = workHeight * 0.1;


            double strokethickness = workItem * 0.2;  //Stroke thinkness

            double radius = (workItem * 0.9) / 2;
            double radiusWidth = (workWidth * 0.9) / 2;
            double radiusHeight = (workHeight * 0.9) / 2;

            //Vector center = new(radius + adjust, radius + adjust);
            Vector center = new(radiusWidth + adjustWidth, radiusHeight + adjustHeight);


            const double start_angle = 0;
            const double end_angle = Math.PI;


            //Background circle.
            Ellipse ellipse = new()
            {
                Margin = new Thickness(adjustWidth, adjustHeight, 0, 0),
                Width = workWidth * 1.1,
                Height = workHeight * 1.1,
                Opacity = 0.5,
                Stroke = EllipseBackground,
                StrokeThickness = strokethickness
            };


            MyContent.Children.Add(ellipse);



            //First Arc
            ArcSegment arcSegment = new()
            {
                IsLargeArc = false,
                //Point = new Point(center.X + radius * Math.Cos(end_angle), center.Y + radius * Math.Sin(end_angle)),
                Point = new Point(center.X + radiusWidth * Math.Cos(end_angle), center.Y + radiusHeight * Math.Sin(end_angle)),

                Size = new Size(radiusWidth, radiusHeight),
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
                StrokeThickness = strokethickness,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                Data = pathGeometry,
                RenderTransform = _rotateTransform
            };

            Canvas.SetLeft(arc_path, adjustWidth);
            Canvas.SetTop(arc_path, adjustHeight);



            MyContent.Children.Add(arc_path);

            _rotateTransform2 = new(0, center.X, center.Y);
            arc_path = new Path
            {
                Opacity = 1,
                Stroke = Color2,
                StrokeThickness = strokethickness,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                Data = pathGeometry,
                RenderTransform = _rotateTransform2
            };

            Canvas.SetLeft(arc_path, adjustWidth);
            Canvas.SetTop(arc_path, adjustHeight);


            MyContent.Children.Add(arc_path);
            if (IsActive)
            {
                //Visibility = Visibility.Visible;
                StartArcAnimation();
            }
            else
            {
                StopArcAnimation();
                // Visibility= Visibility.Collapsed;   
            }
            isRendered = true;
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderArc();


        }
    }
}