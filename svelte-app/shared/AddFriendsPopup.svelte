<script>
    import { onMount } from "svelte";

    import api from "services/api";
    import ApiDependent from "shared/misc/ApiDependent.svelte";

    let link = null;

    onMount(async () => {
        link = writable(await api.getJson('friendships/new/preparelink', 'Failed creating a link'));
    });
</script>

<div class="row mb-3">
    <h4>Add friends</h4>
</div>

<div class="row mb-3">
    <div class="card p-3">
        <p>Search by name</p>
    </div>
</div>

<div class="row">
    <div class="card p-3">
        <p>Use invite link</p>
        <ApiDependent ready={link != null}>

        </ApiDependent>
    </div>
</div>