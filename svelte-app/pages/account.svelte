<script>
    import { getContext, onMount } from 'svelte';
    import { fly, fade } from 'svelte/transition';
    import { get } from 'svelte/store';

    import api from 'services/api';
    import { notBlank } from 'shared/misc/validation';

    import PasswordChange from 'shared/PasswordChange.svelte';
    import AddFriendsPopup from 'shared/AddFriendsPopup.svelte';
    import { navigate } from 'pages/utils';

    const user = getContext('user');
    user.subscribe(undo);

    const { open, close } = getContext('simple-modal');

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
        await api.safe.put('users/me', u, 'Failed updating account information');
    }

    async function openAddFriendsPopup() {
        open(AddFriendsPopup);
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

    <div class="row mb-1">
        <div class="col-6"></div>
        <div class="col-6">
            <button class="btn btn-secondary" type="button" on:click={() => open(PasswordChange)}>Update password</button>
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

<h3 class="mt-3">Friends</h3>
<div class="d-flex flex-column gap-2" style="max-width: 500px">
    <div class="card p-3 text-large-1 text-center" role="button"
        on:click={openAddFriendsPopup} on:keydown={e => e.key == 'Enter' && openAddFriendsPopup()}
        tabindex="0">
        <span>
            <i class="bi-plus-lg me-3"></i>
            Add friends
        </span>
    </div>
    <!-- {#each $friends as friend}
        <div class="card p-3" role="link" 
            on:click={() => navigate(`/`)}>

        </div>
    {/each} -->
</div>