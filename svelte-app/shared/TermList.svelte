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

    function onItemDelete(term) {
        if (term.id !== undefined) api.post(`terms/${term.id}/delete`);
    }

    // function create() {
    //     var term = {id: undefined, termSetId: termSetId, value: '', definition: '', labels: [], termList: termList};
    //     termsWritable.update(x => x.pushed(term));
    //     dataList.edit(term);
    // }
</script>

<div class="d-flex flex-column">
    {#each $termsWritable.filter(x => termList === null || x.termList === termList) as term }
        <TermCard {term} showTermList={showTermLists}/>
    {:else}
        <p class="lead small">No terms yet!</p>
    {/each}
</div>

<!-- <div class="container">
    <DataList itemsWritable={termsWritable} bind:editData={editData} bind:this={dataList}
        {onItemEdit} {onItemUpdate} {onItemDelete}
        let:editing let:item={term} let:idx
        let:actions>
        <div class="row">
            {#if editing}
                <p>haha nothing to edit</p>
                <StandardEditorButtons {actions} item={term} />
            {:else}
            {/if}
        </div>
    </DataList>

</div> -->