<script>
    export let click;
</script>

<div role="button" class="card p-3 col-xs-12 col-sm-5 col-xl-4 col-xxl-3" style="height: 110px" on:click={click}>
    <slot />
</div>