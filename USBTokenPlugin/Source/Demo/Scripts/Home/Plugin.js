id = {
    checkPlugin: 0,
    getCertInfo: 1,
    signPDF: 2,
    chooseFile: 3,
    signXML: 4,
    signPdfAndXml: 5
};
// variables
var ports = [4000, 4001, 4002, 4003];
//var ports = [6696, 6697, 6698, 6699];
var portIndex = 0;
var webSocket;
var cmcPluginCheckingCallback;
var pluginStatus = 0;
window.addEventListener("beforeunload", function (e) {
    pluginStatus = 0;
});

// get function name
function getFuncName(jsCallback) {
    if ((navigator.userAgent.indexOf("MSIE") != -1) || (!!document.documentMode == true))
        return jsCallback.toString().match(/^function\s*([^\s(]+)/)[1];
    else
        return jsCallback.name;
}
// cmc plugin object
var Plugin = {
    connect: function (port, data) {
        if (pluginStatus !== 1) {
            webSocket = new WebSocket("wss://localhost:" + port + "/plugin");
            timer = setTimeout(function () {
                //console.log("timeout");
                var s = webSocket;
                webSocket = null;
                s.close();
                portIndex++;
                Plugin.tryConnect(data);
            }, 2000);
            // opened
            webSocket.onopen = function () {
                pluginStatus = 1;
                //console.log("Opened");
                clearTimeout(timer); // clear 	            
                webSocket.send(data);
            }
            // closed
            webSocket.onclose = function () {
                //console.log("Closed");
                pluginStatus = 0;
            }
            // message
            webSocket.onmessage = function (message) {
                // handle data	     
                var result = message.data;
                var split = result.split("*");
                var test = JSON.parse(split[0]);
                window[split[1]](split[0]);
            }
            // error
            webSocket.onerror = function () {
                //console.log("Error");
                pluginStatus = 0;
            }
        }
        else {
            // send message
            webSocket.send(data);
        }
    },
    // thu ket noi lai bang cong khac
    tryConnect: function (data) {
        if (portIndex < ports.length) {
            this.connect(ports[portIndex], data);
        }
        else {
            //console.log("Failed to connect plugin");
            cmcPluginCheckingCallback("-1");
        }
    },
    // gui du lieu xuong plugin
    sendDataToPlugin: function (data) {
        var jsData = "";
        jsData += JSON.stringify(data);
        this.connect(ports[0], jsData);
    },
    // Ham ky so: ho tro ky pdf
    signPDF: function (data, jsCallback) {
        var dataInfo = {};
        dataInfo.id = id.signPDF;
        dataInfo.data = data;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    },
    // Ham ky so: ho tro ky xml
    signXML: function (data, uri, jsCallback) {
        var dataInfo = {};
        dataInfo.id = id.signXML;
        dataInfo.data = data;
        dataInfo.uri = uri;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    },
    signPdfAndXml: function (data, jsCallback) {
        var dataInfo = {};
        dataInfo.id = id.signPdfAndXml;
        dataInfo.data = data;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    },
    // ham lay thong tin chung thu so
    getCertInfo: function (jsCallback) {
        var dataInfo = {};
        dataInfo.id = id.getCertInfo;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    },
    // convert file to base64
    chooseFile: function (jsCallback) {
        var dataInfo = {};
        dataInfo.id = id.chooseFile;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    },
    // ham kiem tra plugin 
    checkPlugin: function (jsCallback) {
        cmcPluginCheckingCallback = jsCallback;
        var dataInfo = {};
        dataInfo.id = id.checkPlugin;
        dataInfo.callback = getFuncName(jsCallback);
        return this.sendDataToPlugin(dataInfo);
    }
}