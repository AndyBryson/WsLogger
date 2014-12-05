WsLogger
========
Application to log GoFree data messages from Navico products.

TODO
----
<ul>
<li>Replace websocket-sharp -- it seems to cause a few issues</li>
<li>Remove 50 item limit</li>
<li>Re-write? It should probably handle state transitions better</li>
</ul>

Build Instructions
------------------
<h4>Windows</h4>
<p>Open WsLogger.sln in Visual Studio and build.</p>

<h4>Linux</h4>
<p>Install mono as per your platform. Then:<br>
mdtool build "--configuration:Release|x86"  WsLoggerMono.sln<br>
or run createLinuxPackage.sh<br>
or open WsLoggerMono.sln in MonoDevelop</p>


License
-------
This code is released under the MIT or GPLv2. Choose the one you like.
