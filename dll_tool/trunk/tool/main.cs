/*
dll_tool
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

References
http://www.csharphelp.com/archives3/archive500.html
Inside microsoft .Net il assembler by Serge Lidin
.res file format info from http://wotsit.org
*/

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

class main: Form
{
	public string[] args;

	public string TEMP = Path.GetTempPath();

	public tools Tools;
	public ilTool ilAsm = null;
	public ilTool ilDasm = null;

	public string dllName;

	public ListBox lb;
	public ListView methodBox;
	public ListView exportBox;
	public ListBox stringBox;
	public CheckBox stringCheck;
	public CheckBox editIl;
	public Button build;
	public ComboBox exportsType;
	ComboBox ilAsmVersion;
	ComboBox ilDasmVersion;

	public static void Main(string[] arg) 
	{
		Application.Run(new main(arg));
	}

	public main(string[] arg)
	{
		args = arg;

		Text = "dll_tool";
		Size = new Size(600,580);
		this.Load += new EventHandler(main_Load);
		this.Disposed += new EventHandler(main_Disposed);

		//ilasm
		Tools = tools.GetInstance();
		//make sure ilasm and ildasm are installed.
		if (Tools.IlAsm.Count == 0)
		{
			MessageBox.Show("ilasm.exe not found.\nInstall .Net SDK.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		if (Tools.IlDasm.Count == 0)
		{
			MessageBox.Show("ildasm.exe not found.\nInstall .Net SDK.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		if (Tools.IlAsm.Count > 0)
            ilAsm = (ilTool) Tools.IlAsm[Tools.IlAsm.Count - 1];
		if (Tools.IlDasm.Count > 0)
			ilDasm = (ilTool) Tools.IlDasm[Tools.IlDasm.Count - 1];

		//don't select "Custom" version if other versions exist
		if (Tools.IlAsm.Count > 1)
			ilAsm = (ilTool) Tools.IlAsm[Tools.IlAsm.Count - 2];
		if (Tools.IlDasm.Count > 1)
			ilDasm = (ilTool) Tools.IlDasm[Tools.IlDasm.Count - 2];

		//load button
		Button load = new Button();
		load.Parent = this;
		load.Text = "&Load";
		load.Width -= 10;
		load.Location = new Point(10,10);
		load.Click += new EventHandler(load_Click);

		//build button
		build = new Button();
		build.Parent = this;
		build.Text = "&Build";
		build.Location = new Point(85,10);
		build.Width -= 10;
		build.Enabled = false;
		build.Click += new EventHandler(build_Click);

		//load settings button
		Button loadDsf = new Button();
		loadDsf.Parent = this;
		loadDsf.Text = "Load .dsf";
		loadDsf.Width -= 10;
		loadDsf.Location = new Point(160, 10);
		loadDsf.Click += new EventHandler(loadDsf_Click);

		//save settings button
		Button saveDsf = new Button();
		saveDsf.Parent = this;
		saveDsf.Text = "Save .dsf";
		saveDsf.Width -= 10;
		saveDsf.Location = new Point(235, 10);
		saveDsf.Click += new EventHandler(saveDsf_Click);

		//about button
		Button about = new Button();
		about.Parent = this;
		about.Text = "About";
		about.Width -= 10;
		about.Location = new Point(310, 10);
		about.Click += new EventHandler(about_Click);

		//exit button
		Button exit = new Button();
		exit.Parent = this;
		exit.Text = "E&xit";
		exit.Width -= 10;
		exit.Location = new Point(385, 10);
		exit.Click += new EventHandler(exit_Click);

		//edit il before build checkbox
		editIl = new CheckBox();
		editIl.Parent = this;
		editIl.Text = "&Edit il before build";
		editIl.Width = 122;
		editIl.Location = new Point(this.Width - editIl.Width - 10, 5);
		editIl.Anchor = AnchorStyles.Top | AnchorStyles.Right;

		//ilasm label
		Label lab = new Label();
		lab.Parent = this;
		lab.AutoSize = true;
		lab.Location = new Point(5, 45);
		lab.Text = "ilAsm";

		//ilasm combobox
		ilAsmVersion = new ComboBox();
		ilAsmVersion.Parent = this;
		ilAsmVersion.DropDownStyle = ComboBoxStyle.DropDownList;
		ilAsmVersion.Width = 80;
		ilAsmVersion.Location = new Point(lab.Width + lab.Left, 40);
		foreach(ilTool iltool in Tools.IlAsm)
			ilAsmVersion.Items.Add(iltool.Version);
		ilAsmVersion.SelectedIndex = ilAsmVersion.Items.Count - 1;
		//don't select custom
		if (ilAsmVersion.Items.Count > 1)
			ilAsmVersion.SelectedIndex = ilAsmVersion.Items.Count - 2;
		ilAsmVersion.SelectedIndexChanged += new EventHandler(ilAsmVersion_SelectedIndexChanged);

		//ildasm label
		lab = new Label();
		lab.Parent = this;
		lab.AutoSize = true;
		lab.Location = new Point(ilAsmVersion.Left + ilAsmVersion.Width + 5, 45);
		lab.Text = "ilDasm";

		//ildasm combobox
		ilDasmVersion = new ComboBox();
		ilDasmVersion.Parent = this;
		ilDasmVersion.DropDownStyle = ComboBoxStyle.DropDownList;
		ilDasmVersion.Width = 80;
		ilDasmVersion.Location = new Point(lab.Left + lab.Width, 40);
		foreach(ilTool iltool in Tools.IlDasm)
			ilDasmVersion.Items.Add(iltool.Version);
		ilDasmVersion.SelectedIndex = ilDasmVersion.Items.Count - 1;
		//don't select "custom";
		if (ilDasmVersion.Items.Count > 1)
			ilDasmVersion.SelectedIndex = ilDasmVersion.Items.Count - 2;
		ilDasmVersion.SelectedIndexChanged += new EventHandler(ilDasmVersion_SelectedIndexChanged);

		//exports type combobox
		exportsType = new ComboBox();
		exportsType.Parent = this;
		exportsType.DropDownStyle = ComboBoxStyle.DropDownList;
		exportsType.Width = 80;
		exportsType.Location = new Point(this.Width - exportsType.Width - 20, 40);
		exportsType.Items.Add("Cdecl");
		exportsType.Items.Add("StdCall");
		exportsType.SelectedIndex = 0;
		exportsType.Anchor = AnchorStyles.Top | AnchorStyles.Right;

		//exports type label
		lab = new Label();
		lab.Parent = this;
		lab.AutoSize = true;
		lab.Location = new Point(exportsType.Left - lab.Width - 120, 45);
		lab.Text = "Moved methods will be";
		lab.Anchor = AnchorStyles.Top | AnchorStyles.Right;

		//methods groupbox
		GroupBox gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Methods";
		gb.Location = new Point(10, 70);
		gb.Size = new Size(230, 150);

		//method listview
		methodBox = new ListView();
		methodBox.Parent = gb;
		methodBox.Location = new Point(10, 15);
		methodBox.Size = new Size(210,130);
		methodBox.Columns.Add("method",200,HorizontalAlignment.Left);
		methodBox.Columns.Add("full",5,HorizontalAlignment.Left);
		methodBox.View = View.Details;

		//exports groupbox
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Exports";
		gb.Location = new Point(290, 70);
		gb.Size = new Size(290, 150);
		gb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

		//exports listview
		exportBox = new ListView();
		exportBox.Parent = gb;
		exportBox.Location = new Point(10, 15);
		exportBox.Size = new Size(270, 130);
		exportBox.Columns.Add("method", 95, HorizontalAlignment.Left);
		exportBox.Columns.Add("calling conv", 70, HorizontalAlignment.Left);
		exportBox.Columns.Add("export", 95, HorizontalAlignment.Left);
		exportBox.Columns.Add("full", 5, HorizontalAlignment.Left);
		exportBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		exportBox.View = View.Details;
		exportBox.ItemActivate += new EventHandler(exportBox_ItemActivate);

		//move left button
		Button leftBtn = new Button();
		leftBtn.Parent = this;
		leftBtn.Text = "&<";
		leftBtn.Location = new Point(250, 150);
		leftBtn.Size = new Size(30,70);
		leftBtn.Click += new EventHandler(leftBtn_Click);

		//move right button
		Button rightBtn = new Button();
		rightBtn.Parent = this;
		rightBtn.Text = "&>";
		rightBtn.Location = new Point(250, 75);
		rightBtn.Size = new Size(30, 70);
		rightBtn.Click += new EventHandler(rightBtn_Click);

		//string table groupbox
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "String Table";
		gb.Location = new Point(10,230);
		gb.Size = new Size(570,145);
		gb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

		//string table listbox
		stringBox = new ListBox();
		stringBox.Parent = gb;
		stringBox.Location = new Point(10,15);
		stringBox.Size = new Size(450, 125);
		stringBox.DoubleClick += new EventHandler(stringBox_DoubleClick);
		stringBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

		//add string table checkbox
		stringCheck = new CheckBox();
		stringCheck.Parent = gb;
		stringCheck.Text = "Add string table";
		stringCheck.Location = new Point(465, 15);
		stringCheck.Size = new Size(102, 20);
		stringCheck.Anchor = AnchorStyles.Top | AnchorStyles.Right;

		// generate string table button
		Button strBtn = new Button();
		strBtn.Parent = gb;
		strBtn.Text = "&Generate";
		strBtn.Width += 20;
		strBtn.Height -= 5;
		strBtn.Location = new Point(465, 40);
		strBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		strBtn.Click += new EventHandler(strBtn_Click);

		//add string table item button
		Button strAddBtn = new Button();
		strAddBtn.Parent = gb;
		strAddBtn.Text = "&Add";
		strAddBtn.Width += 20;
		strAddBtn.Height -= 5;
		strAddBtn.Location = new Point(465, 60);
		strAddBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		strAddBtn.Click += new EventHandler(strAddBtn_Click);

		//remove string table item button
		Button strRemBtn = new Button();
		strRemBtn.Parent = gb;
		strRemBtn.Text = "&Remove";
		strRemBtn.Width += 20;
		strRemBtn.Height -= 5;
		strRemBtn.Location = new Point(465, 80);
		strRemBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		strRemBtn.Click += new EventHandler(strRemBtn_Click);

		//generate help button
		Button docBuild = new Button();
		docBuild.Parent = gb;
		docBuild.Text = "Generate &help";
		docBuild.Width += 20;
		docBuild.Height -= 5;
		docBuild.Location = new Point(465,100);
		docBuild.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		docBuild.Visible = false;

		//generate ini button
		Button iniBuild = new Button();
		iniBuild.Parent = gb;
		iniBuild.Text = "Generate &ini";
		iniBuild.Width += 20;
		iniBuild.Height -= 5;
		iniBuild.Location = new Point(465,120);
		iniBuild.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		iniBuild.Visible = false;

		//il groubbox
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "IL code (double click to edit)";
		gb.Location = new Point(10, 385);
		gb.Size = new Size(570, 160);
		gb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		//il listbox
		lb = new ListBox();
		lb.Parent = gb;
		lb.Location = new Point(10,15);
		lb.Size = new Size(550,140);
		lb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		lb.DoubleClick += new EventHandler(lb_DoubleClick);
	}

	private void load_Click(object sender, EventArgs e)
	{
		//load dll
		//check for ildasm
		if (ilDasm == null)
		{
			MessageBox.Show("No ildasm.exe selected", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		//check for custom ildasm
		if (ilDasm.Path == null)
		{
			OpenFileDialog ilofd = new OpenFileDialog();
			ilofd.Filter = "Ildasm.exe|ildasm.exe|Exe files (*.exe)|*.exe|All files (*.*)|*.*";
			if (ilofd.ShowDialog() == DialogResult.OK)
			{
				ilDasm.Path = ilofd.FileName;
			}
			else
			{
				MessageBox.Show("No ildasm.exe selected", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "Dll files (*.dll)|*.dll|All files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			dll.Load(ofd.FileName, this);
		}
	}

	private void leftBtn_Click(object sender, EventArgs e)
	{
		//move method left
		if (exportBox.SelectedItems.Count > 0)
		{
			foreach (ListViewItem i in exportBox.SelectedItems)
			{
				ListViewItem method = new ListViewItem(i.SubItems[0].Text);
				method.SubItems.Add(i.SubItems[3].Text);
				methodBox.Items.Add(method);
				i.Remove();
			}
		}
	}

	private void rightBtn_Click(object sender, EventArgs e)
	{
		//move method(s) right
		if (methodBox.SelectedItems.Count > 0)
		{
			exportBox.BeginUpdate();
			methodBox.BeginUpdate();
			foreach (ListViewItem lvi in methodBox.SelectedItems)
			{
				ListViewItem export = new ListViewItem(lvi.Text);
				if (exportsType.SelectedIndex == 0)
                    export.SubItems.Add("Cdecl");
				else
					export.SubItems.Add("StdCall");
				export.SubItems.Add(lvi.Text);
				export.SubItems.Add(lvi.SubItems[1].Text);
				exportBox.Items.Add(export);
				lvi.Remove();
			}
			exportBox.EndUpdate();
			methodBox.EndUpdate();
		}
	}

	private void build_Click(object sender, EventArgs e)
	{
		//build dll
		if (ilAsm == null)
		{
			MessageBox.Show("No ilasm.exe selected", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		if (exportBox.Items.Count == 0)
		{
			MessageBox.Show("No methods to export!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		//custom ilasm
		if (ilAsm.Path == null)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Ilasm.exe|ilasm.exe|Exe files (*.exe)|*.exe|All files (*.*)|*.*";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				ilAsm.Path = ofd.FileName;
			}
			else
			{
				MessageBox.Show("No ilasm.exe selected", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Filter = "Dll files (*.dll)|*.dll";
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			dll.Save(sfd.FileName, this, false);
		}
	}

	private void about_Click(object sender, EventArgs e)
	{
		//about button
		aboutDialog ad = new aboutDialog();
		ad.ShowDialog();
		ad.Dispose();
	}

	private void main_Load(object sender, EventArgs e)
	{
		//hide form if command like args
		if (args.Length > 0)
		{
			this.Hide();
		}
		if (args.Length > 0)
		{
			dll.DoCommandLine(args, this);
		}
	}

	private void exportBox_ItemActivate(object sender, EventArgs e)
	{
		//edit export name when item double clicked in exportBox
		nameDialog ng = new nameDialog("Edit export", true);
		foreach (ListViewItem lvi in exportBox.SelectedItems)
		{
			ng.method = lvi.SubItems[0].Text;
			ng.CallingConvention = lvi.SubItems[1].Text;
			ng.name = lvi.SubItems[2].Text;
			if (ng.ShowDialog() == DialogResult.OK)
			{
				lvi.SubItems[1].Text = ng.CallingConvention;
				lvi.SubItems[2].Text = ng.name;
			}
		}
	}

	private void strBtn_Click(object sender, EventArgs e)
	{
		//use items in exportBox to generate stringtable
		if (exportBox.Items.Count == 0)
		{
			MessageBox.Show("No exported methods to generate string table with!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		stringBox.BeginUpdate();
		foreach (ListViewItem lvi in exportBox.Items)
		{
			//generate string table for each item
			//.method public hidebysig static void test1(float32 f,int32 i,string s,float64 d,unsigned int8 b) cil managed
			string types = "";
			int start,end;
			string func = lvi.SubItems[3].Text;
			//strip double spaces
			func = func.Replace("  "," ");
			//get return type if any
			end = func.IndexOf("(");
			end = func.LastIndexOf(" ",end,end-1);
			start = func.LastIndexOf(" ",end-1,end-2);
			string ret = func.Substring(start+1,end-start-1);
			switch (ret)
			{
				case "void":
					types = "%";
					break;
				case "string":
					types = "[%S";
					break;
				case "int32":
					types = "[%L";
					break;
				case "float32":
					types = "[%F";
					break;
				case "float64":
					types = "[%O";
					break;
				default:
					types = "%";
					break;
			}
			//get parameters if any
			start = func.IndexOf("(") + 1;
			end = func.IndexOf(")");
			ret = func.Substring(start,end-start);
			if (ret != "")
			{
				//float32 f,int32 i,string s,float64 d,unsigned int8 b
				//find the first space to get type, then skip to "," to find start of next
				start = 0;
				end = ret.Length;
				string type;
				while (start > -1 && ret.IndexOf(" ",start,end-start) != -1)
				{
					end = ret.IndexOf(" ",start,end-start);
					type = ret.Substring(start, end-start);
					start = ret.IndexOf(",",end,ret.Length-end)+1;
					if (start == 0)
						start = -1;
					end = ret.Length;
					switch (type)
					{
						case "string":
							types += "S";
							break;
						case "int32":
							types += "L";
							break;
						case "float32":
							types += "F";
							break;
						case "float64":
							types += "O";
							break;
					}
				}
			}
			else
				types += "0"; //no parameters
			stringBox.Items.Add(lvi.SubItems[2].Text.ToUpper().Replace("'","") + types + "%" + lvi.SubItems[2].Text.Replace("'","") + "%");
		}
		stringBox.EndUpdate();
	}

	private void strAddBtn_Click(object sender, EventArgs e)
	{
		//adds string table item
		nameDialog ng = new nameDialog("Add string table item", false);
		if (ng.ShowDialog() == DialogResult.OK)
		{
			stringBox.Items.Add(ng.name);
		}
	}

	private void strRemBtn_Click(object sender, EventArgs e)
	{
		//remove string table item
		if (stringBox.SelectedItems.Count > 0)
		{
			foreach (int i in stringBox.SelectedIndices)
			{
				stringBox.Items.RemoveAt(i);
			}
		}
	}

	private void stringBox_DoubleClick(object sender, EventArgs e)
	{
		//edit selected string table item
		if (stringBox.SelectedItems.Count > 0)
		{
			nameDialog ng = new nameDialog("Edit string table item", false);
			ng.name = (string) stringBox.Items[stringBox.SelectedIndices[0]];
			if (ng.ShowDialog() == DialogResult.OK)
			{
				stringBox.Items[stringBox.SelectedIndices[0]] = ng.name;
			}
		}
	}

	private void exit_Click(object sender, EventArgs e)
	{
		//exit program
		Application.Exit();
	}

	private void main_Disposed(object sender, EventArgs e)
	{
		//clean up _dll.il and _dll.res if required
		if (File.Exists(TEMP + "\\_dll.il"))
			File.Delete(TEMP + "\\_dll.il");
		if (File.Exists(TEMP + "\\_dll.res"))
			File.Delete(TEMP + "\\_dll.res");
	}

	private void lb_DoubleClick(object sender, EventArgs e)
	{
		//edit il
		editIlDialog.editIlasm(lb);
	}

	private void loadDsf_Click(object sender, EventArgs e)
	{
		//load .dsf file
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Load dll settings file file";
		ofd.Filter = "Dll settings file (*.dsf)|*.dsf|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			BinaryReader br = null;
			try
			{
				fs = new FileStream(ofd.FileName, FileMode.Open);
				br = new BinaryReader(fs);
				int i;
				//dll name
				string name = br.ReadString();
				if (dllName != name)
				{
					if (dllName == "")
					{
						//no dll loaded so load dll
						dll.Load(name, this);
					}
					else
					{
						//there is a dll loaded so ask if we want to load name
						string msg = "Currently loaded dll\n" + dllName + "\n\n";
						msg += "dll in .dsf\n" + name + "\n\n";
						msg += "Load dll in .dsf?";
						DialogResult dr = MessageBox.Show(msg, "dll_tool", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
						if (dr == DialogResult.Yes)
							dll.Load(name, this);
						else if (dr == DialogResult.Cancel)
							return;
					}
				}
				//edit il checkbox
				editIl.Checked = br.ReadBoolean();
				//exports
				int num = br.ReadInt32();
				string export, callconv, exportName;
				for (i=0; i<num; i++)
				{
					export = br.ReadString();
					callconv = br.ReadString();
					exportName = br.ReadString();
					//search for export in method list
					foreach(ListViewItem lvi in methodBox.Items)
					{
						if (lvi.Text == export)
						{
							ListViewItem lviEx = new ListViewItem(lvi.Text);
							lviEx.SubItems.Add(callconv);
							lviEx.SubItems.Add(exportName);
							lviEx.SubItems.Add(lvi.SubItems[1].Text);
							exportBox.Items.Add(lviEx);
							lvi.Remove();
						}
					}
				}
				//add stringtable checkbox
				stringCheck.Checked = br.ReadBoolean();
				//strings
				num = br.ReadInt32();
				for (i=0; i<num; i++)
				{
					stringBox.Items.Add(br.ReadString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (br != null)
					br.Close();
				if (fs != null)
					fs.Close();
			}
		}
	}

	private void saveDsf_Click(object sender, EventArgs e)
	{
		//save .dsf file
		if (dllName == "")
		{
			MessageBox.Show("No dll loaded!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save dll settings file";
		sfd.Filter = "Dll settings file (*.dsf)|*.dsf|All Files (*.*)|*.*";
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			BinaryWriter bw = null;
			try
			{
				fs = new FileStream(sfd.FileName, FileMode.Create);
				bw = new BinaryWriter(fs);
				//dll name
				bw.Write(dllName);
				//edit il checkbox
				bw.Write(editIl.Checked);
				//number of exports
				bw.Write(exportBox.Items.Count);
				//exports
				foreach(ListViewItem lvi in exportBox.Items)
				{
					bw.Write(lvi.Text);
					bw.Write(lvi.SubItems[1].Text);
					bw.Write(lvi.SubItems[2].Text);
				}
				//add string table checkbox
				bw.Write(stringCheck.Checked);
				//number of string table items
				bw.Write(stringBox.Items.Count);
				int i;
				for (i=0; i < stringBox.Items.Count; i++)
				{
					bw.Write(stringBox.Items[i].ToString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (bw != null)
					bw.Close();
				if (fs != null)
					fs.Close();
			}
		}
	}

	private void ilDasmVersion_SelectedIndexChanged(object sender, EventArgs e)
	{
		//ilDasm version changed
		ilDasm = (ilTool) Tools.IlDasm[ilDasmVersion.SelectedIndex];
	}

	private void ilAsmVersion_SelectedIndexChanged(object sender, EventArgs e)
	{
		//ilAsm version changed
		ilAsm = (ilTool) Tools.IlAsm[ilAsmVersion.SelectedIndex];
	}
}