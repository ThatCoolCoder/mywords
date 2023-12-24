<script>
    export let actions;
    export let item;
</script>

<button class="btn btn-sm p-0" aria-label="edit" on:click={() => actions.cancelEdit(item)}><i class="bi-arrow-counterclockwise"/></button>
<button class="btn btn-sm p-0" aria-label="delete" on:click={() => actions.del(item)}><i class="bi-trash3"/></button>
<button class="btn btn-sm p-0" aria-label="save" on:click={() => actions.update(item)}><i class="bi-check2"/></button>