<script>
    import { getContext } from "svelte";
    import { get } from "svelte/store";
    import api from "services/api";
    import { fly } from "svelte/transition";
    export let collectionWritable;

    let name = '';
    let description = '';
    let changed = false;

    const { close } = getContext('simple-modal');

    collectionWritable.subscribe(value => {
        name = value.name;
        description = value.description;
        changed = false;
    });

    function saveChanges() {
        collectionWritable.update(x => {
            x.name = name;
            x.description = description;
            return x;
        });
        close();
        let ts = get(collectionWritable);
        api.put(`collections/${ts.id}`, ts, 'Failed saving collection changes');
    }
    
    function cancel() {
        close();
        let ts = get(collectionWritable);
        name = ts.name;
        description = ts.description;
    }
</script>

<div class="row mb-3">
    <h4>Collection properties</h4>
</div>

<div class="row mb-1">
    <div class="col-3">
        <label for="collectionName">Name</label>
    </div>
    <div class="col-9">
        <input bind:value={name} class="mw-100" id="collectionName" on:input={() => changed = true} />
    </div>
</div>

<div class="row mb-1">
    <div class="col-3">
        <label for="collectionDescription">Description</label>
    </div>
    <div class="col-9">
        <textarea bind:value={description} class="w-100" id="collectionDescription" on:input={() => changed = true} />
    </div>
</div>

{#if changed}
    <div class="d-flex justify-content-center gap-2" in:fly={{y:20}} >
        <button class="btn btn-secondary" on:click={cancel}>Cancel</button>
        <button class="btn btn-primary" on:click={saveChanges}>Save changes</button>
    </div>
{:else}
<div style="d-flex justify-content-center gap-2">
    <button class="btn invisible">a</button>
    <button class="btn invisible">b</button>
</div>
{/if}