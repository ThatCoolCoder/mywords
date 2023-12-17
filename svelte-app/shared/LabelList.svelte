<script>
    import { get } from "svelte/store";

    import api from "services/api";

    export let labels;
    export let termSetId;
    let editIdx = -1;

    let name;
    let color;

    function edit(idx) {
        editIdx = idx;
        var label = get(labels)[idx];
        name = label.name;
        color = label.color;
    }

    function closeEdit() {
        name = '';
        color = '';
        editIdx = -1;
    }

    function del() {
        var label = get(labels)[editIdx];
        if (label.id !== undefined) api.post(`labels/${label.id}/delete`);
        closeEdit();
        editIdx = -1;
        labels.update(x => x.delete(editIdx));
    }

    function save() {
        var label = get(labels)[editIdx];

        label.name = name;
        label.color = color;

        if (label.id === undefined) api.post(`labels/`, label);
        else api.put(`labels/${label.id}`, label);

        labels.update(x => x);
        closeEdit();
    }

    function create() {
        var l = {id: undefined, termSetId: termSetId, name: '', color: 'black'};
        editIdx = get(labels).length;
        labels.update(x => x.concat([l]));
    }
</script>

<div class="d-flex flex-column align-items-start">
    {#each $labels as label, idx}
        {#if idx == editIdx}
            <div class="d-flex align-items-center gap-2">
                <input bind:value={name}  placeholder="name" />
                <input type="color" bind:value={color}/>
                <div class="vr"></div> 

                <button class="btn btn-sm p-0" aria-label="edit" on:click={closeEdit}><i class="bi-arrow-counterclockwise"/></button>
                <button class="btn btn-sm p-0" aria-label="delete" on:click={del}><i class="bi-trash3"/></button>
                <button class="btn btn-sm p-0" aria-label="edit" on:click={save}><i class="bi-check2"/></button>
            </div>
        {:else}
            <div class="d-flex align-items-center gap-2 px-2 py-0 rounded" style={`background-color: ${label.color}`} >
                <span>{label.name}</span>
                <a class="btn px-0" role="button" aria-label="edit" on:click={() => edit(idx)}><i class="bi-pencil" /></a>
            </div>
        {/if}
    {:else}
        <span>No labels</span>
    {/each}
</div>

<button class="btn" aria-label="add label" on:click={create}><i class="bi-plus-lg"/></button>