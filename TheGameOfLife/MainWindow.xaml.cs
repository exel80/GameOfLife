using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TheGameOfLife
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private const int gameWidth = 50;
		private const int gameHeight = gameWidth;
		private const double spacing = 1.0;

		private readonly Brush ON = Brushes.Black;
		private readonly Brush OFF = Brushes.LightGray;

		private int roundCount = 0;

		public Rectangle[,] matrix = new Rectangle[gameHeight, gameWidth];
		private readonly DispatcherTimer timer = new DispatcherTimer();

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			// Change button name
			Button btn = sender as Button;
			btn.Content = btn.Content.ToString() == "Start!" ? "Stop!" : "Start!";

			// Game timer toggle
			if (!timer.IsEnabled)
				timer.Start();
			else
				timer.Stop();
		}

		private bool validCoordinate(int i) => i >= 0 && i < gameWidth;

		private void next()
		{
			roundCount++;

			int[,] neighbors = getNeighbors();

			for (int y = 0; y < gameHeight; y++)
			{
				Trace.WriteLine("Row: " + y);
				for (int x = 0; x < gameWidth; x++)
				{
					bool isAlive = matrix[y, x].Fill == ON;
					int countONPixels = neighbors[y, x];

					if (isAlive && countONPixels <= 1)
					{
						/*
						 * RULE : Each cell with one or no neighbors dies, as if by solitude.
						 */

						matrix[y, x].Fill = OFF;
						Trace.WriteLine(String.Format("Rule 1 -> {0}:{1} = {2}", y, x, isAlive));
					}
					else if (isAlive && (countONPixels >= 2 && countONPixels <= 3))
					{
						/*
						 * RULE : Each cell with two or three neighbors survives
						 */
						
						//_matrix[y, x].Fill = ON;
						Trace.WriteLine(String.Format("Rule 2 -> {0}:{1} = {2}", y, x, isAlive));
					}
					else if (isAlive && countONPixels >= 4)
					{
						/*
						 * RULE : Each cell with four or more neighbors dies, as if by overpopulation.
						 */

						matrix[y, x].Fill = OFF;
						Trace.WriteLine(String.Format("Rule 3 -> {0}:{1} = {2}", y, x, isAlive));
					}
					else if (!isAlive && countONPixels == 3)
					{
						/*
						 * RULE : Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
						 */

						matrix[y, x].Fill = ON;
						Trace.WriteLine(String.Format("Rule 4 -> {0}:{1} = {2}", y, x, isAlive));
					}
				}
			}

			round.Content = roundCount;
		}

		private int[,] getNeighbors()
		{
			int[,] _matrix = new int[gameHeight, gameWidth];

			for (int y = 0; y < gameHeight; y++)
			{
				for (int x = 0; x < gameWidth; x++)
				{
					int count = 0;
					//Trace.WriteLine(string.Format("PIXEL {0}:{1}", y, x));
					for (int i = 0; i < 8; i++)
					{
						int xx;
						int yy;
						switch (i)
						{
							default:
							// Top left
							case 0:
								xx = x - 1;
								yy = y - 1;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Top middle
							case 1:
								xx = x;
								yy = y - 1;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Top right
							case 2:
								xx = x + 1;
								yy = y - 1;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Middle left
							case 3:
								xx = x - 1;
								yy = y;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Middle right
							case 4:
								xx = x + 1;
								yy = y;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Bottom left
							case 5:
								xx = x - 1;
								yy = y + 1;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Bottom middle
							case 6:
								xx = x;
								yy = y + 1;

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
							// Bottom right
							case 7:
								xx = x + (validCoordinate(x + 1) ? 1 : 0);
								yy = y + (validCoordinate(y + 1) ? 1 : 0);

								if (!(!validCoordinate(xx) || !validCoordinate(yy)) && matrix[yy, xx].Fill == ON) count++;
								break;
						}
						_matrix[y, x] = count;
					}
				}
			}

			return _matrix;
		}

		private void buildCanvas()
		{

			for (int y = 0; y < gameHeight; y++)
			{
				for (int x = 0; x < gameWidth; x++)
				{
					Rectangle r = new Rectangle
					{
						Width = game.ActualWidth / gameWidth - spacing,
						Height = game.ActualHeight / gameHeight - spacing,
						Fill = OFF
					};
					game.Children.Add(r);

					Canvas.SetLeft(r, x * game.ActualWidth / gameWidth);
					Canvas.SetTop(r, y * game.ActualHeight / gameHeight);

					matrix[y, x] = r;

					r.MouseDown += R_MouseEnter;
					r.MouseEnter += R_MouseEnter;
				}
			}
		}

		private void R_MouseEnter(object sender, MouseEventArgs e)
		{
			coordinate.Content = Mouse.GetPosition(game);
			//coordinate.Content = (game.ActualHeight / gameHeight - spacing);

			// LeftButton pressed state check
			bool leftButton = e.LeftButton == MouseButtonState.Pressed;
			if (!leftButton) return;

			// Pixel definition
			Rectangle pixel = (Rectangle)sender;

			// Draw
			pixel.Fill = pixel.Fill == ON ? OFF : ON;
		}

		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			timer.Interval = TimeSpan.FromMilliseconds(1000 - Math.Ceiling(0.0 * 80));
			timer.Tick += gameTick;
		}

		private void game_Loaded(object sender, RoutedEventArgs e) => buildCanvas();

		private void gameTick(object sender, EventArgs e) => next();

		private void ButtonNext_Click(object sender, RoutedEventArgs e) => next();

		private void gameSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			double value = slider.Value;

			timer.Interval = TimeSpan.FromMilliseconds(1000 - Math.Ceiling(value * 100));
		}
	}
}
