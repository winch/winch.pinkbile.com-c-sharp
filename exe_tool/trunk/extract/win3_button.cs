//
// Windows 3.11 style button
//
// see readme.txt for copyright and license info

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Win3
{
	public class Win3_Button: Button
	{
		bool space = false;
		public Win3_Button()
		{
			//
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaint (e);
			Graphics grfx = e.Graphics;
			Font font = new Font(Font,FontStyle.Bold);
			StringFormat strfmt = new StringFormat();
			strfmt.HotkeyPrefix = HotkeyPrefix.Show;
			strfmt.Alignment = StringAlignment.Center;
			strfmt.LineAlignment = StringAlignment.Center;
			Brush border = new SolidBrush(Color.Black);
			Brush light = new SolidBrush(SystemColors.ControlLightLight);
			Brush dark = new SolidBrush(SystemColors.ControlDark);
			Pen single = new Pen(border,1);

			grfx.FillRectangle(new SolidBrush(this.Parent.BackColor),ClientRectangle);

			//border
			grfx.DrawLine(single,1,0,this.Width-2,0);
			grfx.DrawLine(single,1,this.Height-1,this.Width-2,this.Height-1);
			grfx.DrawLine(single,0,1,0,this.Height-2);
			grfx.DrawLine(single,this.Width-1,1,this.Width-1,this.Height-2);

			grfx.FillRectangle(new SolidBrush(SystemColors.Control),3,3,this.Width-5,this.Height-5);

			if (this.Capture == true && (this.Parent.GetChildAtPoint(this.Parent.PointToClient(MousePosition)) == this || space == true))
			{
				//button down and mouse over button
				grfx.DrawRectangle(single,1,1,this.Width-3,this.Height-3);
				single = new Pen(dark,1);
				grfx.DrawLine(single,2,2,2,this.Height-3);
				grfx.DrawLine(single,2,2,this.Width-3,2);
				grfx.DrawString(this.Text,font,new SolidBrush(SystemColors.ControlText),new Rectangle(1,1,this.Width-1,this.Height-1),strfmt);
				//draw doted box around text
				float x,y;
				x = grfx.MeasureString(this.Text,font).Width - 5;
				y = grfx.MeasureString(this.Text,font).Height;
				single = new Pen(border,1);
				single.DashStyle = DashStyle.Dot;
				grfx.DrawRectangle(single,(this.Width / 2) - (x/2)+1,(this.Height /2) - (y/2)+1, x+2, y-1);
			}
			else
			{
				//button up
				single = new Pen(light,1);
				grfx.DrawLine(single,1,1,this.Width-3,1);
				grfx.DrawLine(single,1,2,this.Width-4,2);
				grfx.DrawLine(single,1,1,1,this.Height-3);
				grfx.DrawLine(single,2,1,2,this.Height-4);
				single = new Pen(dark,1);
				grfx.DrawLine(single,2,this.Height-3,this.Width-2,this.Height-3);
				grfx.DrawLine(single,1,this.Height-2,this.Width-2,this.Height-2);
				grfx.DrawLine(single,this.Width-3,2,this.Width-3,this.Height-2);
				grfx.DrawLine(single,this.Width-2,1,this.Width-2,this.Height-2);
				if (this.Enabled == true)
					grfx.DrawString(this.Text,font,new SolidBrush(SystemColors.ControlText),new Rectangle(0,0,this.Width-1,this.Height-1),strfmt);
				else
					grfx.DrawString(this.Text,font,new SolidBrush(SystemColors.ControlDark),new Rectangle(0,0,this.Width-1,this.Height-1),strfmt);
				if (this.Focused == true)
				{
					//draw doted box around text
					float x,y;
					x = grfx.MeasureString(this.Text,font).Width - 5;
					y = grfx.MeasureString(this.Text,font).Height;
					single = new Pen(border,1);
					single.DashStyle = DashStyle.Dot;
					grfx.DrawRectangle(single,(this.Width / 2) - (x/2),(this.Height /2) - (y/2), x+2, y-1);
				}
			}
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				space = true;
				Invalidate();
			}
			base.OnKeyDown (e);
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				space = false;
				Invalidate();
			}
			base.OnKeyUp (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			Invalidate();
		}

	}
}