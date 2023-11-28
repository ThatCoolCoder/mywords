// Maybe you will say that a file like this is bad practice but basically this is just global imports for everything that I want easily accessible.

export function navigate(url) {
    // if we want to support serving from a subdir, can add system for remapping absolute paths here
    history.pushState({}, null, url);
}

export function navigateBackend(url) {
    document.location.href = url;
}

export default {
    navigate,
    navigateBackend
};