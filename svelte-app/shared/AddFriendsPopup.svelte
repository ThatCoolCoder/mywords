<script>
    import { onMount } from "svelte";
    import { writable } from "svelte/store";
    import { fade } from "svelte/transition";

    import api from "services/api";
    import ApiDependent from "shared/misc/ApiDependent.svelte";

    let link = null;
    let showingCopied = false;

    function createInviteLink() {
        return `${document.location.origin}/u/${$link.senderId}/addfriend`;
    }

    function copyLink() {
        navigator.clipboard.writeText(createInviteLink());
        showingCopied = true;
        setTimeout(() => showingCopied = false, 5000);
    }

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
            <div class="card bg-light p-2" role="button" tabindex="0" on:click={copyLink} on:keypress={e => e.key == 'Enter' && copyLink()}>
                <span class="fake-anchor">{createInviteLink()}</span> <i class="bi-clipboard"></i>
            </div>
        </ApiDependent>
        {#if showingCopied}
            <div class="bg-success rounded-2 p-2 mt-2 text-light" in:fade out:fade>Copied to clipboard</div>
        {/if}
    </div>

</div>

<style>
    .fake-anchor {
        color: blue;
        text-decoration: underline;
    }
</style>