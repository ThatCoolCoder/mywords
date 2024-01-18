<script>
    import { getContext, onMount } from 'svelte';

    import api from 'services/api';

    const { get, set } = getContext('user');
    let user = get();

    let givenName;
    let familyName;

    async function undo() {
        user = get();
        givenName = user.givenName;
        familyName = user.familyName;
    }

    async function save() {
        user = get();
        user.givenName = givenName;
        user.familyName = familyName;
        await api.put('users/me', user, 'Failed updating account information');
    }
</script>

<title>Account | MyWords</title>
<h2>Your Account</h2>
<hr />

<div style="max-width: 400px">
    <div class="row mb-1">
        <div class="col-6">
            <label for="givenName">First name</label>
        </div>
        <div class="col-6">
            <input bind:value={givenName} class="mw-100 form-control" id="givenName" />
        </div>
    </div>

    <div class="row mb-1">
        <div class="col-6">
            <label for="FamilyName">Last name</label>
        </div>
        <div class="col-6">
            <input bind:value={familyName} class="mw-100 form-control" id="familyName" />
        </div>
    </div>

    <div class="row">
        <div class="col-12 d-flex justify-content-center gap-3">
            <button class="btn btn-secondary" on:click={undo}>Undo</button>
            <button class="btn btn-primary" on:click={save}>Save changes</button>
        </div>
    </div>
</div>