<script>
    import { get } from "svelte/store";
    import api from 'services/api';

    export let label;
    let editing = false;

    let name = '';
    let color = '';

    export function edit() {
        editing = true;
        name = get(label).name;
        color = get(label).color;
    }

    export function save() {
        editing = false;
        label.update(x => {
            x.name = name;
            x.color = color;
            return x;
        });
        // todo: no endpoint for this
    }

    export function discard() {
        editing = false;
    }

    export function del() {
        // uuh perhaps we need proper data grid component
    }
</script>

<div class="bs-card d-flex align-items-center gap-3">
    {#if editing}
        <input bind:value={name}  placeholder="name" />
        <input type="color" bind:value={name}/>
        <div class="vr"></div> 
        
        <button class="btn btn-sm p-0" aria-label="edit" on:click={discard}><i class="bi-arrow-counterclockwise"/></button>
        <button class="btn btn-sm p-0" aria-label="delete" on:click={del}><i class="bi-trash3"/></button>
        <button class="btn btn-sm p-0" aria-label="edit" on:click={save}><i class="bi-check2"/></button>
    {:else}
        <p style={`background-color: ${$label.color}`}>{$label.name}</p>
        <button class="btn" aria-label="edit" on:click={edit}><i class="bi-pencil"/></button>
    {/if}
</div>