#include <windows.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

//functions
int getSearchPaths(char *SearchPath[], int count);
int writeInternalFiles(HGLOBAL block, void* data);
HGLOBAL getMem(HGLOBAL block, int size);

HGLOBAL DLL_EXPORT decompress_block(void* data, DWORD dataSize)
{
    char *buffer;
    int searchCount = 3;           // number of paths in search array
    char *searchPath[searchCount]; // array of paths to search for files

    HGLOBAL block = NULL;
    char* nameLength;
    int* dataLength;
    void* blockdata;
    int i;
    char message[100];

    //fill searchPath array
    if (getSearchPaths(searchPath, searchCount) == 0)
    {
        //error filling serachPath array
        return;
    }
    return 0;

    //get intial block of memory

    //copy internal files to block
    writeInternalFiles(block, data);

    //return block;
    return block;
}

int writeInternalFiles(HGLOBAL block, void* data)
{
    //write internal files to block
    int nameLen = 1;
    return 0;
}

HGLOBAL getMem(HGLOBAL block, int size)
{
    //increases block by size
    if (block == 0)
    {
        //alloc
        block = GlobalAlloc(GMEM_FIXED, size);
    }
    else
    {
        //realloc
        block = GlobalReAlloc(block, size, 0);
    }
    if (block == 0)
    {
        MessageBox(GetActiveWindow(), "Global(Re)Alloc Failed!", "Error!", 0);
    }
    return block;
}

int getSearchPaths(char *searchPath[], int count)
{
    //fills the searchPath array with paths to search for files
    //returns 0 on failure > 0 on success
    char *buffer;
    DWORD reglen;
    HKEY hKey;
    long ret;
    //find location of dbpro dir
    ret = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Dark Basic\\Dark Basic Pro\\", 0, KEY_QUERY_VALUE, &hKey);
    if (ret != ERROR_SUCCESS)
    {
        MessageBox(GetActiveWindow(), "DarkBASIC pro location not found", "Error!", 16);
        return 0;
    }
    buffer = malloc(1024);
    ret = RegQueryValueEx(hKey, "INSTALL-PATH", NULL, NULL, (LPBYTE) buffer, &reglen);
    if (ret != ERROR_SUCCESS)
    {
        MessageBox(GetActiveWindow(), "DarkBASIC pro location not found", "Error!", 16);
        return 0;
    }
    RegCloseKey(hKey);
    //plugins
    searchPath[0] = malloc(strlen(buffer)+21); //"\compiler\plugins\"
    *searchPath[0] = 0x0;
    strcat(searchPath[0], buffer);
     strcat(searchPath[0], "\\compiler\\plugins\\");
    //plugins-user
    if (count > 1)
    {
        searchPath[1] = malloc(strlen(buffer)+25); //"\plugins-user\"
        *searchPath[1] = 0x0;
        strcat(searchPath[1], buffer);
        strcat(searchPath[1], "\\compiler\\plugins-user\\");
    }
    //effects
    if (count > 2)
    {
        searchPath[2] = malloc(strlen(buffer) + 20); //"\effects\"
        *searchPath[2] = 0x0;
        strcat(searchPath[2], buffer);
        strcat(searchPath[2], "\\compiler\\effects\\");
    }
    free(buffer);
    //add extra search paths here
    return 1;
}


BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            // attach to process
            // return FALSE to fail DLL load
            break;

        case DLL_PROCESS_DETACH:
            // detach from process
            break;

        case DLL_THREAD_ATTACH:
            // attach to thread
            break;

        case DLL_THREAD_DETACH:
            // detach from thread
            break;
    }
    return TRUE; // succesful
}
