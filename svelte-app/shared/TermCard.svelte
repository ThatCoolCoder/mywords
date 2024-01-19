

<script>
    import LabelCard from 'shared/LabelCard.svelte';

    export let term = {value: '', definition: '', notes: '', labels: []};
</script>

<div class="card p-3 d-flex flex-row gap-2">
    <div class="flex-grow-1">
        <div class="row">
            <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control" placeholder="Value" bind:value={term.value} />
            </div>
            <div class="col-xs-12 col-md-4 col-xxl-3 form-group">
                <input class="form-control" placeholder="Definition" bind:value={term.definition} />
            </div>
            <!-- Todo: big plus button on the right filling all sub-rows -->
        </div>
        <div class="row">
            <div class="form-group">
                <textarea bind:value={term.notes} class="form-control" style="resize: none" placeholder="Notes"></textarea>
            </div>
        </div>
        <!-- <div class="row d-flex gap-2 justify-content-start">
            <span class="w-auto">
                Labels:
            </span>
            {#each term.labels as label}
                <LabelCard {label} />
            {/each}
            <button class="btn btn-sm w-auto"><i class="bi-pencil"/></button>
        </div> -->
    </div>
    <div class="flex-shrink-1">
        <slot name="right" />
    </div>
</div>