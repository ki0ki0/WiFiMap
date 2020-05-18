using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WiFiMapCore.HeatMap
{
    public class HeatMap : FrameworkElement
    {
        private const int GradientNumber = 256; // Number of color gradients

        public static readonly DependencyProperty PointsSourceProperty = DependencyProperty.RegisterAttached(
            "PointsSource",
            typeof(ObservableCollection<IHeatPoint>),
            typeof(HeatMap),
            new FrameworkPropertyMetadata(OnPointsSourceChanged)
        );

        private readonly RadialGradientBrush[] _brushes; // monochrome brushes used for heat map drawing

        private readonly VisualCollection _heatMapVisuals;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public HeatMap()
        {
            // init visuals
            _heatMapVisuals = new VisualCollection(this);

            // init brushes
            _brushes = new RadialGradientBrush[GradientNumber];

            for (var i = 0; i < _brushes.Length; i++)
                _brushes[i] = new RadialGradientBrush(Color.FromArgb((byte) i, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255));
        }


        protected override int VisualChildrenCount => _heatMapVisuals.Count;

        public static ObservableCollection<IHeatPoint> GetPointsSource(UIElement element)
        {
            return (ObservableCollection<IHeatPoint>) element.GetValue(PointsSourceProperty);
        }

        public static void SetPointsSource(UIElement element, ObservableCollection<IHeatPoint> value)
        {
            element.SetValue(PointsSourceProperty, value);
        }

        private static void OnPointsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var heatMap = (HeatMap) d;
            heatMap.Render();
        }
        
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _heatMapVisuals.Count)
                throw new ArgumentOutOfRangeException("index");

            return _heatMapVisuals[index];
        }


        /// <summary>
        ///     Renders heat map content into the given canvas. The rendering is done using monochrome color (white) and need
        ///     colorizing effect to have custom palette.
        /// </summary>
        public void Render()
        {
            // clear visuals
            _heatMapVisuals.Clear();

            // draw heatmap points
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                var heatPoints = GetPointsSource(this)?.ToList();
                if (heatPoints != null)
                    foreach (var point in heatPoints)
                    {
                        var widthHeight = (double) point.Intensity;

                        dc.DrawRectangle(_brushes[point.Intensity], null,
                            new Rect(point.X - widthHeight / 2, point.Y - widthHeight / 2, widthHeight, widthHeight));
                    }

                _heatMapVisuals.Add(drawingVisual);
            }
        }
    }
}