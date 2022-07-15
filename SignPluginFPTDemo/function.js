var hSession = "";
var domain = "https://127.0.0.1:14408/";
var signAttached = 0;
var checkOCSP = 0;
var algDigest = "SHA_1"; //MD5|SHA_1|SHA_224|SHA_256|SHA_384|SHA_512
var signLevel = "CADES-BES"; //CADES-BES|CADES-T|CADES-C|CADES-X1|CADES-X2|CADES-XL1|CADES-XL2
var reqDigest = 1;
//var urlTimestamp = "http://time.certum.pl";
var urlTimestamp = "http://tsa.lca.la/tsa/request";
//PDF only
var signerTitle = "KÝ BỞI:";
var signTimeTitle = "KÝ NGÀY:";
var invisible = 0;
//XML only
var xmlDSig = 0;
var tagXML = "*";
var xades_Version = "XADES_v1_4_1";
var xades_Form = "XADES_T";
var process = false;
var LibList_MACOS = ["fptca_v3.dylib", "fptca_v4.dylib"];
var LibList_WIN = ["fptca_v3.dll", "fptca_v4.dll"];

var signplugin_installer = "TrustCASignPlugin.exe";
var Base64 = {
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
    encode: function(e) {
        var t = "";
        var n, r, i, s, o, u, a;
        var f = 0;
        e = Base64._utf8_encode(e);
        while (f < e.length) {
            n = e.charCodeAt(f++);
            r = e.charCodeAt(f++);
            i = e.charCodeAt(f++);
            s = n >> 2;
            o = (n & 3) << 4 | r >> 4;
            u = (r & 15) << 2 | i >> 6;
            a = i & 63;
            if (isNaN(r)) { u = a = 64 } else if (isNaN(i)) { a = 64 }
            t = t + this._keyStr.charAt(s) + this._keyStr.charAt(o) + this._keyStr.charAt(u) + this._keyStr.charAt(a)
        }
        return t
    },
    decode: function(e) {
        var t = "";
        var n, r, i;
        var s, o, u, a;
        var f = 0;
        e = e.replace(/[^A-Za-z0-9+/=]/g, "");
        while (f < e.length) {
            s = this._keyStr.indexOf(e.charAt(f++));
            o = this._keyStr.indexOf(e.charAt(f++));
            u = this._keyStr.indexOf(e.charAt(f++));
            a = this._keyStr.indexOf(e.charAt(f++));
            n = s << 2 | o >> 4;
            r = (o & 15) << 4 | u >> 2;
            i = (u & 3) << 6 | a;
            t = t + String.fromCharCode(n);
            if (u != 64) { t = t + String.fromCharCode(r) }
            if (a != 64) { t = t + String.fromCharCode(i) }
        }
        t = Base64._utf8_decode(t);
        return t
    },
    _utf8_encode: function(e) {
        e = e.replace(/rn/g, "n");
        var t = "";
        for (var n = 0; n < e.length; n++) {
            var r = e.charCodeAt(n);
            if (r < 128) { t += String.fromCharCode(r) } else if (r > 127 && r < 2048) {
                t += String.fromCharCode(r >> 6 | 192);
                t += String.fromCharCode(r & 63 | 128)
            } else {
                t += String.fromCharCode(r >> 12 | 224);
                t += String.fromCharCode(r >> 6 & 63 | 128);
                t += String.fromCharCode(r & 63 | 128)
            }
        }
        return t
    },
    _utf8_decode: function(e) {
        var t = "";
        var n = 0;
        var r = c1 = c2 = 0;
        while (n < e.length) {
            r = e.charCodeAt(n);
            if (r < 128) {
                t += String.fromCharCode(r);
                n++
            } else if (r > 191 && r < 224) {
                c2 = e.charCodeAt(n + 1);
                t += String.fromCharCode((r & 31) << 6 | c2 & 63);
                n += 2
            } else {
                c2 = e.charCodeAt(n + 1);
                c3 = e.charCodeAt(n + 2);
                t += String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63);
                n += 3
            }
        }
        return t
    }
}

function initPlugin() {
    var XML = new XMLWriter();
    XML.BeginNode("root");
    XML.BeginNode("start");
    XML.Attrib("Company", "Mobile-ID Co., Ltd");
    XML.EndNode();
    //Produces: <start Bar="Some Value" />
    XML.BeginNode("one");
    XML.WriteString("The first information M&T");
    XML.EndNode();
    //Produces <one>The first information</one>
    XML.Node("two", "The second information: b->c: a>b");
    XML.EndNode();
    XML.Close();
    document.getElementById('xml_Data').value = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\
<xml>\
<note Id=\"data\">\
<to>Tove</to>\
<from>Jani</from>\
<heading>Reminder</heading>\
<body>Don't forget me this weekend!</body>\
</note>\
</xml>";
    //document.getElementById('xml_Data').value = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"+XML.ToString();

    //=================>>Check OS<<=================
    var OSName = "Unknown";
    if (window.navigator.userAgent.indexOf("Windows NT 6.2") != -1) OSName = "Windows 8";
    if (window.navigator.userAgent.indexOf("Windows NT 6.1") != -1) OSName = "Windows 7";
    if (window.navigator.userAgent.indexOf("Windows NT 6.0") != -1) OSName = "Windows Vista";
    if (window.navigator.userAgent.indexOf("Windows NT 5.1") != -1) OSName = "Windows XP";
    if (window.navigator.userAgent.indexOf("Windows NT 5.0") != -1) OSName = "Windows 2000";
    if (window.navigator.userAgent.indexOf("Mac") != -1) OSName = "Mac/iOS";
    if (window.navigator.userAgent.indexOf("X11") != -1) OSName = "UNIX";
    if (window.navigator.userAgent.indexOf("Linux") != -1) OSName = "Linux";
    //=================>>Check OS<<=================

    var jsonObj = new Object();
    jsonObj.OperationId = 1;
    if (OSName == "Mac/iOS") {
        jsonObj.pkcs11Lib = LibList_MACOS;
    } else if ((OSName == "UNIX") || (OSName == "Linux")) {
        alert("Not Support");
        return;
    } else {
        jsonObj.pkcs11Lib = LibList_WIN;
    }
    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);
            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    hSession = json_res.SessionId;
                    alert("Comunication with SignPlugin OK");
                } else {
                    alert(json_res.ResponseMsg);
                }

            } catch (err) {
                alert("Error: " + err.message);
            }
            if (response == "") {
                alert("Please setup SignPlugin and F5 to try again");
                window.location = './' + signplugin_installer;
                return;
            }
        } else if (httpReq.readyState == 4 && httpReq.status != 200) {
            alert("Please setup SignPlugin and F5 to try again");
            window.location = './' + signplugin_installer;
            return;
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);

}

function getCertifcate() {
    if (process == true)
        return;
    var jsonObj = new Object();
    jsonObj.OperationId = 2;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;

    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    document.getElementById('cert_rawData').value = json_res.certInfo.Base64Encode;
                    document.getElementById('cert_SNB').value = json_res.certInfo.SerialNumber;
                    document.getElementById('cert_CN').value = json_res.certInfo.CommonName;
                    document.getElementById('cert_DN').value = json_res.certInfo.SubjectDN;
                    document.getElementById('cert_Issuer').value = json_res.certInfo.Issuer;
                    document.getElementById('cert_ValidFrom').value = json_res.certInfo.NotBefore;
                    document.getElementById('cert_ValidTo').value = json_res.certInfo.NotAfter;
                    document.getElementById('cert_Thumprint').value = json_res.certInfo.Thumbprint;
                } else {
                    document.getElementById('cert_rawData').value = "";
                    document.getElementById('cert_SNB').value = "";
                    document.getElementById('cert_CN').value = "";
                    document.getElementById('cert_DN').value = "";
                    document.getElementById('cert_Issuer').value = "";
                    document.getElementById('cert_ValidFrom').value = "";
                    document.getElementById('cert_ValidTo').value = "";
                    document.getElementById('cert_Thumprint').value = "";
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function sign() {
    if (process == true)
        return;
    var result_style = document.getElementById('verify_result').style;
    result_style.display = 'none';
    var msg = msg = Base64.encode(document.getElementById('sign_Data').value);
    if (msg == "") {
        alert("Please enter data to be sign");
        return;
    }
    var jsonObj = new Object();
    jsonObj.OperationId = 3;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;
    jsonObj.DataToBeSign = msg;
    jsonObj.reqDigest = reqDigest;
    jsonObj.algDigest = algDigest;
    jsonObj.signAttached = signAttached;
    jsonObj.urlTimestamp = urlTimestamp;
    //jsonObj.sigLevel = signLevel;

    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    document.getElementById('sign_Signature').value = json_res.Signature;
                } else {
                    document.getElementById('sign_Signature').value = "";
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function verify() {
    if (process == true)
        return;
    var msg = Base64.encode(document.getElementById('sign_Data').value);
    var signature = document.getElementById('sign_Signature').value;
    if (signature == "") {
        document.getElementById('verify_Signature').value = "";
        alert("Please enter signature to verify");
        return;
    }
    var jsonObj = new Object();
    jsonObj.OperationId = 4;
    jsonObj.SessionId = hSession;
    jsonObj.signature = signature;
    jsonObj.DataToBeSign = msg;


    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    document.getElementById('verify_DTBS').value = Base64.decode(json_res.DataToBeSign);
                    document.getElementById('SignerInfo_rawData').value = json_res.SingerInfo.Base64Encode;
                    document.getElementById('SignerInfo_SNB').value = json_res.SingerInfo.SerialNumber;
                    document.getElementById('SignerInfo_CN').value = json_res.SingerInfo.CommonName;
                    document.getElementById('SignerInfo_DN').value = json_res.SingerInfo.SubjectDN;
                    document.getElementById('SignerInfo_Issuer').value = json_res.SingerInfo.Issuer;
                    document.getElementById('SignerInfo_ValidFrom').value = json_res.SingerInfo.NotBefore;
                    document.getElementById('SignerInfo_ValidTo').value = json_res.SingerInfo.NotAfter;
                    document.getElementById('SignerInfo_Thumprint').value = json_res.SingerInfo.Thumbprint;
                    var result_style = document.getElementById('verify_result').style;
                    result_style.display = '';
                    alert("Signature is valid");
                } else {
                    var result_style = document.getElementById('verify_result').style;
                    result_style.display = 'none';
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function browseFile() {
    if (process == true)
        return;
    var jsonObj = new Object();
    jsonObj.OperationId = 6;
    jsonObj.SessionId = hSession;
    jsonObj.signAttached = 0;
    jsonObj.openFile = 1;

    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);

    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);
            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    document.getElementById('file_input').value = json_res.PathFile;
                    //save file
                    jsonObj = new Object();
                    jsonObj.OperationId = 6;
                    jsonObj.SessionId = hSession;
                    jsonObj.signAttached = 0;
                    jsonObj.openFile = 0;

                    json_req = JSON.stringify(jsonObj);
                    json_req = window.btoa(json_req);
                    var reqSave;
                    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
                        reqSave = new XMLHttpRequest();
                    } else { // code for IE6, IE5
                        reqSave = new ActiveXObject("Microsoft.XMLHTTP");
                    }
                    reqSave.onreadystatechange = function() {
                        if (reqSave.readyState == 4 && reqSave.status == 200) {
                            response = Base64.decode(reqSave.responseText);
                            json_res = JSON.parse(response);
                            if (json_res.ResponseCode == 0) {
                                document.getElementById('file_signed').value = json_res.PathFile;
                            } else {
                                document.getElementById('file_signed').value = "";
                                alert(json_res.ResponseMsg);
                            }
                            process = false;
                        }
                    }
                    reqSave.open("POST", domain + "process", true);
                    reqSave.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                    reqSave.send("request=" + json_req);
                } else {
                    document.getElementById('file_input').value = "";
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function signFile() {
    if (process == true)
        return;

    var inputFile = document.getElementById('file_input').value;
    var outputFile = document.getElementById('file_signed').value;
    if (inputFile == "") {
        alert("Please browse file to sign");
        return;
    }
    if (outputFile == "") {
        alert("Please choose file to save");
        return;
    }
    var jsonObj = new Object();
    jsonObj.OperationId = 7;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;
    jsonObj.infile = inputFile;
    jsonObj.outfile = outputFile;
    jsonObj.algDigest = algDigest;
    jsonObj.urlTimestamp = urlTimestamp;
    jsonObj.offsetX = 100;
    jsonObj.offsetY = 100;
    jsonObj.sigWidth = 100;
    jsonObj.sigHeight = 100;
    //PDF
    jsonObj.invisible = invisible;
    jsonObj.signerTitle = signerTitle;
    jsonObj.signTimeTitle = signTimeTitle;
    //XML
    jsonObj.tagXML = tagXML;
    jsonObj.XMLDSig = xmlDSig;
    jsonObj.xades_Version = xades_Version;
    jsonObj.xades_Form = xades_Form;

    var json_req = JSON.stringify(jsonObj);
    //json_req = window.btoa(json_req);
    json_req = Base64.encode(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    alert("Successfully. Result: " + json_res.PathFile);
                } else {
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function signXMLData() {
    if (process == true)
        return;

    var xmlData = document.getElementById('xml_Data').value;
    if (xmlData == "") {
        alert("Please enter xmlData to sign");
        return;
    }

    var jsonObj = new Object();
    jsonObj.OperationId = 8;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;
    jsonObj.algDigest = algDigest;
    jsonObj.urlTimestamp = urlTimestamp;
    //XML
    jsonObj.DataToBeSign = Base64.encode(xmlData);
    //jsonObj.DataToBeSign = window.btoa(xmlData);
    jsonObj.tagXML = tagXML;
    jsonObj.XMLDSig = xmlDSig;
    jsonObj.xades_Version = xades_Version;
    jsonObj.xades_Form = xades_Form;

    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                if (json_res.ResponseCode == 0) {
                    document.getElementById('xml_Signature').value = Base64.decode(json_res.Base64Result);
                } else {
                    document.getElementById('xml_Signature').value = "";
                    alert(json_res.ResponseMsg);
                }
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function DCSigner() {
    if (process == true)
        return;
    var msg = Base64.encode(document.getElementById('DCSigner_Data').value);
    if (msg == "") {
        alert("Please enter data to be sign");
        return;
    }
    var jsonObj = new Object();
    jsonObj.OperationId = 5;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;
    jsonObj.DataToBeSign = msg;
    jsonObj.reqDigest = reqDigest;
    jsonObj.algDigest = algDigest;

    var json_req = JSON.stringify(jsonObj);
    json_req = window.btoa(json_req);
    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    httpReq.onreadystatechange = function() {
        if (httpReq.readyState == 4 && httpReq.status == 200) {
            response = Base64.decode(httpReq.responseText);

            process = false;
            try {
                var json_res = JSON.parse(response);
                document.getElementById('DCSigner_result').value = response;
            } catch (err) {
                alert("Error: " + err.message);
            }
        }
    }
    httpReq.open("POST", domain + "process", true);
    httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    httpReq.send("request=" + json_req);
}

function sign_loop() {
    var count = 2; //demo for sign multi request
    var results = [];
    if (process == true)
        return;
    var result_style = document.getElementById('verify_result').style;
    result_style.display = 'none';
    var msg = Base64.encode(document.getElementById('sign_Data').value);
    if (msg == "") {
        alert("Please enter data to be sign");
        return;
    }
    var jsonObj = new Object();
    jsonObj.OperationId = 3;
    jsonObj.SessionId = hSession;
    jsonObj.checkOCSP = checkOCSP;
    jsonObj.DataToBeSign = msg;
    jsonObj.reqDigest = reqDigest;
    jsonObj.algDigest = algDigest;
    jsonObj.signAttached = signAttached;
    jsonObj.urlTimestamp = urlTimestamp;
    jsonObj.sigLevel = signLevel;

    var json_req = JSON.stringify(jsonObj);
    //json_req = Base64.encode(json_req);
    json_req = window.btoa(json_req);


    var httpReq;
    var response = "";
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        httpReq = new XMLHttpRequest();
    } else { // code for IE6, IE5
        httpReq = new ActiveXObject("Microsoft.XMLHTTP");
    }
    (function loop(i, length) {
        if (i >= length) {
            if (results.length) {
                document.getElementById('sign_Signature').value = JSON.stringify(results);
            }
            process = false;
            return;
        }

        httpReq.onreadystatechange = function() {
            if (httpReq.readyState == 4 && httpReq.status == 200) {
                response = Base64.decode(httpReq.responseText);

                try {
                    var json_res = JSON.parse(response);
                    results.push(json_res);
                    console.log('-->' + i + ' response: ' + response);
                    if (json_res.ResponseCode == 0) {
                        document.getElementById('sign_Signature').value = json_res.Signature;
                    } else {
                        document.getElementById('sign_Signature').value = "";
                        alert(json_res.ResponseMsg);
                    }
                    loop(i + 1, length);
                } catch (err) {
                    alert("Error: " + err.message);
                    process = false;
                    return;
                }
            } else {
                process = false;
                return;
            }
        }
        httpReq.open("POST", domain + "process", true);
        httpReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        httpReq.send("request=" + json_req);

    })(0, count);
}