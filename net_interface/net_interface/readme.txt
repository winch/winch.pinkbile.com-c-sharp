net_int - call dbpro dlls from .net dlls
++++++++

dbp@pinkbile.com
http://winch.pinkbile.com/

requires .Net SDK

version history
+++++++++++++++

v0.6 28 september 2005
- Removed annoying messageBox from net_tool

v0.5 23 July 2005
-net_int now uses templates for language support. This means you can modify templates to change the output code.
 New templates can also be added to support other languages. Templates are included for C# and vb.net
-Function arguments now created using the description section of the string table if it exists.
-net_tool now creates exe in .net dll directory and doesn't delete it after it finishes running

v0.4 06 July 2005
-net_int now works better with string tables that have gaps

v0.3 05 July 2005
-added net_tool.

v0.2 01 July 2005
-added search box so you can easily locate which dll a function is in
-added vb.net export

v0.1 29 June 2005
-initial release

Desciption
++++++++++

This program will use the string tables of the various dbpro dlls to generate a source file that allows you to
call the functions in the dbpro dlls from tpc dlls. Currently there is only support for C# and vb.net but by making
a template you can add support for other languages.

net_int usage
+++++++++++++

Select the dlls you want to use funtions from. Optionally enter a namespace in the namespace text box. Enter a name for
the generated class in the classname text box.
Choose which template you want the exported code to use.
Click the build button and choose where to save the generated file.
Write a dll using the funtions.
Use dll_tool to convert the dll to a dbpro tpc dll.
dll_tool can be downloaded from http://winch.pinkbile.com/dll_tool.php
There is a step by step guide to dll_tool at http://winch.pinkbile.com/dll_tool_doc/

Searching.
Click the build button in the search groupbox. This scans the string tables and builds a list of commands.
Enter terms in the text box and it will search the list of commands and show you will dll the command is in.
Currently only dbpro dlls are searched not third party dlls in plugins-user.

Templates.
Templates are stored in the templates directory. When the net_int starts it searches the templates dir for
*.txt files and loads any it finds. You can select the template to use on export from the combo box.
Read templates.txt if you want to modify or create new templates.

net_tool usage
++++++++++++++

net_tool requires dll_tool v0.11 or newer, download from http://winch.pinkbile.com/dll_tool.php
Open the dbproexe dir and move net_int.dll to your plugins-user directory.
Compile the dbpro project in the dbrproexe dir.
Start net_tool.
Click "dll_tool location" and browse to dll_tool.exe
Click "Dbpro exe location" and browse to the dbpro exe you just compiled.
Adjust the display settings to those you want the exe to use.
Click the ".Net dll location" button and browse to the location you ide compiles to.
Click the Go button.

Net_tool will now check the .Net dll location for a the dll every interval.
When it finds a dll it;

1. Runs it through dll_tool which will export a method call Do_It if found and save the dll with
.new appended to the filename

2. Copies Dbpro exe to the .net dll directory and replaces net_int.dll with the dll produced by dll_tool

3. Run the new dbpro exe

Todo
++++

Write ide plugins to do the work of net_tool.

license
+++++++

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