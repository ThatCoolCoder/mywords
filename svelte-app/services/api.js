var onError = (message, details) => alert(message + '\n' + details);

async function get() {
    onError('oh no');
}

export default {
    onError,
    get
}