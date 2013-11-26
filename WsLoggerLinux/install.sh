cp -r WebSocketLogger /usr/lib/
echo '#!/bin/bash' > WebSocketLogger.sh
echo 'mono /usr/lib/WebSocketLogger/WsLogger.exe' >> WebSocketLogger.sh
mv WebSocketLogger.sh /usr/bin/WebSocketLogger
chmod 755 /usr/bin/WebSocketLogger
