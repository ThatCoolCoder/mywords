<script>
    var isOpen = false;

    export function open() {
        isOpen = true;
    }

    export function close() {
        isOpen = false;
    }
</script>

<div class="footer fixed-bottom p-4 d-flex w-100 error-popup {isOpen ? 'error-popup-open' : 'error-popup-closed'}" aria-hidden={! isOpen}>
    <div class="flex-grow-1">
        <h5><i class="bi-exclamation-triangle"></i> Something went wrong</h5>
        If this error persists please contact support (when we get some)
    </div>
    <button on:click={close}><i class="bi-x-lg"></i></button>
</div>

<style>
    .error-popup {
        background-color: var(--bs-danger-border-subtle);
        transition: transform 0.2s;
    }

    .error-popup-closed {
        transform: translateY(100%);
    }
</style>