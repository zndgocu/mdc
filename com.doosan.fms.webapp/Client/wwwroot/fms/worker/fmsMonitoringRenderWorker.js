const __HUB_SERVER_URL = 'https://localhost:44304/FmsData';
const __HUB_GET_JS_URL = 'https://localhost:44382/lib/microsoft/signalr/dist/browser/signalr.js';
var _window = this || self || window;
let _connection = undefined;
let _auth = undefined;



//#region fake dom
var document = self.document = { parentNode: null, nodeType: 9, toString: function () { return "FakeDocument" } };
var window = self.window = self;
var fakeElement = Object.create(document);
fakeElement.nodeType = 1;
fakeElement.toString = function () { return "FakeElement" };
fakeElement.parentNode = fakeElement.firstChild = fakeElement.lastChild = fakeElement;
fakeElement.ownerDocument = document;

document.head = document.body = fakeElement;
document.ownerDocument = document.documentElement = document;
document.getElementById = document.createElement = function () { return fakeElement; };
document.createDocumentFragment = function () { return this; };
document.getElementsByTagName = document.getElementsByClassName = function () { return [fakeElement]; };
document.getAttribute = document.setAttribute = document.removeChild =
    document.addEventListener = document.removeEventListener =
    function () { return null; };
document.cloneNode = document.appendChild = function () { return this; };
document.appendChild = function (child) { return child; };
//#endregion

onmessage = async function (args) {
    if (args.data.request == 'startWorker') {
        var sendData = {
            request: 'startWorker',
            data: false,
        }
        _auth = args.data.data.auth;

        var result = await startWorker(__HUB_GET_JS_URL, __HUB_SERVER_URL);
    }
    else {
        console.log('asdf');
    }
}

async function startPooling() {

}

async function startWorker(hubJsUrl, hubUrl) {
    try {
        importScripts(hubJsUrl);
        //나중에 cors 확인하자
        _connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
                accessTokenFactory: () => {
                    return this._auth;
                }
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();

        _connection.onclose(async () => {
            await OnClose();
        });

        _connection.on("HubCliFuncReqeustMdcInitData", (message) => {
            HubCliFuncReqeustMdcInitData(message);
        });
        _connection.on("HubCliFuncBroadCastMdc", (message) => {
            HubCliFuncBroadCastMdc(message);
        });
        

        await StartHubClient();
    }
    catch (e) {
        console.log(e);
        return false;
    }
    return true;
}

let _connected = false;
let _receiveFirst = false;

async function StartHubClient() {
    try {
        await _connection.start();
        _connected = true;
        await HubRequestInitData();
    } catch (err) {
        console.log(err);
        setTimeout(StartHubClient, 3000);
    }
};

async function OnClose() {
    await StartHubClient();
}

async function HubRequestInitData() {
    try {
        if (_connected == false || _connection == undefined || _connection == null || _receiveFirst == true) return;
        await _connection.invoke("HubSrvFuncReqeustMdcInitData", "aa");
        setTimeout(HubRequestInitData, 3000);
    } catch (err) {
        console.log(err);
        setTimeout(HubRequestInitData, 3000);
    }
}

function HubCliFuncReqeustMdcInitData(message) {
    _receiveFirst = true;
    var send = {
        request: 'HubCliFuncReqeustMdcInitData',
        data: message.message
    }
    postMessage(send);
}

function HubCliFuncBroadCastMdc(message) {
    if (_receiveFirst == true) {
        var send = {
            request: 'HubCliFuncBroadCastMdc',
            data: message
        }
        postMessage(send);
    }
}

//onmessage = async function (request, signalRUrl) {
//    if (request == 'RequestPrefabData') {
//        signalRHubStart(signalRUrl);
//        return;
//    }
//}






//var connection = undefined;
//
//onmessage = async function (request, signalRUrl) {
//    if (request == 'start') {
//        signalRHubStart(signalRUrl);
//        return;
//    }
//}
//
//function signalRHubStart(signalRUrl) {
//    connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
//    connection.on("ReceiveMessage", function (message) {
//        signalRHubOnReceiveMessage(message);
//    });
//    connection.start();
//}
//
//function signalRHubOnReceiveMessage(message) {
//    console.log(message);
//}