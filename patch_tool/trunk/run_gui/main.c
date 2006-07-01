//
// © the_winch 2005
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
// http://winch.pinkbile.com : dbp@pinkbile.com

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <Wincrypt.h>

//functions
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
void doPatch(HWND hwnd);
int findHead(FILE *file);
int getByte(FILE *file);
char *getName(FILE *file);
void getProExe(HWND hwnd);

void MsgBoxInt(int i)
{
  char buffer[255];
  sprintf(buffer, "%i", i);
  MessageBox(NULL, buffer, "",MB_OK);
}

//globals
HWND btnPatch, editInfo, editExe;

typedef struct itm
{
  char *name;
  int action;
  int start;
  int length;
  int done;
} items;

char *proexe,*oldexe;
char *filename,*checksum;
FILE *patch, *fopen();    //patch file
FILE *f_newexe, *fopen(); // new patched exe
FILE *f_oldexe, *fopen(); // old proexe

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR CmdLine, int CmdShow)
{
    static TCHAR AppName[] = "builder";
    HMENU hmenu;
    HWND hwnd;
    MSG msg;
    WNDCLASS wndclass;
    int x,y;
    HWND d_hwnd;
    RECT size;

    char *name,*version,*buffer;

    wndclass.style = 0;
    wndclass.lpfnWndProc = WndProc;
    wndclass.cbClsExtra = 0;
    wndclass.cbWndExtra = 0;
    wndclass.hInstance = hInstance;
    wndclass.hIcon = LoadIcon(hInstance, "A");
    wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
    wndclass.hbrBackground = (HBRUSH) (COLOR_BTNFACE + 1);
    wndclass.lpszMenuName = NULL;
    wndclass.lpszClassName = AppName;

    if(!RegisterClass(&wndclass))
    {
		MessageBox(NULL, "Could not register window.", "Error!",MB_ICONEXCLAMATION | MB_OK);
		return -1;
	}

    //open patch file
    buffer = malloc(512);
    GetModuleFileName(NULL,buffer,512);
    patch = fopen(buffer,"rb");
    free(buffer);

    //get length of patch tool
    fseek(patch, 0, SEEK_END);
    long patchLength = ftell(patch);

    long dataStart = findHead(patch); //position in file patch data starts
    fseek(patch,dataStart,SEEK_SET);

    //check there is actually data appended to patch
    if (dataStart >= patchLength)
    {
       MessageBox(NULL, "No patch data.", "Error!",MB_ICONEXCLAMATION | MB_OK);
	   return -1;
    }

    d_hwnd = GetDesktopWindow();
    GetWindowRect(d_hwnd,&size);

    x = LOWORD(GetDialogBaseUnits());
    y = HIWORD(GetDialogBaseUnits());

    x *= 50.5;
    y *= 27.5;

    hwnd = CreateWindow(AppName,getName(patch), WS_CAPTION | WS_POPUPWINDOW | WS_MINIMIZEBOX, ((size.right-size.left)/2)-(x/2), ((size.bottom-size.top)/2)-(y/2), x, y, NULL, NULL, hInstance, NULL);
    ShowWindow(hwnd, CmdShow);
    UpdateWindow(hwnd);

    hmenu = GetSystemMenu(hwnd, FALSE);
    AppendMenu(hmenu, MF_SEPARATOR, 0, NULL);
    AppendMenu(hmenu, MF_STRING, 1, "About");

    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    return msg.wParam;
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    static HWND btnCancel,btnBrowse,frame,label;
    char *buffer;
    int cx,x,cy,y;

    switch (message)
    {
        case WM_CREATE:

            cx = LOWORD(GetDialogBaseUnits());
            cy = HIWORD(GetDialogBaseUnits());
            x = cx ; y = cx;

            //info textbox
            editInfo = CreateWindowEx(WS_EX_CLIENTEDGE, "edit", NULL, WS_CHILD | WS_BORDER | WS_VISIBLE | WS_HSCROLL | WS_VSCROLL | ES_READONLY | ES_LEFT | ES_MULTILINE | ES_AUTOHSCROLL | ES_AUTOVSCROLL,
                                    x,y,cx*48,cy*20, hwnd, (HMENU) 3, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);

            y += cy * 20.5;

            //text label
            label = CreateWindow("static", "exe to patch", WS_CHILD | WS_VISIBLE, x, y, 12*cx, cy, hwnd, (HMENU)4, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);

            x += cx * 11;

            //exe textbox
            editExe = CreateWindowEx(WS_EX_CLIENTEDGE, "edit", NULL, WS_CHILD | WS_VISIBLE | WS_BORDER | ES_READONLY | ES_LEFT | ES_AUTOHSCROLL,
                                    x,y,cx*32,cy*1.5, hwnd, (HMENU) 5, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);

            x += cx * 33;

            //exe browse button
            btnBrowse = CreateWindow("button", "...", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON, x, y, cx*4, cy * 1.5, hwnd, (HMENU) 6, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);

            x = cx * 39;
            y += cy * 2;

            //cancel button
            btnCancel = CreateWindow("button", "Exit", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON, x, y, 10*cx, cy*2, hwnd, (HMENU) 2, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);

            x -= cx * 12;

            //patch button
            btnPatch = CreateWindow("button", "Patch", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON, x, y, 10*cx, cy*2, hwnd, (HMENU) 1, ((LPCREATESTRUCT) lParam)-> hInstance, NULL);
            EnableWindow(btnPatch, FALSE);

            //filename
            filename = getName(patch);
            //checksum
            if (getByte(patch) == 1)
            {
                //read checksum
                checksum = calloc(33,1);
                fread(checksum,1,32,patch);
            }
            else
            {
                checksum = 0x0;
            }
            //info
            SetWindowText(editInfo,getName(patch));

            return 0;

        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;

        case WM_COMMAND:
            if (HIWORD(wParam) ==  BN_CLICKED)
            {
                if (lParam == (LPARAM) btnCancel)
                {
                    PostQuitMessage(0);
                    return 0;
                }
                if (lParam == (LPARAM) btnBrowse)
                {
                    getProExe(hwnd);
                }
                if (lParam == (LPARAM) btnPatch)
                {
                    doPatch(hwnd);
                }
            }
            return 0;

        case WM_SYSCOMMAND:
            if (LOWORD(wParam) == 1)
            {
                MessageBox(hwnd,"patch_tool v0.6\n\nhttp://winch.pinkbile.com/","",0);
                return 0;
            }
    }
    return DefWindowProc(hwnd, message, wParam, lParam);
}

void getMd5(char* checksum, void* buffer, int bufferLen)
{
    //get md5 checksum string of buffer
    HCRYPTPROV cryptProv;
    HCRYPTHASH hash;
    DWORD hashLen = 16;
    int i;
    unsigned char hashData[hashLen + 1];
    if(CryptAcquireContext(&cryptProv, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
    {
        CryptCreateHash(cryptProv, CALG_MD5, 0, 0, &hash);
        CryptHashData(hash, buffer, bufferLen, 0);
        CryptGetHashParam(hash, HP_HASHVAL, hashData, &hashLen, 0);
        for (i = 0; i < 16; i++)
        {
            sprintf(checksum + i * 2, "%02x", hashData[i]);
        }
        checksum[32] = 0;
        if(hash)
            CryptDestroyHash(hash);
        if(cryptProv)
            CryptReleaseContext(cryptProv,0);
    }
    else
    {
        checksum[0] = 0;
    }
}

void doPatch(HWND hwnd)
{
    long head;
    char *buffer;
    int numItems,i;

    char *oldname;     // name from oldexe
    char *oldnamepos;  // name from oldexe
    int action;        // was an action done?
    int oldlen;        // lenght from oldexe
    int oldlendata;    // lenght from oldexe for filedata

    //md5 checksum stuff
	char hex_output[32 + 1]; //generated hash

    //disable patch button
    EnableWindow(btnPatch, FALSE);

    //check checksum
    if (checksum != NULL)
    {
        f_oldexe = fopen(proexe,"rb");
        fseek(f_oldexe,0,SEEK_END);
        oldlen = ftell(f_oldexe);
        buffer = malloc(oldlen);
        fseek(f_oldexe,0,SEEK_SET);
        fread(buffer,1,oldlen,f_oldexe);
        fclose(f_oldexe);
        getMd5(hex_output, buffer, oldlen);
        MessageBox(GetActiveWindow(), hex_output, "", 0);
        free(buffer);
        //check checksums match
        if (strcmp(hex_output,checksum) != 0)
        {
            MessageBox(hwnd, "Target exe does not match checksum\nPatch failed","Error!",16);
            return;
        }
    }

    //rename proexe to proexe + ".old"
    oldexe = malloc(strlen(proexe)+4);
    *oldexe = 0x0;
    strcat(oldexe,proexe);
    strcat(oldexe,".old");
    DeleteFile(oldexe);
    MoveFile(proexe,oldexe);

    //open old exe and new exe
    f_oldexe = fopen(oldexe,"rb");
    f_newexe = fopen(proexe,"wb");

    //find head of f_oldexe and write it to f_newexe
    head = findHead(f_oldexe);

    //write head to newexe
    rewind(f_oldexe);
    buffer = malloc(head);
    fread(buffer,1,head,f_oldexe);
    fwrite(buffer,1,head,f_newexe);
    free(buffer);

    //get a list of files in patch
    numItems = getByte(patch);
    items Items[numItems+1];
    //fill the list
    for (i=0; i<numItems; i++)
    {
        Items[i].action = getByte(patch);
        Items[i].name = getName(patch);
        Items[i].start = ftell(patch)+4;
        Items[i].done = 0;
        //if action = add/replace
        if (Items[i].action == 0)
        {
            fread(&Items[i].length,1,4,patch);
            fseek(patch,Items[i].length,SEEK_CUR);
        }
    }

    //for each file in proexe check if there is a file in the patch
    oldlen = 1;
    while (oldlen > 0 && oldlen < 100)
    {
        //get length
        fread(&oldlen,1,4,f_oldexe);
        //is it a file)
        if (oldlen > 0 && oldlen < 100)
        {
            //it's a file
            //get name
            oldname = malloc(oldlen+1);
            oldnamepos = oldname;
            for (i=0; i<oldlen; i++)
            {
                *oldnamepos = fgetc(f_oldexe);
                oldnamepos ++;
            }
            *oldnamepos = 0x0;
            //check if oldname matches a name in patch
            action = 0;
            for (i=0 ;i<numItems; i++)
            {
                if (Items[i].done == 0 && strcmp(oldname, Items[i].name) == 0)
                {
                    //there is a match so perform action 0=add/replace 1=remove
                    switch (Items[i].action)
                    {
                        case 0 :
                        // add replace
                        //write name
                        fwrite(&oldlen,1,4,f_newexe);
                        fwrite(oldname,1,oldlen,f_newexe);
                        //write data
                        fwrite(&Items[i].length,1,4,f_newexe);
                        buffer = malloc(Items[i].length);
                        fseek(patch,Items[i].start,SEEK_SET);
                        fread(buffer,1,Items[i].length,patch);
                        fwrite(buffer,1,Items[i].length,f_newexe);
                        free(buffer);
                        //skip date in oldexe
                        fread(&oldlendata,1,4,f_oldexe);
                        fseek(f_oldexe,oldlendata,SEEK_CUR);
                        break;

                        case 1 :
                        // remove
                        //skip date in oldexe
                        fread(&oldlendata,1,4,f_oldexe);
                        fseek(f_oldexe,oldlendata,SEEK_CUR);
                        break;
                    }
                Items[i].done = 1;
                action = 1;
                }
            }
            if (action == 0)
            {
            // if an action hasn't been taken write the file to new exe
            //write name
            fwrite(&oldlen,1,4,f_newexe);
            fwrite(oldname,1,oldlen,f_newexe);
            //get length of data
            fread(&oldlendata,1,4,f_oldexe);
            //write length
            fwrite(&oldlendata,1,4,f_newexe);
            //get and write data
            buffer = malloc(oldlendata);
            fread(buffer,1,oldlendata,f_oldexe);
            fwrite(buffer,1,oldlendata,f_newexe);
            free(buffer);
            }
        free(oldname);
        }
        else
        {
            //it's extra or compressed block
            //first check for any actions that have not been done
            for (i=0 ;i<numItems; i++)
            {
                if (Items[i].done == 0)
                {
                    //items action not done
                    switch (Items[i].action)
                    {
                        case 0 :
                        // add/replace
                        //write name
                        oldlendata = strlen(Items[i].name);
                        fwrite(&oldlendata,1,4,f_newexe);
                        fwrite(Items[i].name,1,oldlendata,f_newexe);
                        //write filedata
                        fwrite(&Items[i].length,1,4,f_newexe);
                        buffer = malloc(Items[i].length);
                        fseek(patch,Items[i].start,SEEK_SET);
                        fread(buffer,1,Items[i].length,patch);
                        fwrite(buffer,1,Items[i].length,f_newexe);
                        free(buffer);
                        break;

                        case 1 :
                        //remove
                        buffer = malloc(23+strlen(Items[i].name));
                        *buffer = 0x0;
                        strcat(buffer,Items[i].name);
                        strcat(buffer," Could not be removed!");
                        MessageBox(hwnd,buffer,"Warning!",48);
                        break;
                    }
                    Items[i].done = 1;
                }
            }
            // then write extra or compressed block
            fwrite(&oldlen,1,4,f_newexe);
            while (!feof(f_oldexe))
            {
                oldlendata = fgetc(f_oldexe);
                if (!feof(f_oldexe))
                    putc(oldlendata,f_newexe);
            }
        }
    }

    //clean up
    fclose(f_oldexe);
    fclose(f_newexe);
    fclose(patch);

    //Success Message
    MessageBox(hwnd,"Patch Success!","",64);
}

int findHead(FILE *file)
{
  //finds the end of exe which is start of attache files in pro exe and patch info in patch
  //skip dos stub
  fseek(file, 60, SEEK_SET);
  int e_lfannew;
  fread(&e_lfannew, 1, 4, file);
  fseek(file, e_lfannew + 6, SEEK_SET);
  //IMAGE_FILE_HEADER
  short NumberOfSections;
  fread(&NumberOfSections, 1, 2, file);
  fseek(file, 240, SEEK_CUR);
  //end of IMAGE_OPTIONAL_HEADER
  //section directories
  int Size = 0; // size of section
  int Pos = 0;  // position of section
  int i;
  for (i=0; i<NumberOfSections; i++)
  {
      fseek(file, 16, SEEK_CUR);
      fread(&Size, 1, 4, file);
      fread(&Pos, 1, 4, file);
      fseek(file, 16, SEEK_CUR);
  }
  //end of section directories
  return (Pos + Size);
}

int getByte(FILE *file)
{
  //reads a byte from file
  unsigned char c;
  fread(&c,1,1,file);
  return((int) c);
}

char *getName(FILE *file)
{
    //reads a name from file
    int len;
    char *str;
    //get length
    fread(&len,1,4,file);
    //get name
    str = calloc(len+1,1);
    fread(str,1,len,file);
    return(str);
}

void getProExe(HWND hwnd)
{
      //Show an open file dialog to locate the dbpro exe to be patched
      int i;
      char *filter,*filterpos,*filepos,*file;
      filter = malloc((strlen(filename)*2)+3);
      file = malloc(255);
      *file = 0x0;
      *filter = 0x0;
      filterpos = filter;
      filepos = filename;
      for (i=0; i<strlen(filename); i++)
      {
            *filterpos = *filepos;
            filterpos ++;
            filepos ++;
      }
      *filterpos = 0x0;
      filterpos ++;
      filepos = filename;
      for (i=0; i<strlen(filename); i++)
      {
            *filterpos = *filepos;
            filterpos ++;
            filepos ++;
      }
      *filterpos = 0x0;
      filterpos ++;
      *filterpos = 0x0;

      OPENFILENAME ofn;
      ZeroMemory(&ofn, sizeof(OPENFILENAME));
      ofn.lStructSize = sizeof(ofn);
      ofn.hwndOwner = hwnd;
      ofn.lpstrFile = 0x0;
      ofn.lpstrTitle = "Select File to Patch";
      ofn.lpstrFilter = filter;
      ofn.nMaxFile = MAX_PATH;
      ofn.nMaxFileTitle = MAX_PATH;
      ofn.lpstrFile = file;
      ofn.Flags = OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST;
      if (GetOpenFileName(&ofn))
      {
            //return filename
            free(&ofn);
            free(&filter);
            SetWindowText(editExe,file);
            EnableWindow(btnPatch, TRUE);
            proexe = file;
            return;
      }
      else
      {
            //return nothing
            free(&ofn);
            free(&filter);
            free(&file);
            return;
      }
}

