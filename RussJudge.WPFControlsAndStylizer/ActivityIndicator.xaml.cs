using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RussJudge.WPFControlsAndStylizer
{
    /// <summary>
    /// A spinner that indicates activity while <see cref="IsActive"/> is true.
    /// </summary>
    public partial class ActivityIndicator : UserControl
    {
        // The animations never change, so every indicator shares the same two frozen instances.
        private static readonly DoubleAnimation ClockwiseSpin = CreateSpin(0, 360, 1000);
        private static readonly DoubleAnimation CounterClockwiseSpin = CreateSpin(360, 0, 800);

        // The shapes are created once and then only updated.
        private readonly RotateTransform _rotateTransform = new();
        private readonly RotateTransform _rotateTransform2 = new();
        private readonly Ellipse _ellipse = new() { Opacity = 0.5 };
        private readonly Path _arc = CreateArc(0.75);
        private readonly Path _arc2 = CreateArc(1);
        private bool _isSpinning;

        public ActivityIndicator()
        {
            InitializeComponent();

            _ellipse.Stroke = EllipseBackground;
            _arc.Stroke = Color1;
            _arc.RenderTransform = _rotateTransform;
            _arc2.Stroke = Color2;
            _arc2.RenderTransform = _rotateTransform2;

            MyContent.Children.Add(_ellipse);
            MyContent.Children.Add(_arc);
            MyContent.Children.Add(_arc2);

            IsVisibleChanged += OnIsVisibleChanged;
            UpdateShapes();
        }

        private static DoubleAnimation CreateSpin(double from, double to, double milliseconds)
        {
            DoubleAnimation animation = new()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            animation.Freeze();
            return animation;
        }

        private static Path CreateArc(double opacity)
        {
            return new Path
            {
                Opacity = opacity,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round
            };
        }

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActivityIndicator me)
            {
                // Only the stroke of the affected shape changes; nothing has to be rebuilt.
                Brush brush = (Brush)e.NewValue;
                if (e.Property == EllipseBackgroundProperty)
                {
                    me._ellipse.Stroke = brush;
                }
                else if (e.Property == Color1Property)
                {
                    me._arc.Stroke = brush;
                }
                else if (e.Property == Color2Property)
                {
                    me._arc2.Stroke = brush;
                }
            }
        }

        public static readonly DependencyProperty EllipseBackgroundProperty =
            DependencyProperty.Register(nameof(EllipseBackground), typeof(Brush),
            typeof(ActivityIndicator), new PropertyMetadata(Brushes.Gray, OnBrushChanged));

        /// <summary>
        /// The brush of the ring behind the two spinning arcs.
        /// </summary>
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

        /// <summary>
        /// The brush of the arc that spins clockwise.
        /// </summary>
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

        /// <summary>
        /// The brush of the arc that spins counter-clockwise.
        /// </summary>
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
                me.UpdateSpin();
            }
        }

        /// <summary>
        /// Whether the indicator is spinning.  The default style binds this to
        /// <see cref="GlobalControl.IsActive"/> and shows the indicator only while it is true.
        /// </summary>
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

        /// <summary>
        /// Sizes and positions the ring and the two arcs for the current size of the control.
        /// </summary>
        private void UpdateShapes()
        {
            double width = ActualWidth;
            double height = ActualHeight;
            if (width <= 0 || height <= 0)
            {
                // Not laid out yet (or no room at all): there is nothing to draw.
                MyContent.Visibility = Visibility.Collapsed;
                return;
            }
            MyContent.Visibility = Visibility.Visible;

            double workItem = Math.Min(width, height) * 0.75;
            double workWidth = width * 0.75;
            double workHeight = height * 0.75;

            //top-left corner placement.
            double adjustWidth = workWidth * 0.1;
            double adjustHeight = workHeight * 0.1;

            double strokeThickness = workItem * 0.2;

            double radiusWidth = (workWidth * 0.9) / 2;
            double radiusHeight = (workHeight * 0.9) / 2;

            Point center = new(radiusWidth + adjustWidth, radiusHeight + adjustHeight);

            //Background circle.
            Canvas.SetLeft(_ellipse, adjustWidth);
            Canvas.SetTop(_ellipse, adjustHeight);
            _ellipse.Width = workWidth * 1.1;
            _ellipse.Height = workHeight * 1.1;
            _ellipse.StrokeThickness = strokeThickness;

            // Both arcs are the lower half of the ring; each is rotated around the center of the ring.
            // The arc starts and ends on the ring, so both ends use the horizontal radius.
            StreamGeometry geometry = new();
            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(new Point(center.X + radiusWidth, center.Y), isFilled: false, isClosed: false);
                context.ArcTo(new Point(center.X - radiusWidth, center.Y), new Size(radiusWidth, radiusHeight),
                    rotationAngle: 0, isLargeArc: false, SweepDirection.Clockwise, isStroked: true, isSmoothJoin: false);
            }
            geometry.Freeze();

            UpdateArc(_arc, _rotateTransform, geometry, center, strokeThickness, adjustWidth, adjustHeight);
            UpdateArc(_arc2, _rotateTransform2, geometry, center, strokeThickness, adjustWidth, adjustHeight);
        }

        private static void UpdateArc(Path arc, RotateTransform rotation, Geometry geometry, Point center,
            double strokeThickness, double left, double top)
        {
            arc.Data = geometry;
            arc.StrokeThickness = strokeThickness;
            Canvas.SetLeft(arc, left);
            Canvas.SetTop(arc, top);

            // Moving the center keeps a running animation going; the spin does not restart when the control is resized.
            rotation.CenterX = center.X;
            rotation.CenterY = center.Y;
        }

        /// <summary>
        /// Runs the animation only while the indicator is active and can actually be seen.
        /// </summary>
        private void UpdateSpin()
        {
            bool shouldSpin = IsActive && IsVisible;
            if (shouldSpin == _isSpinning)
            {
                return;
            }
            _isSpinning = shouldSpin;

            // Passing null removes the animation, which also stops its clock.
            _rotateTransform.BeginAnimation(RotateTransform.AngleProperty, shouldSpin ? ClockwiseSpin : null);
            _rotateTransform2.BeginAnimation(RotateTransform.AngleProperty, shouldSpin ? CounterClockwiseSpin : null);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateSpin();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateShapes();
        }
    }
}
