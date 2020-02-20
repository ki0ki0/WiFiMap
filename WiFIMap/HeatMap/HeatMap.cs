using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WiFIMap.HeatMap
{
	public class HeatMap : FrameworkElement
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public HeatMap()
		{
			// init visuals
			_heatMapVisuals = new VisualCollection(this);

			// init brushes
			_brushes = new RadialGradientBrush[GradientNumber];

			for (int i = 0; i < _brushes.Length; i++)
			{
				_brushes[i] = new RadialGradientBrush(Color.FromArgb((byte)i, 255, 255, 255), Color.FromArgb(0, 255, 255, 255));
			}
		}

		public static ObservableCollection<HeatPoint> GetPointsSource(UIElement element)
		{
			return (ObservableCollection<HeatPoint>)element.GetValue(PointsSourceProperty);
		}

		public static void SetPointsSource(UIElement element, ObservableCollection<HeatPoint> value)
		{
			element.SetValue(PointsSourceProperty, value);
		}

		public static readonly DependencyProperty PointsSourceProperty = DependencyProperty.RegisterAttached(
			"PointsSource",
			typeof(ObservableCollection<HeatPoint>),
			typeof(HeatMap),
			new FrameworkPropertyMetadata(OnPointsSourceChanged)
		);

		private static void OnPointsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var heatMap = (HeatMap)d;
			heatMap.Render();
		}

		private const int GradientNumber = 256; // Number of color gradients

		private readonly VisualCollection _heatMapVisuals;

		private readonly RadialGradientBrush[] _brushes; // monochrome brushes used for heat map drawing


		protected override int VisualChildrenCount => _heatMapVisuals.Count;

		protected override Visual GetVisualChild(int index)
		{
			if (index < 0 || index >= _heatMapVisuals.Count)
				throw new ArgumentOutOfRangeException("index");

			return _heatMapVisuals[index];
		}


		/// <summary>
		/// Renders heat map content into the given canvas. The rendering is done using monochrome color (white) and need colorizing effect to have custom palette.
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
				{
					foreach (HeatPoint point in heatPoints)
					{
						var widthHeight = (double)point.Intensity / 5;

						dc.DrawRectangle(_brushes[point.Intensity], null, new Rect(point.X - widthHeight / 2, point.Y - widthHeight / 2, widthHeight, widthHeight));
					}
				}

				_heatMapVisuals.Add(drawingVisual);
			}
		}
	}
}
