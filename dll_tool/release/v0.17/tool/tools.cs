/*
dll_tool2
Copyright (C) 2006 the_winch

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
//find installed versions of ilasm and ildasm
//

using System.Collections;
using Microsoft.Win32;
using System.IO;

sealed class tools
{
	private static tools theTools = null;
	public ArrayList IlAsm = new ArrayList();
	public ArrayList IlDasm = new ArrayList();

	public static tools GetInstance()
	{
		//create new instance if required
		if (theTools == null)
			theTools = new tools();
		return theTools;
	}

	private tools()
	{
		ilTool tool;
		string installRoot = null;
		//find locations of ildasm and ilasm
		RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework");
		if (reg != null)
		{
			if (reg.GetValue("sdkInstallRootv1.1") != null)
			{
				tool = new ilTool();
				tool.Path = (string)reg.GetValue("sdkInstallRootv1.1") + "bin\\ildasm.exe";
				tool.Version = "v1.1";
				IlDasm.Add(tool);
			}
			if (reg.GetValue("sdkInstallRootv2.0") != null)
			{
				tool = new ilTool();
				tool.Path = (string)reg.GetValue("sdkInstallRootv2.0") + "bin\\ildasm.exe";
				System.Windows.Forms.MessageBox.Show(tool.Path);
				tool.Version = "v2.0";
				IlDasm.Add(tool);
			}
			installRoot = (string)reg.GetValue("InstallRoot");
			reg.Close();
			//add custom
			tool = new ilTool();
			tool.Path = null;
			tool.Version = "Custom";
			IlDasm.Add(tool);
		}
		//find ilasm
		if (installRoot != null)
		{
			string[] dirs = Directory.GetDirectories(installRoot);
			//search install root for directories than contain ilasm.exe
			foreach (string str in dirs)
			{
				if (File.Exists(str + Path.DirectorySeparatorChar + "ilasm.exe"))
				{
					tool = new ilTool();
					//use directory as version string
					int last = str.LastIndexOf(Path.DirectorySeparatorChar);
					if (last != -1)
					{
						tool.Version = str.Substring(last + 1, str.Length - last - 1);
					}
					else
						tool.Version = str;
					tool.Path = str + Path.DirectorySeparatorChar + "ilasm.exe";
					IlAsm.Add(tool);
				}
			}
		}
		//custom
		tool = new ilTool();
		tool.Path = null;
		tool.Version = "Custom";
		IlAsm.Add(tool);
	}
}

class ilTool
{
	public string Path;
	public string Version;
}
