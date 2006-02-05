/*
net_int
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
// functions related to generating the source file
//

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class Build
{
	public static void DoBuild(string fileName, string darkBasic, CheckedListBox plugins, CheckedListBox pluginsUser, string nameSpace, string className, template tp)
	{
		FileStream fs = null;
		StreamWriter sw = null;
		int tab = 0; //number of tabs to insert before text
		ArrayList al = new ArrayList();
		StringBuilder sb = new StringBuilder(255);
		string DebugStringTable = "";
		string DebugDll = "";
		try
		{
			fs = new FileStream(fileName, FileMode.Create);
			sw = new StreamWriter(fs);
			int i;
			//add dlls to arraylist
			//plugins
			for (i=0; i<plugins.Items.Count; i++)
			{
				if (plugins.GetItemChecked(i) == true)
				{
					al.Add(darkBasic + "plugins" + Path.DirectorySeparatorChar + plugins.Items[i].ToString());
				}
			}
			//plugins-user
			for (i=0; i<pluginsUser.Items.Count; i++)
			{
				if (pluginsUser.GetItemChecked(i) == true)
				{
					al.Add(darkBasic + "plugins-user" + Path.DirectorySeparatorChar + pluginsUser.Items[i].ToString());
				}
			}
			//write file header
			sw.Write(Parse(tp.header, "", "", className, nameSpace, tab, tp));
			if (tp.header.IndexOf("%t") != -1)
				tab ++;
			//write namepaceStart
			if (nameSpace != "")
			{
				sw.Write(Parse(tp.nameSpaceStart, "", "", className, nameSpace, tab, tp));
				if (tp.nameSpaceStart.IndexOf("%t") != -1)
				{
					tab ++;
					sw.Write("\t");
				}
			}
			//write classStart
			if (className != "")
			{
				sw.Write(Parse(tp.classStart, "", "", className, nameSpace, tab, tp));
				if (tp.classStart.IndexOf("%t") != -1)
				{
					tab ++;
					sw.Write("\t");
				}
			}
			//export methods for each dll
			foreach(string str in al)
			{
				int s = 0;
				int bad = 0;
				uint hinst = Build.LoadLibrary(str);
				DebugDll = str;
				while (bad < 5)
				{
					if (Build.LoadString(hinst, s, sb, 255) > 0)
					{
						DebugStringTable = sb.ToString();
						bad = 0;
						try
						{
							sw.Write(Parse(tp.method, DebugStringTable, Path.GetFileName(str), "", "", tab, tp));
						}
						catch (Exception ex)
						{
							//Error with string table item
							sw.WriteLine(tp.comment.Replace("%m", "Method write fail | " + ex.Message + " | " + DebugStringTable));
						}
					}
					else
					{
						bad ++;
					}
					s ++;
				}
			}
			//write classEnd
			if (tp.classStart != "" && className != "")
			{
				if (tp.classStart.IndexOf("%t") != -1)
					tab --;
				sw.Write(Parse(tp.classEnd, "", "", className, nameSpace, tab, tp));
			}
			//write namespaceEnd
			if (tp.nameSpaceStart != "" && nameSpace != "")
			{
				if (tp.nameSpaceStart.IndexOf("%t") != -1)
					tab --;
				sw.Write(Parse(tp.nameSpaceEnd, "", "", className, nameSpace, tab, tp));
			}
			//write file footer
			sw.Write(Parse(tp.footer, "", "", className, nameSpace, tab, tp));
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message + "\n\n" + DebugStringTable + "\n" + DebugDll, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (sw != null)
				sw.Close();
			if (fs != null)
				fs.Close();
		}
	}

	static string Parse(string text, string stringTable, string dll, string className, string nameSpace, int tab, template tp)
	{
		//replaces special characters %? in text with correct text
		int i;
		string tabs = "";
		text = text.Replace("%n",nameSpace); // %n nameSpace
		text = text.Replace("%c",className); // %c className
		text = text.Replace("%t", ""); // %t tab
		//stringTable
		if (stringTable != "")
		{
			string[] table = stringTable.Split("%".ToCharArray());
			if (table[2] == "??")
				throw new Exception("Bad entrypoint");
			text = text.Replace("%e", table[2]); // %e entrypoint
			text = text.Replace("%f", table[0].ToLower().Replace("[","").Replace(" ", "_").Replace("$", "")); // %f function name
			//return type
			if (table[0].IndexOf("[") != -1)
			{
				//return
				string ret = table[1].Substring(0, 1);
				table[1] = table[1].Substring(1, table[1].Length - 1);
				text = text.Replace("%r", GetReturnType(ret, tp));
			}
			else
			{
				//no return
				text = text.Replace("%r", tp.RZ);
			}
			//function parameters
			string p = "";
			string[] arg;
			//MessageBox.Show(stringTable + "\n" + table.Length.ToString());
			if (table.Length < 4)
				arg = null;
			else
				arg = table[3].Split(",".ToCharArray());
			int argNum = 1;
			for (i=0; i<table[1].Length; i++)
			{
				p += GetType(table[1].Substring(i, 1), tp);
				if (i != table[1].Length-1)
					p += ", ";
				p = p.Replace("%a", GetArgName(arg, argNum));
				argNum ++;
			}
			text = text.Replace("%p", p);
		}
		//dll
		if (dll != "")
		{
			text = text.Replace("%d", dll); // %d dll name
		}
		//add tab characters
		for (i=0; i<tab; i++)
			tabs += "\t";
		text = text.Replace("\n", "\n" + tabs);
		return text;
	}

	static string GetArgName(string[] args, int num)
	{
		//returns the parameter name using the last section of the string table
		//
		//SPRITE%LLLL%?Sprite@@YAXHHHH@Z%Sprite Number, XPos, YPos, Image Number
		//would give Sprite Xpos YPos Image
		//
		//if last section is blank use
		//a b c d e f g etc.
		if (args == null)
		{
			//use letters
			return Encoding.ASCII.GetString(new byte[] { (byte)(96 + num) });
		}
		else
		{
			//use string table
			num --;
			string ret = args[num].Trim().Split(" ".ToCharArray())[0];
			return ret;
		}
	}

	static string GetType(string type, template tp)
	{
		//returns the matching parameter type in tp from a dbpro string table type
		switch (type)
		{
			case "0":
				type = tp.Z;
				break;
			case "L":
				type = tp.L;
				break;
			case "D":
				type = tp.D;
				break;
			case "F":
				type = tp.F;
				break;
			case "S":
				type = tp.S;
				break;
			default:
				throw new Exception("Unknown Type " + type);
		}
		return type;
	}

	static string GetReturnType(string type, template tp)
	{
		//returns the matching return type in tp from a dbpro string table type
		switch (type)
		{
			case "L":
				type = tp.RL;
				break;
			case "D":
				type = tp.RD;
				break;
			case "F":
				type = tp.RF;
				break;
			case "S":
				type = tp.RS;
				break;
			default:
				throw new Exception("Unknown ReturnType " + type);
		}
		return type;
	}

	[DllImport("user32.dll", EntryPoint="LoadStringA")]
	public static extern int LoadString(uint hinst, int id, StringBuilder buffer, int bufferMax);

	[DllImport("kernel32.dll", EntryPoint="LoadLibrary")]
	public static extern uint LoadLibrary(string fileName);

	[DllImport("kernel32.dll", EntryPoint="FreeLibrary")]
	public static extern void FreeLibrary(uint hinst);

}