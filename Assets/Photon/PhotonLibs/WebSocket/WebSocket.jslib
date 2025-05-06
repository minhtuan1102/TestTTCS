var LibraryWebSockets = {
$webSocketInstances: [],

<<<<<<< Updated upstream
SocketCreate: function(url, protocols)
=======
SocketCreate: function(url, protocols, openCallback, recvCallback, errorCallback, closeCallback)
>>>>>>> Stashed changes
{
    var str = UTF8ToString(url);
    var prot = UTF8ToString(protocols);
    var socket = {
        socket: new WebSocket(str, [prot]),
<<<<<<< Updated upstream
        buffer: new Uint8Array(0),
        error: null,
        messages: [],
        send: typeof(SharedArrayBuffer) == "function" ? // SharedArrayBuffer is available
    		function (socketInstance, ptr, length) {
                const socket = webSocketInstances[socketInstance];
                // depending on the caller, HEAPU8.buffer can be SharedArrayBuffer or ArrayBuffer in the same app
                const b = HEAPU8.buffer instanceof SharedArrayBuffer ? new Uint8Array(HEAPU8.slice(ptr, ptr + length)).buffer : new Uint8Array(HEAPU8.buffer, ptr, length);
                socket.socket.send(b);
            }
            :
            function (socketInstance, ptr, length) { // SharedArrayBuffer is not defined, ptr type is always ArrayBuffer
                const socket = webSocketInstances[socketInstance];
                socket.socket.send(new Uint8Array(HEAPU8.buffer, ptr, length));
            }
    }
    socket.socket.binaryType = 'arraybuffer';
    socket.socket.onmessage = function (e) {
//		if (e.data instanceof Blob)
//		{
//			var reader = new FileReader();
//			reader.addEventListener("loadend", function() {
//				var array = new Uint8Array(reader.result);
//				socket.messages.push(array);
//			});
//			reader.readAsArrayBuffer(e.data);
//		}
        if (e.data instanceof ArrayBuffer)
        {
            var array = new Uint8Array(e.data);
            socket.messages.push(array);
        }
    };
    socket.socket.onclose = function (e) {
        if (e.code != 1000)
        {
            if (e.reason != null && e.reason.length > 0)
                socket.error = e.reason;
            else
            {
                switch (e.code)
                {
                    case 1001:
                        socket.error = "Endpoint going away.";
                        break;
                    case 1002:
                        socket.error = "Protocol error.";
                        break;
                    case 1003:
                        socket.error = "Unsupported message.";
                        break;
                    case 1005:
                        socket.error = "No status.";
                        break;
                    case 1006:
                        socket.error = "Abnormal disconnection.";
                        break;
                    case 1009:
                        socket.error = "Data frame too large.";
                        break;
                    default:
                        socket.error = "Error "+e.code;
                }
            }
        }
    }
    var instance = webSocketInstances.push(socket) - 1;
=======
        error: null,
        sendBufForShared: null,
        send: typeof(SharedArrayBuffer) == "function" ? // SharedArrayBuffer is available and will not crash in 'isinstance' check
    		function (socketInstance, ptr, length) {
                if (HEAPU8.buffer instanceof SharedArrayBuffer) {
                    if (!this.sendBufForShared || this.sendBufForShared.byteLength < length) {
                        this.sendBufForShared = new ArrayBuffer(length);
                    }
                    var u8arr = new Uint8Array(this.sendBufForShared, 0, length);
                    u8arr.set(new Uint8Array(HEAPU8.buffer, ptr, length));
                    this.socket.send(u8arr);
                }  else {
                    this.socket.send(new Uint8Array(HEAPU8.buffer, ptr, length));
                }
            }
            :
            function (socketInstance, ptr, length) { // SharedArrayBuffer is not defined, ptr type is always ArrayBuffer
                this.socket.send(new Uint8Array(HEAPU8.buffer, ptr, length));
            }
    }
    var instance = webSocketInstances.push(socket) - 1;
    socket.socket.binaryType = 'arraybuffer';
    
    socket.socket.onopen = function () {
        {{{ makeDynCall('vi', 'openCallback') }}}(instance);
    }
    socket.socket.onmessage = function (e) {
        if (e.data instanceof ArrayBuffer)
        {
            const b = e.data;
            const ptr = _malloc(b.byteLength);
            const dataHeap = new Int8Array(HEAPU8.buffer, ptr, b.byteLength);
            dataHeap.set(new Int8Array(b));
            {{{ makeDynCall('viii', 'recvCallback') }}}(instance, ptr, b.byteLength);
            _free(ptr);
        }
    };
    socket.socket.onerror = function (e) {
        {{{ makeDynCall('vii', 'errorCallback') }}}(instance, e.code);
    }
    socket.socket.onclose = function (e) {
        if (e.code != 1000)
        {
            {{{ makeDynCall('vii', 'closeCallback') }}}(instance, e.code);
        }
    }
>>>>>>> Stashed changes
    return instance;
},

SocketState: function (socketInstance)
{
    var socket = webSocketInstances[socketInstance];
    return socket.socket.readyState;
},

SocketError: function (socketInstance, ptr, bufsize)
{
 	var socket = webSocketInstances[socketInstance];
 	if (socket.error == null)
 		return 0;
    stringToUTF8(socket.error, ptr, bufsize);
    return 1;
},

SocketSend: function (socketInstance, ptr, bufsize)
{
    var socket = webSocketInstances[socketInstance];
    socket.send(socketInstance, ptr, bufsize);
},

<<<<<<< Updated upstream
SocketRecvLength: function(socketInstance)
{
    var socket = webSocketInstances[socketInstance];
    if (socket.messages.length == 0)
        return 0;
    return socket.messages[0].length;
},

SocketRecv: function (socketInstance, ptr, length)
{
    var socket = webSocketInstances[socketInstance];
    if (socket.messages.length == 0)
        return 0;
    if (socket.messages[0].length > length)
        return 0;
    HEAPU8.set(socket.messages[0], ptr);
    socket.messages = socket.messages.slice(1);
},

=======
>>>>>>> Stashed changes
SocketClose: function (socketInstance)
{
    var socket = webSocketInstances[socketInstance];
    socket.socket.close();
}
};

autoAddDeps(LibraryWebSockets, '$webSocketInstances');
mergeInto(LibraryManager.library, LibraryWebSockets);
