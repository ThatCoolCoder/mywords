<script>
    import { writable, get } from "svelte/store";

    export let itemsWritable;
    
    export let onItemEdit; // has item passed to it and can return some edit data which is then accessible through editData[itemIdx]
    export let onItemUpdate; // has item and associated editData passed to it, if it returns false then the item is not valid and it won't be updated
    export let onItemDelete; // has item passed to it

    let editingItems = writable([]);
    export let editData = writable([]);

    export function edit(item) {
        var thisItemData = {};
        if (onItemEdit) thisItemData = onItemEdit(item);
        
        editData.update(l => {l[indexOf(item)] = thisItemData; return l; });
        editingItems.update(l => l.pushed(item));
    }

    export function del(item) {
        stopEditingItem(item);
        itemsWritable.update(l => l.deleteItem(item));
        if (onItemDelete) onItemDelete(item);
    }

    function stopEditingItem(item) {
        editData.update(l => l.deleteIdx(indexOf(item)));
        editingItems.update(l => l.deleteItem(item));
    }

    export function cancelEdit(item) {
        stopEditingItem(item);
    }

    export function update(item) {
        var isValid = true;
        if (onItemUpdate) isValid = onItemUpdate(item, $editData[indexOf(item)]) ?? true;
        if (isValid) stopEditingItem(item);
    }

    function indexOf(item) {
        return get(itemsWritable).indexOf(item);
    }
</script>

{#each $itemsWritable as item, idx}
    <slot {item} {idx} editing={$editingItems.includes(item)}
        actions={{edit, cancelEdit, del, update}} />
{:else}
    <span>No items in list</span>
{/each}