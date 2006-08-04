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
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

class dll
{
	tools Tools = tools.GetInstance(); //remove this
	string dllFile;
	string tempDir;
	readonly string ilFileName = "dll.il";
	string ilFile;
	string resFile;
	ArrayList ilCode = new ArrayList();
	ilTool ilAsm = null;
	ilTool ilDasm = null;

	public dll(string DllFilename)
	{
		dllFile = DllFilename;
		//do this properly
		tempDir = Path.GetTempFileName();
		ilFile = tempDir + Path.DirectorySeparatorChar + ilFileName;
		resFile = Path.GetDirectoryName(ilFile) + Path.DirectorySeparatorChar
			+ Path.GetFileNameWithoutExtension(ilFile) + ".res";
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
			throw new FileNotFoundException("Dll file not found", dllFile);
		if (File.Exists(ilDasm.Path) == false)
			throw new FileNotFoundException("ilDasm exe not found", ilDasm.Path);
		Process process = new Process();
		process.StartInfo.FileName = ilDasm.Path;
		process.StartInfo.Arguments += "\"" + dllFile + "\"";
        process.StartInfo.Arguments += " /TEXT /OUT:\"" + ilFile + "\"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		while (process.HasExited == false)
			Application.DoEvents();
		if (File.Exists(ilFile) == false)
		{
			throw new FileNotFoundException("ilDasm did not output .il file.", ilFileName);
		}
		FileStream fs = null;
		StreamReader sr = null;
		try
		{
			fs = new FileStream(ilFile, FileMode.Open);
			sr = new StreamReader(fs);
			while (sr.Peek() > 0)
			{
				ilCode.Add(sr.ReadLine());
			}
		}
		catch (Exception ex)
		{
			throw new FileLoadException("Unable to load .il file", ilFile);
		}
		finally
		{
			if (sr != null)
				sr.Close();
			if (fs != null)
				fs.Close();
		}
	}

	public void Save(string fileName)
	{
		if (File.Exists(ilAsm.Path) == false)
			throw new FileNotFoundException("ilAsm exe not found", ilAsm.Path);
		if (File.Exists(ilFile) == false)
            throw new FileNotFoundException(".il file not found", ilFile);
		FileStream fs = null;
		StreamWriter sw = null;
		//write il file
		try
		{
			fs = new FileStream(ilFile, FileMode.Create);
			sw = new StreamWriter(fs);
			foreach (string str in ilCode)
			{
				sw.WriteLine(str);
			}
		}
		catch (Exception ex)
		{
			//TODO find appropriate exception to throw here
			throw ex;
		}
		finally
		{
			if (sw != null)
				sw.Close();
			if (fs != null)
				fs.Close();
		}
		//write res file
		//compile dll
		Process process = new Process();
		process.StartInfo.FileName = ilAsm.Path;
		//" /OUT:\"" + fileName + "\"" + " \"" + win.TEMP + "_dll.il\"" + " /DLL /resource:\"" + win.TEMP + "_dll.res\"";
		process.StartInfo.Arguments = " /DLL /OUTPUT=\"" + fileName + "\"";
		if (File.Exists(resFile))
			process.StartInfo.Arguments += " /RESOURCE=\"" + resFile + "\"";
		process.StartInfo.Arguments += " \"" + ilFile + "\"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();
		string result = process.StandardOutput.ReadToEnd();
		while (process.HasExited == false)
			Application.DoEvents();
	}
}