; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

#define AppVersion GetFileVersion("RespectPhone\bin\Release\RespectPhone.exe")
#define AppID "4fb4ef8c-f5e7-4932-a612-c33a2ec38672"
#define AppCopyright "Copyright (C) 2014-2022 Respect"
#define AppName "RespectSoftPhone"
#define CompanyName "Respect"
#define SetupBaseName     "RSoftPhoneInstall"
#define AppIcoName "phico.ico"

[Setup]
AppVerName={cm:RespectPhone,{#AppVersion}}
AppName="RespectSoftPhone"
AppPublisher="OOO Respect"
AppPublisherURL=http://respectrb.ru/
AppVersion={#AppVersion}
DefaultDirName={userappdata}\{#CompanyName}\{#AppName}
DefaultGroupName={#CompanyName}
UninstallDisplayIcon={app}\RespectPhone.exe
Compression=zip
SolidCompression=true
OutputDir="install"
OutputBaseFilename={#SetupBaseName}
VersionInfoVersion={#AppVersion}
PrivilegesRequired=admin
AppID={#AppID}
UninstallDisplayName={#AppName}
;<<<<<<< HEAD
;=======
;{userappdata} = C:\Documents and Settings\username\Application Data
;>>>>>>> 5a1e138bcefc992f636cd7578969398d9b9f225c

[CustomMessages]
RespectPhone=RespectSoftPhone
MyAppName=RespectSoftPhone
MyAppVerName=EstVis %1
;nl.MyDescription=Mijn omschrijving
;nl.MyAppName=Mijn programma
;nl.MyAppVerName=Mijn programma %1
;de.MyDescription=Meine Beschreibung
;de.MyAppName=Meine Anwendung
;de.MyAppVerName=Meine Anwendung %1

[Files]
Source:"RespectPhone\bin\Release\RespectPhone.exe";DestDir:"{app}"
Source:"RespectPhone\bin\Release\AutoUpdater.NET.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\AutoUpdater.NET.pdb";DestDir:"{app}"
Source:"RespectPhone\bin\Release\AutoUpdater.NET.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\BouncyCastle.Crypto.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\BouncyCastle.Crypto.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\DnsClient.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\DnsClient.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Microsoft.Extensions.Logging.Abstractions.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Microsoft.Extensions.Logging.Abstractions.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Microsoft.Win32.Registry.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Microsoft.Win32.Registry.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.Asio.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.Core.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.Midi.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.Wasapi.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.WinForms.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.WinMM.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\NAudio.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Newtonsoft.Json.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\Newtonsoft.Json.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\RespectPhone.exe.config";DestDir:"{app}"
Source:"RespectPhone\bin\Release\RespectPhone.pdb";DestDir:"{app}"
Source:"RespectPhone\bin\Release\SIPSorcery.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\SIPSorceryMedia.Abstractions.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Buffers.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Buffers.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Security.AccessControl.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Security.AccessControl.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Security.Principal.Windows.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\System.Security.Principal.Windows.xml";DestDir:"{app}"
Source:"RespectPhone\bin\Release\websocket-sharp.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\WpfAnimatedGif.dll";DestDir:"{app}"
Source:"RespectPhone\bin\Release\WpfAnimatedGif.pdb";DestDir:"{app}"
Source:"RespectPhone\bin\Release\WpfAnimatedGif.xml";DestDir:"{app}"
 

Source: "RespectPhone\phico.ico"; DestDir: "{app}"
Source: "RespectPhone\Resources\aa_off.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\aa_on.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\cancel_transfer.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\list.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\loading.gif"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\m_off.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\m_on.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\phone_down.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\phone_up.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\sp_on.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\transfer.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\wf_ic_settings.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\beep1.wav"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\Ringing.wav"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\tube_sound.wav"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\away_off.png"; DestDir: "{app}\Resources"
Source: "RespectPhone\Resources\away_on.png"; DestDir: "{app}\Resources"

[Registry]
;Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "ESTVIS"; ValueData: """{app}\estvis.exe"""; Flags: uninsdeletevalue
;Root: HKCU; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "ESTVIS"; ValueData: """{app}\estvis.exe"""; Flags: uninsdeletevalue
;Source: "MyProg.chm"; DestDir: "{app}"
;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\RespectPhone"; Filename: "{app}\RespectPhone.exe"
Name: "{userdesktop}\RespectPhone"; Filename: "{app}\RespectPhone.exe"; IconFilename: "{app}\{#AppIcoName}"; Tasks: desktopicon


[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; 

[Dirs]
Name: "{app}"; 
Name: "{app}\tmp"; Permissions: everyone-full

;[Files]
;Source: tmp\*; DestDir: {app}\tmp\; Flags: ignoreversion recursesubdirs createallsubdirs; Permissions: everyone-full



[Code]
/////////////////////////////////////////////////////////////////////
function AppDataFolder(Param: String): String;
begin
//if IsTaskSelected('common') then
//    Result := ExpandConstant('{commonappdata}')
//  else
    Result := ExpandConstant('{localappdata}')
end;

/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#AppID}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

