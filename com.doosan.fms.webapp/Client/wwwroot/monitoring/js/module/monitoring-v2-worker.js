onmessage = async function (e) {
    try {
        const requests = e.data.map((url) =>
            fetch(url, {
                method: "GET",
            })
        );

        let results = [];
        await Promise.all(requests)
            .then((responses) => {
                const errors = responses.filter((response) => !response.ok);
                if (errors.length > 0) {
                    throw errors.map((response) => Error(response.statusText));
                }
                const json = responses.map((response) => response.json());
                return Promise.all(json);
            })
            .then((data) => {
                results.push(data);
            })
            .catch((errors) => {
                results.push(errors);
            });
        postMessage(results);
    }
    catch (e) {
        postMessage(e);
    }





    // fetch(e.data, {
    //     method: "GET",
    // }).then(response => response.json())
    //     .then(function (responseJson) {
    //         postMessage(responseJson);
    //     })
    //     .catch(function (error) {
    //         postMessage(error)
    //     });
}