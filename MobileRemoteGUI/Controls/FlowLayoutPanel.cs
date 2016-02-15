using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
	public enum LayoutStyle{ GridLayout, FlowLayout}
	public class FlowLayoutPanel : Panel
	{
		#region Private Members

		private IContainer components = null;
		private LayoutStyle myLayoutStyle;

		#endregion

		#region Constructors

		public FlowLayoutPanel()
		{
			InitializeComponent();
		}

		#endregion

		#region Public Properties
		
		public LayoutStyle LayoutStyle
		{
			get
			{
				return myLayoutStyle;
			}
			set
			{
				myLayoutStyle = value;
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnResize(EventArgs e)
		{
			if (this.LayoutStyle == LayoutStyle.FlowLayout)
			{
				int nextTop = 0, nextLeft = 0;
				int maxHeight = 0, maxWidth = 0;
				int ParentWidth;
				if (this.Parent != null)
				{
					ParentWidth = this.Parent.Width;
				}
				else
				{
					ParentWidth = this.Width;
				}


                foreach (Control myControl in this.Controls)
                {
                    maxWidth = Math.Max(maxWidth, myControl.Width);
                    maxHeight = Math.Max(maxHeight, myControl.Height);
                }

				foreach(Control myControl in this.Controls)
				{
                    if ((nextLeft + myControl.Width) > this.Width)
					{
						nextTop += maxHeight;
						nextLeft = 0;
					}
                    myControl.Top = nextTop;
                    myControl.Left = nextLeft;

                    nextLeft += myControl.Width;
				}
				this.AutoScrollPosition = new System.Drawing.Point(0,0);
			}
			base.OnResize(e);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#endregion
	}
}

