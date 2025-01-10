<script>
    import api from "services/api";
    import { onMount } from "svelte";
    import { writable } from "svelte/store";

    export let username;
    let user;
    
    onMount(async () => {
        user = writable(await api.getJson())
    })
</script>