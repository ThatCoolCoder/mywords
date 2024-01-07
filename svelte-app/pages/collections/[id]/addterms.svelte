<script>
    import { onMount, getContext } from 'svelte';
    import { writable } from 'svelte/store';
    import { params } from '@roxi/routify';

    import api from 'services/api.js';
    
    import ApiDependent from 'shared/misc/ApiDependent.svelte';

    let set;
    let terms;
    let labels;
    
    onMount(async () => {

        
    });

    // const { open, close } = getContext('simple-modal');


</script>

<title>Add Terms | MyWords</title>

<!-- <ApiDependent ready={set != null}>
    <h2>Add terms - { $set.name }</h2>
    <fieldset>
        <legend  class="float-none w-auto">Configure terms</legend>
    </fieldset>
    
</ApiDependent> -->