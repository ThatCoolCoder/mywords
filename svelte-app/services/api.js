import errorService from "./errorService";

export async function get(url, failMessage='Failed fetching data from server') {
    const response = await fetch('/api/' + url, {
        credentials: 'include',
    });
    if (! response.ok) {
        errorService.display(failMessage, `Network error: ${response.statusText}. ${await response.text()}`);
        return false;
    }

    try {
        return await response.json();
    }
    catch {
        errorService.display(failMessage, `Error parsing response json`);
        return false;
    }
}

export async function post(url, data, failMessage='Failed sending data to server', overrideMethod=null) {
    let response;
    try {
        response = await fetch('/api/' + url, {
            credentials: 'include',
            method: overrideMethod ?? 'post',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        });
    }
    catch {
        errorService.display(failMessage, 'Failed creating request json');
    }
    if (! response.ok) {
        errorService.display(failMessage, `Network error: ${response.statusText}. ${await response.text()}`);
        return false;
    }
    return response;
}

export async function postJsonResponse(url, data, failMessage='Failed sending data to server', overrideMethod=null) {
    let response = await post(url, data, failMessage, overrideMethod);
    try {
        return await response.json();
    }
    catch
    {
        errorService.display(failMessage, 'Failed parsing server response');
    }
}

export async function put(url, data, failMessage='Failed sending data to server') {
    // Is actually the same code except for method so might as well be efficient but also provide a nice interface
    return await post(url, data, failMessage, 'put');
}

export async function delete_(url, failMessage='Failed deleting resource') {
    try {
        let response = await fetch('/api/' + url, {
            credentials: 'include',
            method: 'delete'
        });

        if (! response.ok) {
            errorService.display(failMessage, `Network error: ${response.statusText}`);
            return false;
        }
    }
    catch {
        errorService.display(failMessage, 'Failed creating request json');
    }
}

export default {
    get,
    post,
    postJsonResponse,
    put,
    delete_
}