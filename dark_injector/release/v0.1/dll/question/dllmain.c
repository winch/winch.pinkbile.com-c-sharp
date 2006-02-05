#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

#if BUILDING_DLL
# define DLLIMPORT __declspec (dllexport)
#else /* Not BUILDING_DLL */
# define DLLIMPORT __declspec (dllimport)
#endif /* Not BUILDING_DLL */

DLLIMPORT HGLOBAL decompress_block(void* data, DWORD dataSize)
{
    HGLOBAL block;
    
    if (IDYES == MessageBox(GetActiveWindow(), "Continue running exe?", "", MB_YESNO))
    {
        //for the dbpro exe to run it needs to recieve the data as if it has been decompressed
        //by the real compress.dll. Since the dbpro exe is not compressed we just need to allocate
        //some memory, copy the already uncompressed data to it then return the ptr to dbpro
        block = GlobalAlloc(GMEM_FIXED, dataSize);
        if (block)
        {
            CopyMemory(block, data, dataSize);
        }
    }
    else
    {
        //Returning 0 to dbpro will tell it an error has occured and the dbpro exe will silently exit
        block = (HGLOBAL) 0;
    }
    return block;
}


BOOL APIENTRY DllMain (HINSTANCE hInst     /* Library instance handle. */ ,
                       DWORD reason        /* Reason this function is being called. */ ,
                       LPVOID reserved     /* Not used. */ )
{
    switch (reason)
    {
      case DLL_PROCESS_ATTACH:
        break;

      case DLL_PROCESS_DETACH:
        break;

      case DLL_THREAD_ATTACH:
        break;

      case DLL_THREAD_DETACH:
        break;
    }

    /* Returns TRUE on success, FALSE on failure */
    return TRUE;
}
