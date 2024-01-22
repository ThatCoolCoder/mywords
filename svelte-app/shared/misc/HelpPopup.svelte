<script>
    // extremely primitive help thingo. Might move it to a little thing that just pops from the button, might add a more heavy-duty version with proper markdown support
    export let topic;
    export let text;
</script>

{#if topic}
    <h3>{topic}</h3>
{/if}
{#each text.split('\n') as line}
    <p>{line}</p>
{/each}