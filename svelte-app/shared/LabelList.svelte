<script>
    import { get, writable } from "svelte/store";

    import api from "services/api";
    import DataList from "shared/misc/DataList.svelte";

    export let labels;
    export let termSetId;
    let editData;
    let dataList;

    function onItemEdit(label) {
        return {name: label.name, color: label.color};
    }

    function onItemUpdate(label, editData) {
        label.name = editData.name;
        label.color = editData.color;

        if (label.id === undefined) api.post(`labels/`, label);
        else api.put(`labels/${label.id}`, label);
    }

    function onItemDelete(label) {
        if (label.id !== undefined) api.post(`labels/${label.id}/delete`);
    }

    function create() {
        var l = {id: undefined, termSetId: termSetId, name: '', color: '#ffaaaa'};
        labels.update(x => x.pushed(l));
        dataList.edit(l);
    }
</script>

<div class="d-flex flex-column align-items-start">
<DataList itemsWritable={labels} bind:editData={editData} bind:this={dataList}
    {onItemEdit} {onItemUpdate} {onItemDelete}
    let:editing let:item={label} let:idx
    let:actions>
    {#if editing}
        <div class="d-flex align-items-center gap-2">
            <input bind:value={$editData[idx].name} placeholder="name" />
            <input type="color" bind:value={$editData[idx].color}/>
            <div class="vr"></div> 

            <button class="btn btn-sm p-0" aria-label="edit" on:click={() => actions.cancelEdit(label)}><i class="bi-arrow-counterclockwise"/></button>
            <button class="btn btn-sm p-0" aria-label="delete" on:click={() => actions.del(label)}><i class="bi-trash3"/></button>
            <button class="btn btn-sm p-0" aria-label="save" on:click={() => actions.update(label)}><i class="bi-check2"/></button>
        </div>
    {:else}
        <div class="d-flex align-items-center gap-2 px-2 py-0 rounded" style={`background-color: ${label.color}`} >
            <span>{label.name}</span>
            <a class="btn px-0" role="button" aria-label="edit" on:click={() => actions.edit(label)}><i class="bi-pencil" /></a>
        </div>
    {/if}
</DataList>
</div>


<button class="btn" aria-label="add label" on:click={create}><i class="bi-plus-lg"/></button>