<script>
    import { onMount, setContext } from "svelte";
    import { writable, get } from "svelte/store";

    import api from 'services/api';

    export let id;

    let collectionId;
    let collection;
    let terms = writable([]);
    let labels = makeLabelsStore();
    
    setContext('labels', labels);
    
    let props = writable({});
    
    // We use a custom store here so we can remove labels from terms before they are deleted
    function makeLabelsStore() {
        let inner = writable([]);

        return {
            update(f) {
                let v = f(get(inner));
                removeDeletedLabels(v).then(() => inner.set(v));
            },
            set(v) {
                removeDeletedLabels(v).then(() => inner.set(v));
            },
            subscribe(a, b) {
                return inner.subscribe(a, b);
            }
        };
    };

    async function removeDeletedLabels(newLabels) {
        let labelIds = newLabels.map(x => x.id);
        let someChanged = false;
        for (let term of get(terms)) {
            let oldLabels = term.labels;
            for (let labelId of term.labels) {
                if (! labelIds.includes(labelId)) {
                    term.labels.splice(term.labels.indexOf(labelId), 1);
                }
            }
            if (oldLabels.length != term.labels.length) {
                await api.safe.put(`terms/${id}`, term, 'Failed removing redundant labels from terms');
                someChanged = true;
            }
        }
        if (someChanged) terms.set(terms);
    };

    onMount(async () => {
        collectionId = id;
        collection = writable(await api.getJson(`collections/${id}`, 'Failed fetching collection info'));
        terms.set(await api.safe.getJson(`collections/${id}/terms`, 'Failed loading terms'));
        labels.set(await api.safe.getJson(`collections/${id}/labels`, 'Failed loading labels'));

        await api.safe.post(`collections/${id}/viewed`, 'Failed saving collection viewing stuffs') // push to top of recent

        props.set({collectionId, collection, terms, labels});
    });
</script>

<!-- todo: find a way to key this without recreating all the components, as it makes children hit server twice -->
{#key $props}
    <slot props={$props}/>
{/key}