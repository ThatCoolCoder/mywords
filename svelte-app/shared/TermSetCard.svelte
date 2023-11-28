<script>
    export let click;
</script>

<div role="button" class="card p-3 col-12 col-sm-6 col-md-4 col-lg-4 col-xxl-2" style="height: 110px" on:click={click}>
    <slot />
</div>