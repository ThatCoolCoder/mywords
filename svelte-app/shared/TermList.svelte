<script>
    import { writable } from "svelte/store";

    import api from "services/api";

    import DataList from "shared/misc/DataList.svelte";
    import StandardEditorButtons from "shared/misc/StandardEditorButtons.svelte";

    export let title = 'Terms';
    export let terms = writable([]);

    let editData;
    let dataList;

    function onItemEdit(term) {
        return {value: term.value, definition: term.definition, description: term.description};
    }

    function onItemUpdate(term) {

    }

    function onItemDelete(term) {
        if (label.id !== undefined) api.post(`terms/${term.id}/delete`);
    }

</script>

<p>{title}</p>
<DataList itemsWritable={terms} bind:editData={editData} bind:this={dataList}
    {onItemEdit} {onItemUpdate} {onItemDelete}
    let:editing let:item={label} let:idx
    let:actions>

</DataList>