<script>
    import { get, writable } from "svelte/store";

    import api from "services/api";
    
    import DataList from "shared/misc/DataList.svelte";
    import StandardEditorButtons from "shared/misc/StandardEditorButtons.svelte";
    import TermCard from "shared/TermCard.svelte";

    export let termsWritable;
    export let termSetId;
    export let termList = null; // as in which of the user-lists it's in
    export let showTermLists = false;
    export let syncWithApi = true;
    let editData;
    let dataList;

    function onItemEdit(term) {
        return {
            id: term.id,
            termSetId: term.termSetId,
            value: term.value,
            definition: term.definition,
            notes: term.notes,
            termList: term.termList
        };
    }

    function onItemUpdate(term, editData) {
        if (editData.name.trim().length == 0) return false;

        

        if (term.id === undefined) api.post(`terms/`, term);
        else api.put(`terms/${term.id}`, term);
    }

    function onTermDeleted(term) {
        termsWritable.update(terms => terms.deleteItem(term));
    }
</script>

<div class="d-flex flex-column gap-2">
    {#each $termsWritable.filter(x => termList === null || x.termList === termList) as term}
        <TermCard {term} showTermList={showTermLists} {syncWithApi} onDeleted={onTermDeleted}/>
    {:else}
        <p class="lead small">No terms yet!</p>
    {/each}
</div>