# KioskBrowser
WPF application using WebView2 to show window in full screen without title bar.

**Setup**

WebView2 currently do not support the stable version of Edge Chromium, so you have a couple of options for what to install for using WebView2.

Download the WebView2 Runtime.<br/> 
https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section
 
Else you can install Edge chromium Beta<br/>
https://www.microsoftedgeinsider.com/nb-no/download


**Usage**
```console
KioskBrowser.exe "http://www.google.com"
```

Close window by pressing ESC. Make sure that window have fous.
