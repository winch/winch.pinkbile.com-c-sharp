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
*/

//
//stuff for loading and saving dll
//

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

class dll
{
	public static void DoCommandLine(string[] args, main win)
	{
		//process command line args
		int i;
		string str;
		bool normalLoad = false;
		str = args[0];
		if (str.IndexOf("-a") == -1)
		{
			normalLoad = true;
		}
		for (i=0; i<args.Length; i++)
		{
			str = args[i];
			if (normalLoad == false)
			{
				if (File.Exists(str))
				{
					bool doItFound = false;
					Load(str, win);
					if (normalLoad == false)
					{
						foreach(ListViewItem lvi in win.methodBox.Items)
						{
							if (lvi.Text == "Do_It")
							{
								//move Do_It method to export list
								ListViewItem export = new ListViewItem(lvi.Text);
								if (win.exportsType.SelectedIndex == 0)
									export.SubItems.Add("Cdecl");
								else
									export.SubItems.Add("StdCall");
								export.SubItems.Add(lvi.Text);
								export.SubItems.Add(lvi.SubItems[1].Text);
								win.exportBox.Items.Add(export);
								lvi.Remove();
								doItFound = true;
							}
						}
						if (doItFound == false)
						{
							//Do_It mehod not in dll
							MessageBox.Show("Do_It method not found!", "dll_tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							//save dll as str.new
							dll.Save(str + ".new", win, true);
						}
					}
				}
				else
				{
					if (str != "-a")
						MessageBox.Show("*" + str + "*" + "\n\nFile not Found", "dll_tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		if (normalLoad == true)
		{
			win.Show();
		}
		else
		{
			Application.Exit();
		}
	}

	public static void Save(string fileName, main win, bool Quiet)
	{
		//show busy cursor
		Cursor.Current = Cursors.WaitCursor;
		//disable build button to prevent building twice
		win.build.Enabled = false;
		//find .corflags 0x00000002 and insert .vtfixup after it
		int i;
		win.lb.BeginUpdate();
		int slot = 1; //current slot number
		for (i = 0; i < win.lb.Items.Count; i++)
		{
			if (win.lb.Items[i].ToString().StartsWith(".corflags 0x00000002"))
			{
				//create VTableFixup table with enough slots for exported methods
				foreach (ListViewItem lvi in win.exportBox.Items)
				{
					win.lb.Items.Insert(i+1, ".vtfixup [1] int32 fromunmanaged at VT_0" + slot.ToString());
					win.lb.Items.Insert(i+2, ".data VT_0" + slot.ToString() + " = int32(0)");
					slot ++;
				}
			}
		}
		slot = 1;
		//.vtentry 1 : 1
		//.export [1] as SayHello
		//for each method in exportBox find it and export it
		int c;
		for (c = 0; c < win.exportBox.Items.Count; c++)
		{
			for (i = 0; i < win.lb.Items.Count; i++)
			{
				if (win.lb.Items[i].ToString() == win.exportBox.Items[c].SubItems[3].Text)
				{
					//change method to cdecl
					string meth = win.lb.Items[i].ToString();
					int end = meth.IndexOf("(");
					if (end > - 1)
					{
						int ii = end - 1;
						while (meth.Substring(ii,1) != " ")
							ii --;
						end = ii;
						meth = meth.Insert(end, " modopt([mscorlib]System.Runtime.InteropServices.CallConv" + win.exportBox.Items[c].SubItems[1].Text + ") ");
					}
					win.lb.Items[i] = meth;
					//found method so export it
					win.lb.Items.Insert(i+2, ".vtentry " + slot.ToString() + " : 1");
					//export method
					win.lb.Items.Insert(i+3, ".export ["+slot.ToString()+"] as " + win.exportBox.Items[c].SubItems[2].Text);
					slot ++; //increment slot number
				}
			}
		}
		//edit il if edit il before build checkbox is checked
		if (win.editIl.Checked == true)
		{
			editIlDialog.editIlasm(win.lb);
		}
		//save code in lb
		FileStream fs = null;
		StreamWriter sw = null;
		try
		{
			fs = new FileStream(win.TEMP + "_dll.il", FileMode.Create);
			sw = new StreamWriter(fs);
			for (i = 0; i < win.lb.Items.Count; i++)
				sw.WriteLine(win.lb.Items[i].ToString());
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			sw.Close();
			fs.Close();
		}
		//add string table to .res file if required
		if (win.stringCheck.Checked == true && win.stringBox.Items.Count > 0)
		{
			BinaryWriter bw = null;
			try
			{
				if (File.Exists(win.TEMP + "_dll.res"))
				{
					//if a .res file exists use it
					fs = new FileStream(win.TEMP + "_dll.res", FileMode.Append);
					bw = new BinaryWriter(fs);
				}
				else
				{
					//no .res file so create one
					fs = new FileStream(win.TEMP + "_dll.res", FileMode.Create);
					bw = new BinaryWriter(fs);
					//write empty resource
					bw.Write((int)0);
					bw.Write((int)32);
					bw.Write((ushort)65535);
					bw.Write((ushort)0);
					bw.Write((ushort)65535);
					bw.Write((ushort)0);
					bw.Write((int)0);
					bw.Write((int)0);
					bw.Write((int)0);
					bw.Write((int)0);
				}
				//if the first string table item is not blank insert a blank one as dbpro ignores first string table item
				if (win.stringBox.Items[0].ToString() != "")
					win.stringBox.Items.Insert(0,"");
				int item = 0; //current string table item
				int total = win.stringBox.Items.Count; //total string table items
				ushort block = 1; //current string table block (name in header)
				while (item < total)
				{
					//write resource entry header
					//data size
					int datasize = 0;
					//add size of each string in current block to datasize
					for (i=item; i<item+16; i++)
					{
						//get size of each string
						if (i < win.stringBox.Items.Count)
							datasize += win.stringBox.Items[i].ToString().Length;
					}
					//double datasize as strings in res are unicode
					datasize *= 2;
					//add 2bytes for each string table item string length
					datasize += 32; // 16 * 2
					bw.Write(datasize);
					//header size;
					bw.Write((int)32);
					//type
					bw.Write((ushort)65535);
					bw.Write((ushort)6);
					//name
					bw.Write((ushort)65535);
					bw.Write(block);
					block ++; //increase block number for next header
					//data version
					bw.Write((int)0);
					//flags
					//language
					bw.Write((int)0);
					//version
					bw.Write((int)0);
					//characteristics
					bw.Write((int)0);
					//resource header end
					//write resource data
					int times = item + 16;
					for (i=item; i<times; i++)
					{
						//if there is an item in stringBox
						if (i < win.stringBox.Items.Count)
						{
							//write length
							bw.Write((ushort) (win.stringBox.Items[i].ToString().Length));
							//write string
							bw.Write(Encoding.Unicode.GetBytes(win.stringBox.Items[i].ToString()));
							//increment item count
							item ++;
						}
						else
						{
							//write empty string item
							bw.Write((ushort) 0);
						}
					}
				}
				//remove the first blank string table item added earlier
				if (win.stringBox.Items[0].ToString() == "")
					win.stringBox.Items.RemoveAt(0);
			}
			catch (Exception ex) 
			{
				MessageBox.Show(ex.ToString(), "Error!");
			}
			finally
			{
				if (bw != null)
					bw.Close();
				if (fs != null)
					fs.Close();
			}
		}
		//MessageBox.Show("Dll about to be built");
		//build new dll
		//OUT:HelloWorldDll.dll HelloWorldDll.il /DLL /resource:HelloWorldDll.res
		Process process = new Process();
		process.StartInfo.FileName = "\"" + win.ilAsm.Path + "\"";
		process.StartInfo.Arguments = " /OUT:\"" + fileName + "\"" + " \"" + win.TEMP + "_dll.il\"" + " /DLL /resource:\"" + win.TEMP + "_dll.res\"";
		if (Quiet == false)
		{
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			string result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (result.IndexOf("Operation completed successfully") != -1)
				MessageBox.Show("Build sucessfull!","", MessageBoxButtons.OK, MessageBoxIcon.Information);
			else
			{
				DialogResult dr = MessageBox.Show("Build failed!\n\nSee ilasm output?","Fail!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if (dr == DialogResult.Yes)
				{
					//show ilasm output
					outputDialog od = new outputDialog();
					od.outputText = result;
					od.ShowDialog();
				}
			}
		}
		else
		{
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.WaitForExit();
		}
		//delete temp files
		if (File.Exists(win.TEMP + "\\_dll.il"))
			File.Delete(win.TEMP + "\\_dll.il");
		if (File.Exists(win.TEMP + "\\_dll.res"))
			File.Delete(win.TEMP + "\\_dll.res");
	win.lb.EndUpdate();
	//show normal cursor
	Cursor.Current = Cursors.Default;
	}

	public static void Load(string filename, main win)
	{
		//show busy cursor
		Cursor.Current = Cursors.WaitCursor;
		//delete any old .il and .res files
		if (File.Exists(Application.StartupPath + "\\_dll.il"))
			File.Delete(Application.StartupPath + "\\_dll.il");
		if (File.Exists(Application.StartupPath + "\\_dll.res"))
			File.Delete(Application.StartupPath + "\\_dll.res");
		//use ildasm to decompile dll to il code
		Process process = new Process();
		process.StartInfo.FileName = "\"" + win.ilDasm.Path +"\"";
		process.StartInfo.Arguments = " /OUT:\"" + win.TEMP + "\\_dll.il\" \"" + filename + "\"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		process.WaitForExit();
		//MessageBox.Show(process.StandardOutput.ReadToEnd());
		//check it worked
		if (! File.Exists(win.TEMP + "\\_dll.il"))
		{
			MessageBox.Show("ildasm failed!","Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		//enable build button
		win.build.Enabled = true;
		win.dllName = filename;
		//load .il into listbox
		win.lb.BeginUpdate();
		win.lb.Items.Clear();
		win.methodBox.BeginUpdate();
		win.methodBox.Items.Clear();
		win.exportBox.Items.Clear();
		win.stringBox.Items.Clear();
		FileStream fs = null;
		StreamReader sr = null;
		//DateTime startTime = DateTime.Now;
		try
		{
			fs = new FileStream(win.TEMP + "\\_dll.il", FileMode.Open);
			sr = new StreamReader(fs);
			string temp;
			string cat = null;
			while ((temp = sr.ReadLine()) != null)
			{
				temp = temp.Trim();
				if (temp.EndsWith(","))
				{
					cat = temp;
				}
				else
				{
					if (temp.Length > 0 && temp.StartsWith("//") == false)
					{
						if (cat == null)
						{
							win.lb.Items.Add(temp);
						}
						else
						{
							win.lb.Items.Add(cat + temp);
							cat = null;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		finally
		{
			sr.Close();
			fs.Close();
		}
		//remove comments from listview, this makes it easier to parse later
		//join lines that end in ","
		//change .corflags 0x00000001 to .corflags 0x00000002
		int i;
		string str = "";
		bool corflags = false;
		for (i = 0; i < win.lb.Items.Count; i++)
		{
			str = win.lb.Items[i].ToString();
			if (corflags == false)
			{
				if (str == ".corflags 0x00000001")
				{
					win.lb.Items[i] = ".corflags 0x00000002";
					corflags = true;
				}
			}
			if (str.StartsWith(".method"))
			{
				//if next line doesn't start with "{" stick it on the line with .method
				if (! win.lb.Items[i+1].ToString().StartsWith("{"))
				{
					win.lb.Items[i] = win.lb.Items[i].ToString() + " " + win.lb.Items[i+1].ToString();
					win.lb.Items.RemoveAt(i+1);
					//add method to methodBox
					str = win.lb.Items[i].ToString();
				}
				//don't add constructors or pivoke methods
				if (str.IndexOf(".ctor()") == -1 && str.IndexOf("pinvokeimpl") == -1)
				{
					//get method name
					int end = str.IndexOf("(");
					if (end > -1)
					{
						int start = str.LastIndexOf(" ",end, end-1) + 1;
						ListViewItem lvi = new ListViewItem(str.Substring(start,end-start));
						lvi.SubItems.Add(str);
						win.methodBox.Items.Add(lvi);
					}
				}
			}
		}
		win.methodBox.EndUpdate();
		win.lb.EndUpdate();
		//TimeSpan ts = DateTime.Now - startTime;
		//MessageBox.Show(ts.ToString());
		//show normal cursor
		Cursor.Current = Cursors.Default;
	}
}