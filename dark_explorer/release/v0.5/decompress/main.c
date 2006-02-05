#include <windows.h>
#include <stdio.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

// a sample exported function
void DLL_EXPORT decompress(char *oldExeName, int exeSection, int dataOffset, char *newExeName, char *compressDll)
{
    void* (*decomp)(void*, int);
    FILE *oldExe = fopen(oldExeName, "rb");
    FILE *newExe = fopen(newExeName, "wb");
    HANDLE lib = LoadLibrary(compressDll);
    SIZE_T decompSize;
    long time;
    float seconds;
    char extraData[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4E,
                         0x61, 0xBC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    char extra58[] = { 0x10, 0x01, 0x00 };
    char extra59[] = { 0x50, 0x01, 0x00 };
    decomp = (void* (*)(void*, int)) GetProcAddress(lib, "decompress_block");
    void *buffer;

    //get compressed data size
    fseek(oldExe, 0, SEEK_END);
    int dataSize = ftell(oldExe) - dataOffset;
    fseek(oldExe, 0, SEEK_SET);

    //write exe section
    buffer = malloc(exeSection);
    fread(buffer, exeSection, 1, oldExe);
    fwrite(buffer, exeSection, 1, newExe);
    free(buffer);

    //load compressed data into buffer;
    fseek(oldExe, dataOffset, SEEK_SET);
    buffer = malloc(dataSize);
    fread(buffer, dataSize, 1, oldExe);

    //decompress data
    time = GetTickCount();
    void *data = decomp(buffer, dataSize);
    time = GetTickCount() - time;
    seconds = time / 1000.0;
    free(buffer);
    data = GlobalLock((HGLOBAL) data);
    decompSize = GlobalSize((HGLOBAL) data);

    //write decompressed data
    fwrite(data, decompSize, 1, newExe);
    GlobalUnlock((HGLOBAL) data);

    //write extra data
    fwrite(extraData, 17, 1, newExe);
    if (exeSection == 69632)
    {
        //5.8 exe
        write(extra58, 3, 1, newExe);
    }
    else
    {
        //5.9 exe
        fwrite(extra59, 3, 1, newExe);
    }

    //show decompression stats message box
    char msg[255];
    msg[0] = 0;
    sprintf(msg, "Decompress complete in %.2f seconds", seconds);
    MessageBox(GetActiveWindow(), msg, "dark_explorer", 0);

    FreeLibrary(lib);
    fclose(oldExe);
    fclose(newExe);
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
