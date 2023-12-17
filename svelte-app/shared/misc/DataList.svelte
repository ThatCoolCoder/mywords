<script>
    export let itemsWritable;
    export let onItemEdit;
    export let onItemUpdated;
    export let onItemDeleted;

    let editingItems = [];

    export function edit(item) {
        editingItems.push(item);
        onItemEdit(item);
    }

    export function del(item) {
        nevermind();
        itemsWritable.update(x => x.filter(i => i != item));
        if (onItemDeleted) onItemDeleted(item);
    }

    export function nevermind(item) {
        editingItems = editingItems.filter(x => x != item);
    }

    export function update(item) {
        nevermind();
        // itemsWritable.update(x => x.filter(i => i != item));
        if (onItemUpdated) onItemUpdated(item);
    }
</script>

{#each $itemsWritable as item}
    <slot {item} {edit} {del} {nevermind} {update} editing={editingItems.includes(item)} />
{:else}
    <span>No items in list</span>
{/each}