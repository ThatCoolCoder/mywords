<script>
    export let label;
    export let editable = false;

    export let onEditPressed =  function(label) {
        // override for functionality
    }
</script>

<div class="d-flex align-items-center gap-2 px-2 py-0 rounded" style={`background-color: ${label.color}`} >
    <span>{label.name}</span>
    {#if editable}
        <a class="btn px-0" role="button" aria-label="edit" on:click={() => onEditPressed(label)}><i class="bi-pencil" /></a>
    {/if}
</div>