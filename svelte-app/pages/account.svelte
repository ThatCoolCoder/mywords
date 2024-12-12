<script>
    import { getContext, onMount } from 'svelte';
    import { fly, fade } from 'svelte/transition';
    import { get } from 'svelte/store';

    import api from 'services/api';
    import { notBlank } from 'shared/misc/validation';

    const user = getContext('user');
    user.subscribe(undo);

    let givenName;
    let familyName;
    let changed = false;

    async function undo() {
        let u;
        if ((u = get(user)) == null) return;
        setTimeout(() => {
            givenName = u.givenName;
            familyName = u.familyName;
        }); // stupid routify or something means we get cannot set before init without this fake settimeout

        changed = false;
    }

    async function save(event) {
        event.preventDefault();

        var u = get(user);
        u.givenName = givenName;
        u.familyName = familyName;
        user.set(u);
        await api.put('users/me', u, 'Failed updating account information');
    }
</script>

<title>Account | MyWords</title>
<h2>Your Account</h2>
<hr />

<form style="max-width: 400px" on:submit={save}>
    <div class="row mb-1">
        <div class="col-6">
            <label for="givenName">First name</label>
        </div>
        <div class="col-6">
            <input bind:value={givenName} class="mw-100 form-control" id="givenName" required autocomplete="off" on:input={e => changed = notBlank(e) }/>
        </div>
    </div>

    <div class="row mb-1">
        <div class="col-6">
            <label for="FamilyName">Last name</label>
        </div>
        <div class="col-6">
            <input bind:value={familyName} class="mw-100 form-control" id="familyName" required autocomplete="off" on:input={e => changed = notBlank(e) }/>
        </div>
    </div>

    {#if (changed)}
        <div class="row" in:fly={{y:20}}>
            <div class="col-12 d-flex justify-content-center gap-2">
                <button class="btn btn-secondary" type="button" on:click={undo}>Undo</button>
                <button class="btn btn-primary">Save changes</button>
            </div>
        </div>
    {/if}
</form>