<script>
    import { get, writable } from "svelte/store";

    import api from "services/api";

    import TermCard from "shared/TermCard.svelte";
    import { WidthMode }from "shared/TermCard.svelte";

    export let termsStore;
    export let baseTermsWritable = null; // You need to provide this if providing terms in a derived store, should be the writable that the store was derived from
    let targetStore;
    $: targetStore = baseTermsWritable ?? termsStore;

    export let termList = null; // as in which of the user-lists it's in, if set it will filter to that list
    export let showTermLists = false;
    export let syncWithApi = true;
    export let widthMode = WidthMode.Full;
    export let dragAndDropEnabled = false;

    function onTermDeleted(term) {
        targetStore.update(terms => terms.deleteItem(term));
    }

    async function onTermDrop(e) {
        e.preventDefault();

        const id = Number(e.dataTransfer.getData("text/plain"));

        let term = null;
        for (let crntTerm of get(targetStore)) {
            if (crntTerm.id == id) {
                term = crntTerm;
                break;
            }
        }
        // todo: raise error if term is still null
        
        dragging.set(false);

        if (term.termList != termList) {
            term.termList = termList;
            targetStore.set(get(targetStore));
            await api.put(`terms/${term.id}`, term, "Failed moving term into list");
        }
    }
    
    function onTermDragOver(e) {
        e.preventDefault();
    }
</script>

<script context="module">
    let dragging = writable(false);
    let draggingFromList = writable(-1);

    function onDragStart(termList) {
        dragging.set(true);
        draggingFromList.set(termList);
    }
</script>

<div class="d-flex flex-column gap-2" on:drop={onTermDrop} on:dragover={onTermDragOver}>
    {#each $termsStore.filter(x => termList === null || x.termList === termList) as term}
        <TermCard {term} showTermList={showTermLists} {syncWithApi} onDeleted={onTermDeleted} {widthMode} {dragAndDropEnabled} on:dragstart={() => onDragStart(term.termList)}/>
    {:else}
        <p class="lead mb-1">No terms yet</p>
    {/each}
    {#if $dragging && $draggingFromList !== termList}
        <!-- give space for it to drag in to -->
        <div class="card" style="height: 150px"></div>
    {/if}
</div>