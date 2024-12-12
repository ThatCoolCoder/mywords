<script>
    export let click;
</script>

<div class="col-xs-12 col-sm-5 col-xl-4 col-xxl-3">
    <div role="button" class="card p-3" style="height: 7em" on:click={click}>
        <slot />
    </div>
</div>