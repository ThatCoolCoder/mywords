import errorService from "./errorService";

// A note on error handling:
// The regular functions will do error handling for only stuff like JSON parsing failure
// Network errors will be left to the consumer to handle.
// the safe versions meanwhile handle all errors, but you have no control to do custom things on a certain error

export class BadResponse extends Error {
    constructor(response) {
        super("Bad response received from server");
        this.response = response;
    }
}

export async function get(url) {
    const response = await fetch('/api/' + url, {
        credentials: 'include',
    });

    if (! response.ok) throw BadResponse(response);

    return response;
}

export async function getJson(url, failMessage='Failed fetching data from server') {
    const response = await get(url);

    try {
        return await response.json();
    }
    catch (e) {
        if (! e instanceof SyntaxError) throw e;

        errorService.display(failMessage, 'Failed parsing response json');
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
        return false;
    }

    if (! response.ok) throw BadResponse(response);

    return response;
}

export async function postJson(url, data, failMessage='Failed sending data to server', overrideMethod=null) {
    let response = await post(url, data, failMessage, overrideMethod);
    try {
        return await response.json();
    }
    catch (e) {
        if (! e instanceof SyntaxError) throw e;

        errorService.display(failMessage, 'Failed parsing server response');
        return false;
    }
}

export async function put(url, data, failMessage='Failed sending data to server') {
    // Is actually the same code except for method so might as well be efficient but also provide a nice interface
    return await post(url, data, failMessage, 'put');
}

export async function putJson(url, data, failMessage='Failed sending data to server') {
    return await postJson(url, data, failMessage, 'put');
}

export async function delete_(url) {
    let response = await fetch('/api/' + url, {
        credentials: 'include',
        method: 'delete'
    });

    if (! response.ok) throw BadResponse(response);
}



async function formatError(e) {
    if (e instanceof BadResponse) {
        return `Network error: ${e.response.statusText}. ${await e.response.text()}` 
    }
    else {
        return e.message;
    }
}

// Stuff that's already has handlers for http errors, for when it can't go wrong

export const safe = {
    async get(url, failMessage='Failed fetching data from server') {
        try {
            return await get(url);
        }
        catch (e) {
            errorService.display(failMessage, await formatError(e));
            return false;
        }
    },

    async getJson(url, failMessage='Failed fetching data from server') {
        try {
            return await getJson(url, failMessage);
        }
        catch (e) {
            errorService.display(failMessage, await formatError(e));
            return false;
        }
    },

    async post(url, data, failMessage='Failed sending data to server', overrideMethod=null) {
        try {
            return await post(url, data, failMessage, overrideMethod);
        }
        catch (e) {
            errorService.display(failMessage, await formatError(e));
            return false;
        }
    },

    async postJson(url, data, failMessage='Failed sending data to server', overrideMethod=null) {
        try {
            return await postJson(url, data, failMessage, overrideMethod);
        }
        catch (e) {
            errorService.display(failMessage, await formatError(e));
            return false;
        }
    },
    
    async put(url, data, failMessage='Failed sending data to server') {
        return await this.post(url, data, failMessage, 'put');
    },
    
    async put(url, data, failMessage='Failed sending data to server') {
        return await this.postJson(url, data, failMessage, 'put');
    },

    async delete_(url, failMessage='Failed deleting resource') {
        try {
            return await delete_(url);
        }
        catch (e) {
            errorService.display(failMessage, await formatError(e));
            return false;
        } 
    }
    
}

export default {
    get,
    getJson,
    post,
    postJson,
    put,
    putJson,
    delete_,

    safe
}