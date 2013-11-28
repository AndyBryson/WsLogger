#!/bin/bash
#
# Create a tar.gz containing websocket logger stuff, including an installer
#
mdtool build "--configuration:Release|x86"  WsLoggerMono.sln 
rm -r WsLoggerLinux
mkdir WsLoggerLinux
mkdir WsLoggerLinux/WebSocketLogger
cd WsLoggerLinux
cp ../WsLogger/bin/Release/*.dll WebSocketLogger/
cp ../WsLogger/bin/Release/*.exe WebSocketLogger/
cp ../WsLogger/bin/Release/*.xml WebSocketLogger/
rm install.sh
rm WebSocketLogger.sh
echo "cp -r WebSocketLogger /usr/lib/" > install.sh
echo "echo '#!/bin/bash' > WebSocketLogger.sh" >> install.sh
echo "echo 'mono /usr/lib/WebSocketLogger/WsLogger.exe' >> WebSocketLogger.sh" >> install.sh
echo "mv WebSocketLogger.sh /usr/bin/WebSocketLogger" >> install.sh
echo "chmod 755 /usr/bin/WebSocketLogger" >> install.sh 
cd ..
tar -zcvf WebSocketLogger.tar.gz WsLoggerLinux
