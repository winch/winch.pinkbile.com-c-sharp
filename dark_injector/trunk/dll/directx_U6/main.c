#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

#if BUILDING_DLL
# define DLLIMPORT __declspec (dllexport)
#else /* Not BUILDING_DLL */
# define DLLIMPORT __declspec (dllimport)
#endif /* Not BUILDING_DLL */

HGLOBAL decompress_block(void* data, DWORD dataSize)
{
    HGLOBAL block;
	HINSTANCE dxDll;
    char versionString[255];
    int hasDirectx = 0;

    //required directx version 9.0c (December 2005)
    //4.09.00.0904
    //   \  \   \
    //    \  \   sub
    //     \  minor
    //      major
	//plus check for d3dx9_28.dll
    int majorReq = 9;
    int minorReq = 0;
    int subReq = 904;

    int major, minor, sub;

    //get directx version string from registry
    HKEY hKey;
    long ret;
    DWORD reglen = 255;
    //open key
    ret = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\DirectX\\", 0, KEY_QUERY_VALUE, &hKey);
    if (ret == ERROR_SUCCESS)
    {
        //read version string
        ret = RegQueryValueEx(hKey, "Version", NULL, NULL, (LPBYTE) versionString, &reglen);
        if (ret == ERROR_SUCCESS)
        {
            //split version string into major, minor, sub
            char temp[5];
            temp[4] = 0;
            temp[2] = 0;
            //major
            temp[0] = versionString[2];
            temp[1] = versionString[3];
            major = atoi(temp);
            //minor
            temp[0] = versionString[5];
            temp[1] = versionString[6];
            minor = atoi(temp);
            //sub
            temp[0] = versionString[8];
            temp[1] = versionString[9];
            temp[2] = versionString[10];
            temp[3] = versionString[11];
            sub = atoi(temp);
            //check version is equal or newer.
            if (major == majorReq)
            {
                //major == required so need to check minor
                if (minor == minorReq)
                {
                    //minor == required so need to check sub
                    if (sub >= subReq)
                    {
                        //sub is bigger or equal to required so version is new enough
                        hasDirectx = 1;
                    }
                    else
                    {
                        //sub is smaller so version is too old
                        hasDirectx = 0;
                    }
                }
                else
                {
                    if (minor > minorReq)
                    {
                        //minor is bigger than required so must be newer version
                        hasDirectx = 1;
                    }
                    else
                    {
                        //minor is smaller than required so must be older version
                        hasDirectx = 0;
                    }
                }
            }
            else
            {
                if (major > majorReq)
                {
                    //major is bigger than required so must be newer version
                    hasDirectx = 1;
                }
                else
                {
                    //major is smaller than required so must be lower version
                    hasDirectx = 0;
                }
            }
        }
    }
    RegCloseKey(hKey);

    if (hasDirectx == 1)
	{
		//check for d3dx9_28.dll
		dxDll = LoadLibrary("d3dx9_28.dll");
		if (dxDll == NULL)
		{
			//d3dx9_28.dll not found
			hasDirectx = 0;
		}
		else
		{
			FreeLibrary(dxDll);
		}
	}


    if (hasDirectx == 0)
    {
        //directx is too old
        MessageBox(GetActiveWindow(), "DirectX 9.0c (December 2005) or later is required to run this program.\n\nThe latest version of DirectX can be downloaded from\nhttp://www.microsoft.com/directx", "Error!", 0);
        block = (HGLOBAL) 0;
    }

    if (hasDirectx == 1)
    {
        //directx is new enough so load exe
        block = GlobalAlloc(GMEM_FIXED, dataSize);
        if (block)
        {
            CopyMemory(block, data, dataSize);
        }
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
