<script>
    import api from "services/api";
    import ApiDependent from "shared/misc/ApiDependent.svelte";
    import { onMount } from "svelte";
    import { writable } from "svelte/store";


    export let id;
    let user = writable(null);
    
    onMount(async () => {
        user = writable(await api.getJson(`users/${id}`, "Failed getting user data"));
    });

    async function sendFriendRequest() {
        // await api.
    }
</script>

<h3>{$user?.givenName ?? "lod"} {$user?.familyName ?? "ing"}</h3>
<hr />
<ApiDependent ready={$user != null}>
    <p>MyWords user since {new Date($user.joinDate).toDateString()}</p>
</ApiDependent>

<button class="btn btn-primary" on:click={sendFriendRequest}>Add friend</button>