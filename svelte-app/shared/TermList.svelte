<script>
    import api from "services/api";
    
    import TermCard from "shared/TermCard.svelte";
    import { WidthMode }from "shared/TermCard.svelte";

    export let termsWritable;
    export let termList = null; // as in which of the user-lists it's in, if set it will filter to that list
    export let showTermLists = false;
    export let syncWithApi = true;
    export let widthMode = WidthMode.Full;

    function onTermDeleted(term) {
        termsWritable.update(terms => terms.deleteItem(term));
    }
</script>

<div class="d-flex flex-column gap-2">
    {#each $termsWritable.filter(x => termList === null || x.termList === termList) as term}
        <TermCard {term} showTermList={showTermLists} {syncWithApi} onDeleted={onTermDeleted} {widthMode}/>
    {:else}
        <p class="lead">No terms yet</p>
    {/each}
</div>