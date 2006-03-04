#include <windows.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

//functions
int getSearchPaths(char *SearchPath[], int count);
int internalFilesSize(void *data);
HGLOBAL getMem(HGLOBAL block, int size);
int useMd5(void *data);

void debug_num(char *text, int num)
{
    char buffer[255];
    buffer[0] = 0;
    sprintf(buffer, "%s : %d", text, num);
    MessageBox(GetActiveWindow(), buffer, "debug", 0);
}

HGLOBAL DLL_EXPORT decompress_block(void *data, DWORD dataSize)
{
    int searchCount = 3;           // number of paths in search array
    char *searchPath[searchCount]; // array of paths to search for files
    int size, md5;

    HGLOBAL block = NULL;

    //fill searchPath array
    if (getSearchPaths(searchPath, searchCount) == 0)
    {
        //error filling serachPath array
        return;
    }

    //copy internal files to block
    size = internalFilesSize(data);
    block = getMem(block, size);
    CopyMemory(block, data, size);
    //skip internal files and null byte
    data += size + 1;

    //use md5 checksums for external files?
    md5 = useMd5(data);
    data += 1;
    debug_num("useMd5", md5);

    //copy external files to block

    //return block;
    return block;
}

int useMd5(void *data)
{
    //are external files useing checksums?
    char* dataByte;
    dataByte = data;
    return *dataByte;
}

int internalFilesSize(void *data)
{
    //write internal files to block
    int nameLen = 1;
    int dataLen;
    int* dataInt;
    void* dataStart = data;
    //find size of all internal files
    while (nameLen > 0)
    {
        //name
        dataInt = data;
        nameLen = *dataInt;
        if (nameLen > 0)
        {
            data += 4 + nameLen;
            //data
            dataInt = data;
            dataLen = *dataInt;
            data += 4 + dataLen;
        }
    }
    return data - dataStart;
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
