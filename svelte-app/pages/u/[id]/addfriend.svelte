<script>
    import { onMount } from "svelte";

    export let id;

    onMount(() => {
        console.log(id);
        // console.log($param.token);
    })
</script>

<h1>hehe</h1>