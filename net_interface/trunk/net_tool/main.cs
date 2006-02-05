/*
net_tool
Copyright (C) 2005 the_winch

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
//
// main source file
//

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

class main: Form
{
	public string dllTool = ""; //location of dll tool.exe
	public string dbpExe = ""; //location of dbpro exe
	public string netDll = ""; //location of .net dll to look for

	NumericUpDown interval;
	Button toggleTimer;
	Timer timer;

	public TextBox dispWidth;
	public TextBox dispHeight;
	public TextBox dispDepth;
	public ComboBox dispMode;
	public ListBox log;

	int ticks;

	public static void Main()
	{
		Application.Run(new main());
	}

	public main()
	{
		ArrayList al = new ArrayList();
		al.Add(1);

		Text = "net_tool";
		Size = new Size(500, 200);

		int y = 5;

		//dll tool location button
		Button btnDll = new Button();
		btnDll.Parent = this;
		btnDll.Text = "dll_tool location";
		btnDll.Location = new Point(5, y);
		btnDll.Width += 40;
		btnDll.Click += new EventHandler(btnDll_Click);
		y += btnDll.Height + 5;

		//dbpro project location
		Button btnDbpro = new Button();
		btnDbpro.Parent = this;
		btnDbpro.Text = "Dbpro exe location";
		btnDbpro.Location = new Point(5, y);
		btnDbpro.Width += 40;
		btnDbpro.Click += new EventHandler(btnDbpro_Click);
		y += btnDbpro.Height + 5;

		//net dll location
		Button btnNet = new Button();
		btnNet.Parent = this;
		btnNet.Text = ".Net dll location";
		btnNet.Location = new Point(5, y);
		btnNet.Width += 40;
		btnNet.Click += new EventHandler(btnNet_Click);
		y += btnNet.Height + 5;

		//timer groupbox
		GroupBox gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Timer";
		gb.Location = new Point(5, y);
		gb.Size = new Size(120,80);
		y += gb.Height;

		//interval label
		Label lab = new Label();
		lab.Parent = gb;
		lab.AutoSize = true;
		lab.Text = "Interval";
		lab.Location = new Point(5,15);

		//interval updown
		interval = new NumericUpDown();
		interval.Parent = gb;
		interval.Increment = 250;
		interval.Maximum = 10000;
		interval.Location = new Point(lab.Left + lab.Width + 5, 15);
		interval.Width = 60;
		interval.ValueChanged += new EventHandler(interval_ValueChanged);

		//timer start/stop button
		toggleTimer = new Button();
		toggleTimer.Parent = gb;
		toggleTimer.Text = "Go";
		toggleTimer.Location = new Point(10, 45);
		toggleTimer.Width += 20;
		toggleTimer.Click += new EventHandler(toggleTimer_Click);

		y = 5;
		//display mode groupbox
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Display mode";
		gb.Location = new Point(135, y);
		gb.Size = new Size(100, 135);
		y+= gb.Height + 5;

		//about button
		Button btnAbout = new Button();
		btnAbout.Parent = this;
		btnAbout.Text = "About";
		btnAbout.Location = new Point(135, y);
		btnAbout.Width += 25;
		btnAbout.Click += new EventHandler(btnAbout_Click);

		y = 15;

		//width label
		lab = new Label();
		lab.Parent = gb;
		lab.AutoSize = true;
		lab.Text = "Width";
		lab.Location = new Point(5, y);
		//width textbox
		dispWidth = new TextBox();
		dispWidth.Parent = gb;
		dispWidth.Location = new Point(lab.Left + lab.Width + 8, y);
		dispWidth.Width = 40;
		y += dispWidth.Height + 10;

		//height label
		lab = new Label();
		lab.Parent = gb;
		lab.AutoSize = true;
		lab.Text = "Height";
		lab.Location = new Point(5, y);
		//height textbox
		dispHeight = new TextBox();
		dispHeight.Parent = gb;
		dispHeight.Location = new Point(lab.Left + lab.Width + 5, y);
		dispHeight.Width = 40;
		y += dispHeight.Height + 10;

		//depth label
		lab = new Label();
		lab.Parent = gb;
		lab.AutoSize = true;
		lab.Text = "Depth";
		lab.Location = new Point(5, y);
		//height textbox
		dispDepth = new TextBox();
		dispDepth.Parent = gb;
		dispDepth.Location = new Point(lab.Left + lab.Width + 8, y);
		dispDepth.Width = 40;
		y += dispDepth.Height + 10;

		//display mode
		dispMode = new ComboBox();
		dispMode.Parent = gb;
		dispMode.Location = new Point(10, y);
		dispMode.Width = 80;
		dispMode.DropDownStyle = ComboBoxStyle.DropDownList;
		dispMode.Items.Add("Hidden");
		dispMode.Items.Add("Windowed");
		dispMode.Items.Add("Desktop");
		dispMode.Items.Add("Full ex");
		dispMode.Items.Add("Fullscreen");

		//log listbox
		log = new ListBox();
		log.Parent = this;
		log.Location = new Point(245, 5);
		log.Size = new Size(240, 170);

		//log popup menu
		EventHandler mClear  = new EventHandler(mClearOnClick);
		MenuItem[] ami =  { new MenuItem("Clear log", mClear) };
		log.ContextMenu = new ContextMenu(ami);

		//set up timer
		timer = new Timer();
		timer.Tick += new EventHandler(timer_Tick);

		this.Load += new EventHandler(main_Load);
		this.Closed += new EventHandler(main_Closed);
	}

	private void main_Load(object sender, EventArgs e)
	{
		//load config
		if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "config.dat"))
		{
			FileStream fs = null;
			BinaryReader br = null;
			try
			{
				fs = new FileStream(Application.StartupPath + Path.DirectorySeparatorChar + "config.dat", FileMode.Open);
				br = new BinaryReader(fs);
				dllTool = br.ReadString();
				dbpExe = br.ReadString();
				interval.Value = br.ReadDecimal();
				timer.Interval = (int)interval.Value;
				dispWidth.Text = br.ReadString();
				dispHeight.Text = br.ReadString();
				dispDepth.Text = br.ReadString();
				dispMode.SelectedIndex = br.ReadInt32();
				log.Items.Add("Loaded Settings");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (br!= null)
					br.Close();
				if (fs!=null)
					fs.Close();
			}
		}
		else
		{
			//default settings
			interval.Value = 250;
			timer.Interval = 250;
			dispWidth.Text = "640";
			dispHeight.Text = "480";
			dispDepth.Text = "32";
			dispMode.SelectedIndex = 1;
			log.Items.Add("No config, using defaults");
		}
	}

	private void main_Closed(object sender, EventArgs e)
	{
		//save config
		FileStream fs = null;
		BinaryWriter bw = null;
		try
		{
			fs = new FileStream(Application.StartupPath + Path.DirectorySeparatorChar + "config.dat", FileMode.Create);
			bw = new BinaryWriter(fs);
			bw.Write(dllTool);
			bw.Write(dbpExe);
			bw.Write(interval.Value);
			bw.Write(dispWidth.Text);
			bw.Write(dispHeight.Text);
			bw.Write(dispDepth.Text);
			bw.Write(dispMode.SelectedIndex);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (bw != null)
				bw.Close();
			if (fs!=null)
				fs.Close();
		}
	}

	private void btnDll_Click(object sender, EventArgs e)
	{
		//set dll_tool.exe location
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Locate dll_tool.exe";
		ofd.Filter = "dll_tool.exe|dll_tool.exe|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			dllTool = ofd.FileName;
		}
		ofd.Dispose();
	}

	private void btnDbpro_Click(object sender, EventArgs e)
	{
		//dbpro.exe location
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Locate DbPro exe";
		ofd.Filter = "Dbpro exe (*.exe)|*.exe|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			dbpExe = ofd.FileName;
		}
		ofd.Dispose();
	}

	private void btnNet_Click(object sender, EventArgs e)
	{
		//.net location
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Locate .net dll";
		ofd.Filter = ".Net dlls (*.dll)|*.dll|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			netDll = ofd.FileName;
		}
	}

	private void toggleTimer_Click(object sender, EventArgs e)
	{
		//toggle timer
		if (toggleTimer.Text == "Go")
		{
			//start timer
			toggleTimer.Text = "Stop";
			timer.Start();
			ticks = 0;
			log.Items.Add("Started timer, interval "+ timer.Interval.ToString());
			//delete the net dll so we can detect when it is created
			if (netDll != "")
			{
				File.Delete(netDll);
				log.Items.Add(".Net dll deleted");
			}
		}
		else
		{
			//stop timer
			toggleTimer.Text = "Go";
			timer.Stop();
			log.Items.Add("Timer stopped, " + ticks.ToString() + " ticks.");
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		//timer event
		ticks ++;
		if (File.Exists(netDll))
		{
			FileStream fsIn = null;
			BinaryReader brIn = null;
			FileStream fsOut = null;
			BinaryWriter bwOut = null;
			try
			{
				//dll is there so do stuff
				timer.Stop();
				log.Items.Add(".Net dll found, starting...");
				//dll_tool
				log.Items.Add("Running dll_tool");
				Process process = new Process();
				process.StartInfo.FileName = dllTool;
				//MessageBox.Show("-a \"" + netDll + "\"");
				process.StartInfo.Arguments = "-a \"" + netDll + "\"";
				process.Start();
				process.WaitForExit();
				if (File.Exists(netDll + ".new"))
					log.Items.Add("Dll_tool finished");
				else
				{
					log.Items.Add("Dll_tool failed");
					return;
				}
				//dbpro exe
				fsIn = new FileStream(dbpExe, FileMode.Open);
				brIn = new BinaryReader(fsIn);
				fsOut = new FileStream(Path.GetFullPath(netDll) + Path.DirectorySeparatorChar + "game.exe", FileMode.Create);
				bwOut = new BinaryWriter(fsOut);
				log.Items.Add("Started writing new exe");
				ProExe.DoExe(bwOut, fsIn, brIn, this);
				log.Items.Add("Finished writing new exe");
				bwOut.Close();
				fsOut.Close();
				File.Delete(netDll + ".new");
				File.Delete(netDll);
				log.Items.Add(".Net dll deleted");
				log.Items.Add("running new exe");
				process = new Process();
				process.StartInfo.FileName = Path.GetTempPath() + "_temp.exe";
				process.Start();
				process.WaitForExit();
				log.Items.Add("new exe exit");
				log.Items.Add("====================");
				timer.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "net_tool Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (brIn != null)
					brIn.Close();
				if (fsIn != null)
					fsIn.Close();
				if (bwOut != null)
					bwOut.Close();
				if (fsOut != null)
					fsOut.Close();
			}
		}
	}

	private void interval_ValueChanged(object sender, EventArgs e)
	{
		//update timer interval
		try
		{
			timer.Interval = (int)interval.Value;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}
	private void mClearOnClick(object sender, EventArgs e)
	{
		//clear log
		log.Items.Clear();
	}

	private void btnAbout_Click(object sender, EventArgs e)
	{
		//show about dialog
		aboutDialog ab = new aboutDialog();
		ab.ShowDialog();
	}
}
			