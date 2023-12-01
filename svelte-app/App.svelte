<script>
	import { Router } from "@roxi/routify";
	import { routes } from "@roxi/routify/tmp/routes.js";

	const config = {
		urlTransform: {
			remove: url => url.toLowerCase()
		}
	}
</script>

<Router {config} {routes} />