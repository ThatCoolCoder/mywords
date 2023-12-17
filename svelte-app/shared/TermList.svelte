<script>
    export let title = 'Terms';
    export let terms = [];
</script>

<div class="d-flex flex-column align-items-start">
    <h4>{{title}}</h4>
    {#each $terms as term}
        {#if idx == editIdx}
            <div class="d-flex align-items-center gap-2">
                <!-- <input bind:value={name}  placeholder="name" /> -->
                <!-- <input type="color" bind:value={color}/> -->
                <div class="vr"></div> 

                <button class="btn btn-sm p-0" aria-label="edit" on:click={closeEdit}><i class="bi-arrow-counterclockwise"/></button>
                <button class="btn btn-sm p-0" aria-label="delete" on:click={del}><i class="bi-trash3"/></button>
                <button class="btn btn-sm p-0" aria-label="edit" on:click={save}><i class="bi-check2"/></button>
            </div>
        {:else}
            <div class="d-flex align-items-center gap-2 px-2 py-0 rounded" style={`background-color: ${label.color}`} >
                <span>{label.name}</span>
                <a class="btn px-0" role="button" aria-label="edit" on:click={() => edit(idx)}><i class="bi-pencil" /></a>
            </div>
        {/if}
    {:else}
        <span>No terms</span>
    {/each}
</div>