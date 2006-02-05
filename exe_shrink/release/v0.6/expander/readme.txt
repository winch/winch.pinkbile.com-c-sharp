To compile your own expander you need dev-c++ from http://www.bloodshed.net/

After you compile run shrink.bat which will reduce the size of the exe and rename it to expander.dat
Now just move patcher.dat into the same dir as builder.exe and build a patch.

To reduce the expander overhead as much as possible upx.exe is used by shrink.bat to compress the exe.
If you want the exe to be compressed get upx.exe from http://upx.sourceforge.net/ and put it in the
same directory as shrink.bat