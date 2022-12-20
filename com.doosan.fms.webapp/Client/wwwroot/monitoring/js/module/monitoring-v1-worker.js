onmessage = function (e) {
    fetch(e.data, {
        method: "GET",
    })
        //.then((response) => response.data)
        //.then((data) =>
        //    postMessage(data.content)
        //);
        .then((response) =>
            response.text().then(function (text) {
                postMessage(text)
            }));
}