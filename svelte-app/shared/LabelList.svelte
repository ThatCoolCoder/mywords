<script>
    import api from "services/api";
    
    import DataList from "shared/misc/DataList.svelte";
    import StandardEditorButtons from "shared/misc/StandardEditorButtons.svelte";
    import LabelBadge from "shared/LabelBadge.svelte";

    export let labelsWritable;
    export let termSetId;
    let editData;
    let dataList;

    function onItemEdit(label) {
        return {name: label.name, color: label.color};
    }

    async function onItemUpdate(label, editData) {
        if (editData.name.trim().length == 0) return false;

        label.name = editData.name;
        label.color = editData.color;

        if (label.id === undefined) label.id = Number(await (await api.post(`labels/`, label)).text());
        else api.put(`labels/${label.id}`, label);
    }

    function onItemDelete(label) {
        if (label.id !== undefined) api.post(`labels/${label.id}/delete`);
    }

    function create() {
        let l = {id: undefined, termSetId: termSetId, name: '', color: '#ffaaaa'};
        labelsWritable.update(x => x.pushed(l));
        dataList.edit(l);
    }
</script>

<div class="d-flex flex-row align-items-center gap-2">
<DataList itemsWritable={labelsWritable} bind:editData={editData} bind:this={dataList}
    {onItemEdit} {onItemUpdate} {onItemDelete}
    let:editing let:item={label} let:idx
    let:actions>
    {#if editing}
        <div class="d-flex align-items-center gap-2 rounded px-2 py-0 children-no-margin" style="border: 1px solid black">
            <input bind:value={$editData.name} placeholder="name" />
            <input type="color" bind:value={$editData.color}/>
            <div class="vr"></div> 

            <StandardEditorButtons {actions} item={label} />
        </div>
    {:else}
        <LabelBadge {label} editable={true} onEditPressed={() => actions.edit(label)} />
    {/if}
</DataList>
<button class="btn mb-0" aria-label="add label" on:click={create}><i class="bi-plus-lg"/></button>
</div>

