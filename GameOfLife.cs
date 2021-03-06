using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace gameOfLife {
	class GameOfLife : Form {
		// Set the width of the client area here
		private readonly int clientWidth = 800;

		// Set the height of the client area here
		private readonly int clientHeight = 600;

		// Set the size of the cell rectangles here; this will affect the amount of cells
		private readonly int cellSize = 10;

		// Set the probability that a cell will be living when the random grid is first generated
		private readonly double cellLivesProb = 0.4;

		// Create picture box to hold the rendered image
		private readonly PicBoxWithInterpolation renderedGrid = new();

		// Create a bitmap where the grid is drawn to
		private readonly Bitmap drawnGrid;

		// Initialise graphics object for drawing the grid
		private readonly Graphics graphics;

		// Create 2D array representing the grid of the game
		private bool[,] grid;

		// Constructor
		public GameOfLife() {
			// Create the grid with specified values
			grid = new bool[clientWidth / cellSize, clientHeight / cellSize];

			// Set the size
			ClientSize = new Size(clientWidth, clientHeight);

			// Adjust bitmap size
			drawnGrid = new Bitmap(grid.GetLength(0), grid.GetLength(1));

			// Link graphics to bitmap
			graphics = Graphics.FromImage(drawnGrid);

			// Make the picture box usable
			renderedGrid.SizeMode = PictureBoxSizeMode.Zoom;
			renderedGrid.InterpolationMode = InterpolationMode.NearestNeighbor;
			renderedGrid.Location = new Point(0, 0);
			renderedGrid.Size = ClientSize;
			renderedGrid.BackColor = Color.Black;
			Controls.Add(renderedGrid);

			// Set the background color to black, looks nicer
			BackColor = Color.Black;

			// Call method to randomise grid's content
			RandomiseGrid();

			// Link resize of the picture box to form resize
			Resize += (sender, eventArgs) => renderedGrid.Size = ClientSize;

			// Prepare game loop to run when the form is displayed
			Shown += (sender, eventArgs) => GameLoop();
		}

		// Method to randomise grid's content
		private void RandomiseGrid() {
			// Create an object to generate random numbers
			Random random = new();

			// Loop through all rows
			for (int i = 0; i < grid.GetLength(0); i++) {

				// Loop through all cells in that row
				for (int j = 0; j < grid.GetLength(1); j++) {

					// Set to either true or false randomly
					grid[i, j] = Convert.ToBoolean(Math.Round(random.NextDouble() + cellLivesProb - 0.5));
				}
			}
		}

		// Game loop method
		private void GameLoop() {
			// Create a timer that runs draw and game update every x ms
			System.Timers.Timer timer = new(100);
			timer.Elapsed += (sender, eventArgs) => {
				try {
					Draw();
					GameUpdate();
				} catch { }
			};
			timer.Start();
		}

		// Drawing method
		private void Draw() {
			// CLear
			graphics.Clear(Color.Transparent);

			// Loop through all rows
			for (int i = 0; i < grid.GetLength(0); i++) {

				// Loop through all cells in that row
				for (int j = 0; j < grid.GetLength(1); j++) {
					
					// Draw a white square of set size with the coordinates being the position of the cell in the grid times the set size if that cell holds true as value
					if(grid[i, j]) {
						graphics.FillRectangle(Brushes.White, i, j, 1, 1);
					}
				}
			}

			// Apply the image
			renderedGrid.Image = (Bitmap)drawnGrid.Clone();
		}

		// Game update method
		private void GameUpdate() {
			// Initialise a new grid which the new values will temporarily be written to
			bool[,] newGrid = new bool[grid.GetLength(0), grid.GetLength(1)];

			// Loop through all rows
			for (int i = 0; i < grid.GetLength(0); i++) {

				// Loop through all cells in that row
				for (int j = 0; j < grid.GetLength(1); j++) {

					// Count the alive neighbours
					int livingNeighbours = 0;

					// Iterate through the three rows which neighbours could be in
					for (int k = -1; k <= 1; k++) {

						// Iterate through the three cells in that row
						for (int l = -1; l <= 1; l++) {

							// Only continue with the logic if it's not the cell itself
							if (!((k == 0) && (l == 0))) {

								// Introduce variables for the neighbours coordinates, this is needed to wrap the game around it's edges
								int nRow;
								if (i == 0 && k < 0) {
									nRow = grid.GetLength(0) + k;
								} else if (i == grid.GetLength(0) - 1 && k > 0) {
									nRow = k - 1; 
								} else {
									nRow = i + k;
								}
								int nCol;
								if (j == 0 && l < 0) {
									nCol = grid.GetLength(1) + l;
								} else if (j == grid.GetLength(1) - 1 && l > 0) {
									nCol = l - 1;
								} else {
									nCol = j + l;
								}

								// Now look if the neighbour is alive
								if (grid[nRow, nCol]) livingNeighbours++;
							}
						}
					}

					// Now here's the actual game logic!

					if (!grid[i, j] && livingNeighbours == 3) {

						// Cell is born
						newGrid[i, j] = true;
					} else if (grid[i, j] && (livingNeighbours == 2 || livingNeighbours == 3)) {

						// Cell survives
						newGrid[i, j] = true;
					}

					// I don't need to add any other scenarios, because in all others, the cell would die/stay dead.
				}
			}

			// Finally, overwrite the old grid with the new one
			grid = newGrid;
		}
	}
}