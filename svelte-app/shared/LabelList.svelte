<script>
    import { get, writable } from "svelte/store";

    import api from "services/api";
    
    import DataList from "shared/misc/DataList.svelte";
    import StandardEditorButtons from "shared/misc/StandardEditorButtons.svelte";

    export let labels;
    export let termSetId;
    let editData;
    let dataList;

    function onItemEdit(label) {
        return {name: label.name, color: label.color};
    }

    function onItemUpdate(label, editData) {
        if (editData.name.trim().length == 0) return false;

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

<div class="d-flex flex-row align-items-start gap-2">
<DataList itemsWritable={labels} bind:editData={editData} bind:this={dataList}
    {onItemEdit} {onItemUpdate} {onItemDelete}
    let:editing let:item={label} let:idx
    let:actions>
    {#if editing}
        <div class="d-flex align-items-center gap-2 rounded px-2 py-0 children-no-margin" style="border: 1px solid black">
            <input bind:value={$editData[idx].name} placeholder="name" />
            <input type="color" bind:value={$editData[idx].color}/>
            <div class="vr"></div> 

            <StandardEditorButtons {actions} item={label} />
        </div>
    {:else}
        <div class="d-flex align-items-center gap-2 px-2 py-0 rounded" style={`background-color: ${label.color}`} >
            <span>{label.name}</span>
            <a class="btn px-0" role="button" aria-label="edit" on:click={() => actions.edit(label)}><i class="bi-pencil" /></a>
        </div>
    {/if}
</DataList>
<button class="btn" aria-label="add label" on:click={create}><i class="bi-plus-lg"/></button>
</div>

