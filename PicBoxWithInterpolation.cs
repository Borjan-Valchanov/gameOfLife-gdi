using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace gameOfLife {
	/// <summary>
	/// Inherits from PictureBox; adds Interpolation Mode Setting
	/// </summary>
	public class PicBoxWithInterpolation : PictureBox {
		public InterpolationMode InterpolationMode { get; set; }

		protected override void OnPaint(PaintEventArgs paintEventArgs) {
			paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
			if (paintEventArgs.Graphics.InterpolationMode == InterpolationMode.NearestNeighbor) paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			base.OnPaint(paintEventArgs);
		}
	}
}