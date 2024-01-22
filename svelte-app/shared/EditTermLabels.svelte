<script>
    import { writable, get } from "svelte/store";

    export let labels = []; // list of label id, mirrors what is in the term object
    export let availableLabels = writable([]);
    export let selectedLabels = writable({}); // dict of label id to selected

    selectedLabels.subscribe(newVal => {
        labels = Object.entries(newVal).filter(x => x[1]).map(x => Number(x[0]));
        console.log(labels);
    })
</script>

<div class="d-flex flex-column gap-2">
    <h4>Select labels</h4>
    {#each $availableLabels as label}
        <div class="form-check">
            <label class="form-check-label" for="flexCheckDefault">
            <input bind:checked={$selectedLabels[label.id]} class="form-check-input" type="checkbox" name="check">
                {label.name}
            </label>
        </div>
    {:else}
        <p>This collection has no labels - go to its homepage to add some </p>
        <!-- todo: this needs to be a link to homepage -->
    {/each}
</div>