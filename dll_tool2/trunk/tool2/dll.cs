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

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

class dll
{
	tools Tools = new tools(); //remove this
	string dllFile;
	string tempDir;
	readonly string ilFile = "dll.il";
	ilTool ilAsm = null;
	ilTool ilDasm = null;

	public dll(string DllFilename)
	{
		dllFile = DllFilename;
		//do this properly
		tempDir = Path.GetTempFileName();
		File.Delete(tempDir);
		Directory.CreateDirectory(tempDir);

		//just use the first ilAsm and ilDasm for now
		ilAsm = (ilTool)Tools.IlAsm[0];
		ilDasm = (ilTool)Tools.IlDasm[0];
	}

	~dll()
	{
		//clean up temp dir
		if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
	} 

	public void Load()
	{
		if (File.Exists(dllFile) == false)
			throw new FileNotFoundException(null, dllFile);
		if (File.Exists(ilDasm.Path) == false)
			throw new FileNotFoundException(null, ilDasm.Path);
		Process process = new Process();
		process.StartInfo.FileName = ilDasm.Path;
		process.StartInfo.Arguments += "\"" + dllFile + "\"";
        process.StartInfo.Arguments += " /TEXT /OUT:\"" + tempDir + Path.DirectorySeparatorChar + ilFile + "\"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		while (process.HasExited == false)
			Application.DoEvents();
		if (File.Exists(tempDir + Path.DirectorySeparatorChar + ilFile) == false)
		{
			throw new Exception("ilDasm failed");
		}
	}

	public void Save(string fileName)
	{
		if (File.Exists(ilAsm.Path) == false)
			throw new FileNotFoundException(null, ilAsm.Path);
	}
}