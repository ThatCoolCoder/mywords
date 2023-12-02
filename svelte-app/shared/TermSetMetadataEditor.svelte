<script>
    import { getContext } from "svelte";
    import { get } from "svelte/store";
    import api from "services/api";
    export let termSetWritable;

    let name = '';
    let description = '';

    const { close } = getContext('simple-modal');

    termSetWritable.subscribe(value => {
        name = value.name;
        description = value.description;
    });

    function saveChanges() {
        termSetWritable.update(x => {
            x.name = name;
            x.description = description;
            return x;
        });
        close();
        var ts = get(termSetWritable);
        api.put(`termsets/${ts.id}`, ts, 'Failed saving collection changes');
    }
    
    function cancel() {
        var ts = get(termSetWritable);
        name = ts.name;
        description = ts.description;
    }
</script>

<div class="row mb-3">
    <h4>Collection properties</h4>
</div>

<div class="row mb-1">
    <div class="col-3">
        <label for="termSetName">Name</label>
    </div>
    <div class="col-9">
        <input bind:value={name} class="mw-100" id="termSetName" />
    </div>
</div>

<div class="row mb-1">
    <div class="col-3">
        <label for="termSetDescription">Description</label>
    </div>
    <div class="col-9">
        <textarea bind:value={description} class="w-100" id="termSetDescription" />
    </div>
</div>

<div class="d-flex justify-content-center gap-3">
    <button class="btn btn-secondary" on:click={cancel}>Cancel</button>
    <button class="btn btn-primary" on:click={saveChanges}>Save changes</button>
</div>