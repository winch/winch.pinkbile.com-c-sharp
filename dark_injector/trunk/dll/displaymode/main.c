#include <windows.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

HINSTANCE hInstance;
HGLOBAL retData;
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);

HGLOBAL DLL_EXPORT decompress_block(void *data, DWORD datasize)
{
    HWND hwnd;
    MSG msg;
    WNDCLASS wndclass;
    RECT size;
    retData = (HGLOBAL) 0;

    int width = 50;
    int height = 50;
    int x,y;

    hwnd = GetDesktopWindow();
    GetWindowRect(hwnd,&size);
    width = LOWORD(GetDialogBaseUnits());
    height = HIWORD(GetDialogBaseUnits());
    width *= 50.5;
    height *= 27.5;

    wndclass.style = CS_HREDRAW | CS_VREDRAW;
    wndclass.lpfnWndProc = WndProc;
    wndclass.cbClsExtra = 0;
    wndclass.cbWndExtra = 0;
    wndclass.hInstance = hInstance;
    wndclass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
    wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
    wndclass.hbrBackground = (HBRUSH) (COLOR_BTNFACE + 1);
    wndclass.lpszMenuName = NULL;
    wndclass.lpszClassName = "compress";
    RegisterClass(&wndclass);
    hwnd = CreateWindow("compress","Choose display mode", WS_CAPTION | WS_POPUPWINDOW | WS_MINIMIZEBOX, ((size.right-size.left)/2)-(width/2), ((size.bottom-size.top)/2)-(height/2), width, height, NULL, NULL, hInstance, NULL);
    ShowWindow(hwnd, SW_SHOWNORMAL);
    UpdateWindow(hwnd);

    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return retData;
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    static HWND btnOk, btnExit;
    switch (message)
    {
        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;

        case WM_CREATE:
            //
            return 0;
    }
    return DefWindowProc(hwnd, message, wParam, lParam);
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    hInstance = hinstDLL;
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
