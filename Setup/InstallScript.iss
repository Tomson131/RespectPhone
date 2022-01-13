#define AppIcoName "phico.ico"

[Setup]
AppName=RespectPhone
AppVersion=1.0
DefaultDirName={userappdata}\RespectApps\RespectPhone
DefaultGroupName=RespectApps
UninstallDisplayIcon={app}\phico.ico
Compression=lzma2
SolidCompression=yes
OutputDir=Output
PrivilegesRequired=admin
OutputBaseFilename=RespectPhoneSetup

[Files]
Source: "..\RespectPhone\bin\Release\aster_ari.config"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\AutoUpdater.NET.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\AutoUpdater.NET.pdb"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\AutoUpdater.NET.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\BouncyCastle.Crypto.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\BouncyCastle.Crypto.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\DnsClient.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\DnsClient.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Microsoft.Extensions.Logging.Abstractions.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Microsoft.Extensions.Logging.Abstractions.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Microsoft.Win32.Registry.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Microsoft.Win32.Registry.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.Asio.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.Core.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.Midi.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.Wasapi.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.WinForms.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.WinMM.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\NAudio.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\Newtonsoft.Json.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\RespectPhone.exe"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\RespectPhone.exe.config"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\RespectPhone.pdb"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\SIPSorcery.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\SIPSorcery.pdb"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\SIPSorceryMedia.Abstractions.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Buffers.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Buffers.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Security.AccessControl.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Security.AccessControl.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Security.Principal.Windows.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\System.Security.Principal.Windows.xml"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\websocket-sharp.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\WpfAnimatedGif.dll"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\WpfAnimatedGif.pdb"; DestDir: "{app}"
Source: "..\RespectPhone\bin\Release\WpfAnimatedGif.xml"; DestDir: "{app}"

Source: "..\RespectPhone\bin\Release\Resources\aa_off.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\aa_on.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\beep1.wav"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\cancel_transfer.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\list.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\loading.gif"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\m_off.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\m_on.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\phone_down.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\phone_up.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\Ringing.wav"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\sp_off.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\sp_on.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\transfer.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\tube_sound.wav"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\bin\Release\Resources\wf_ic_settings.png"; DestDir: "{app}\Resources"
Source: "..\RespectPhone\phico.ico"; DestDir: "{app}"

[Icons]
Name: "{group}\RespectPhone"; Filename: "{app}\RespectPhone.exe"
Name: "{userdesktop}\RespectPhone"; Filename: "{app}\RespectPhone.exe"; IconFilename: "{app}\{#AppIcoName}";