#pragma once
#include <windows.h>
#include <iostream>
#include <string>
#include <psapi.h>
using namespace std;

int main(int argc, char** argv);
int PrintModules(DWORD processID);

int main(int argc, char** argv)
{
	if (argc < 2)
	{
		cout << "Missing arguments" << endl;
		return -1;
	}

	if (strcmp("modules", argv[1]) == 0) {
        PrintModules(strtoul(argv[2], NULL, 10));
	}
	else if (strcmp("proc", argv[1]) == 0) {
		cout << GetProcAddress(GetModuleHandle(TEXT("kernel32.dll")), argv[2]) << endl;
	}
}

int PrintModules(DWORD processID)
{
    HMODULE hMods[1024];
    HANDLE hProcess;
    DWORD cbNeeded;
    unsigned int i;

    hProcess = OpenProcess(PROCESS_QUERY_INFORMATION |
        PROCESS_VM_READ,
        FALSE, processID);
    if (NULL == hProcess)
        return 1;

    if (EnumProcessModules(hProcess, hMods, sizeof(hMods), &cbNeeded))
    {
        for (i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
        {
            TCHAR szModName[MAX_PATH];
            if (GetModuleFileNameEx(hProcess, hMods[i], szModName,
                sizeof(szModName) / sizeof(TCHAR)))
            {
                wcout << szModName << endl;
            }
        }
    }

    CloseHandle(hProcess);
    return 0;
}