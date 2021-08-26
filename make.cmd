
cd RespectPhone
cd bin
cd Release

"C:\Program Files (x86)\WinRAR\WinRAR.exe" a -r -x*\app.publish -afzip rep_update.zip ^
 RespectPhone.exe^
 AutoUpdater.NET.dll^
 AutoUpdater.NET.pdb^
 AutoUpdater.NET^
 BouncyCastle.Crypto.dll^
 BouncyCastle.Crypto.xml^
 DnsClient.dll^
 DnsClient.xml^
 Microsoft.Extensions.Logging.Abstractions.dll^
 Microsoft.Extensions.Logging.Abstractions.xml^
 Microsoft.Win32.Registry.dll^
 Microsoft.Win32.Registry.xml^
 NAudio.Asio.dll^
 NAudio.Core.dll^
 NAudio.dll^
 NAudio.Midi.dll^
 NAudio.Wasapi.dll^
 NAudio.WinForms.dll^
 NAudio.WinMM.dll^
 NAudio.xml^
 Newtonsoft.Json.dll^
 Newtonsoft.Json.xml^
 RespectPhone.exe.config^
 RespectPhone.pdb^
 SIPSorcery.dll^
 SIPSorceryMedia.Abstractions.dll^
 System.Buffers.dll^
 System.Buffers.xml^
 System.Security.AccessControl.dll^
 System.Security.AccessControl.xml^
 System.Security.Principal.Windows.dll^
 System.Security.Principal.Windows.xml^
 websocket-sharp.dll^
 WpfAnimatedGif.dll^
 WpfAnimatedGif.pdb^
 WpfAnimatedGif.xml^
 Resources^
 
 
move rep_update.zip ../../../install
