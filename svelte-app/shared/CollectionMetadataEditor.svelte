<script>
    import { getContext } from "svelte";
    import { get } from "svelte/store";
    import api from "services/api";
    import { fly } from "svelte/transition";
    import { navigate } from "pages/utils";
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
        let collection = get(collectionWritable);
        api.safe.put(`collections/${collection.id}`, collection, 'Failed saving collection changes');
    }
    
    function cancel() {
        close();
        let collection = get(collectionWritable);
        name = collection.name;
        description = collection.description;
    }

    async function tryDelete() {
        if (confirm("Are you sure that you want to delete this collection? This cannot be undone!")) {
            let collection = get(collectionWritable);
            await api.safe.delete_(`collections/${collection.id}`);
            close();
            navigate("/collections");
        }
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
        <input bind:value={name} class="w-100" id="collectionName" on:input={() => changed = true} />
    </div>
</div>

<div class="row mb-1">
    <div class="col-3">
        <label for="collectionDescription">Description</label>
    </div>
    <div class="col-9">
        <textarea bind:value={description} class="w-100" style="height: 10em" id="collectionDescription" on:input={() => changed = true} />
    </div>
</div>

<div class="row d-flex justify-content-between">
    <!-- Fake button to make layout center properly -->
    <div class="col-auto">
        <button class="btn btn-outline-danger invisible">
            <i class="bi-trash invisible"></i> Delete
        </button>
    </div>
    {#if changed}
        <div class="col-auto" in:fly={{y:20}}>
            <button class="btn btn-outline-secondary me-2" on:click={cancel}>Cancel</button>
            <button class="btn btn-primary" on:click={saveChanges}>Save changes</button>
        </div>
    {:else}
        <div class="col-auto">
            <button class="btn invisible">a</button>
            <button class="btn invisible">b</button>
        </div>
    {/if}
    <div class="col-auto">
        <button class="btn btn-outline-danger" on:click={tryDelete}>
            <i class="bi-trash"></i> Delete
        </button>
    </div>
</div>