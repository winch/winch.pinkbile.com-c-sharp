dark injector
+++++++++++++

v0.2 19 June 2006

Usage
+++++
Select the dbpro exe you want to inject, it must be a normal exeuctable, not compressed or
with a .pck file.
Select the dll you wish to inject.
Press the "Inject dll" button and choose where to save the new exe.

Information
+++++++++++

Dbpro exes consist of a normal exe with data appended on the end. The exe part
is very similar between all dbpro exes. The same exe handles the three different types
of dbpro exes, normal, compressed and exe with .pck file.

With a compressed exe the exe sees that the first appended file is "compress.dll".
It then extracts "compress.dll" and then calls the decompress_block function within
the dll. To this function it passes a pointer to the rest of the appended data and
its size. The "compress.dll" then decompresses the data and returns a pointer to the
decompressed data back to the dbpro exe.

If we inject our own "compress.dll" as the first attached file then the dbpro exe will
extract our dll and call the compress_block function it contains passing a pointer to
the rest of the appended data and its size.

We can then just pass back the already decompressed appended data and the dbpro exe will
start as normal. We can also return 0 which will tell the dbpro exe an error occurred and
it will silently exit.

This is mainly useful if used to inject a user friendly directx version check but it
could be used to do anything that you can be done in a dll and needs to be done as soon
as possible after the exe starts.

This program provides a simple way to inject a dll as the first attached file and some
dlls to inject.

For more information on dbpro exes see
http://winch.pinkbile.com/dbpro_exe_format.php
http://dbp-unchained.tk/

license
+++++++

dark_injector Copyright (C) 2006 the_winch
Permission to copy, use, modify, sell and distribute this software is
granted provided this notice appears un-modified in all copies.
This software is provided as-is without express or implied warranty,
and with no claim as to its suitability for any purpose.