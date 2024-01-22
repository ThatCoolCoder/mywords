<script>
    import { writable, get } from "svelte/store";
    import LabelBadge from "shared/LabelBadge.svelte";

    export let labels = writable([]); // list of label id, mirrors what is in the term object
    $: selectedLabels.set(Object.fromEntries($labels.map(l => [l, true])));

    export let availableLabels = writable([]);
    let sortedAvailableLabels;
    $: sortedAvailableLabels = $availableLabels.toSorted((a, b) => a.name.compareTo(b.name));

    export let selectedLabels = writable({}); // dict of label id to selected or not
    // it feels like this should generate an infinite loop as two reacive statements depend on each other, but it works fine
    $: labels.set(Object.entries($selectedLabels).filter(x => x[1]).map(x => Number(x[0])));
</script>

<div class="d-flex flex-column gap-2">
    <h4>Select labels</h4>
    {#each sortedAvailableLabels as label}
        <div class="form-check">
            <label class="form-check-label" for="flexCheckDefault">
            <input bind:checked={$selectedLabels[label.id]} class="form-check-input" type="checkbox" name="check">
            <LabelBadge {label} />
        </div>
    {:else}
        <p>This collection has no labels - go to its homepage to add some </p>
        <!-- todo: this needs to be a link to homepage -->
    {/each}
</div>