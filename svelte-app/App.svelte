<script>
	import { Router } from "@roxi/routify";
	
	import routes from "./.routify/routes.default.js";
    import ErrorPopup from "shared/misc/ErrorPopup.svelte";

	const config = {
		urlTransform: {
			remove: url => url.toLowerCase()
		}
	}
</script>

<Router {config} {routes} />
<ErrorPopup />