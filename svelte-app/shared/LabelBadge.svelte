<script>
    export let label;
    export let editable = false;

    export let onEditPressed =  function(label) {
        // override for functionality
    }
</script>

<span class="badge rounded-pill" style={`background-color: ${label.color}`} >
    {label.name}
    {#if editable}
        <button class="btn p-0 badge-btn" aria-label="edit" on:click={() => onEditPressed(label)}><i class="bi-pencil" /></button>
    {/if}
</span>